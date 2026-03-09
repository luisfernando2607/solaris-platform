using System.Net;
using System.Text.Json;
using SolarisPlatform.Application.Common.Models;

namespace SolarisPlatform.API.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex, _env.IsDevelopment());
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, bool isDevelopment)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ha ocurrido un error en el servidor";
        List<string>? errors = null;

        switch (exception)
        {
            case FluentValidation.ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Error de validación";
                errors = validationException.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();
                break;

            case SolarisPlatform.Domain.Exceptions.ValidationException domainValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = domainValidationException.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "No autorizado";
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Recurso no encontrado";
                break;

            case InvalidOperationException invalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                message = invalidOperationException.Message;
                break;

            default:
                if (isDevelopment)
                    message = exception.Message;
                break;
        }

        // En desarrollo: construir respuesta enriquecida con inner exception
        object response;
        if (isDevelopment)
        {
            response = new
            {
                success   = false,
                message,
                data      = (object?)null,
                errors,
                timestamp = DateTime.UtcNow,
                // ── Debug info ──────────────────────────────────────
                debug = new
                {
                    exceptionType    = exception.GetType().FullName,
                    exceptionMessage = exception.Message,
                    innerException   = BuildInnerChain(exception.InnerException),
                    stackTrace       = exception.StackTrace?.Split('\n')
                                               .Take(8)
                                               .Select(l => l.Trim())
                                               .Where(l => l.Length > 0)
                                               .ToArray()
                }
            };
        }
        else
        {
            response = ApiResponse.Fail(message: message, errors: errors);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    /// <summary>
    /// Construye la cadena completa de inner exceptions como array de objetos.
    /// Permite ver el error real de PostgreSQL aunque esté 3 niveles adentro.
    /// </summary>
    private static object? BuildInnerChain(Exception? ex)
    {
        if (ex is null) return null;

        var chain = new List<object>();
        var current = ex;
        int depth = 0;

        while (current is not null && depth < 5)
        {
            chain.Add(new
            {
                type    = current.GetType().Name,
                message = current.Message,
                inner   = (object?)null   // se encadena a continuación
            });
            current = current.InnerException;
            depth++;
        }

        return chain;
    }
}

/// <summary>
/// Extensión para registrar el middleware de manejo de excepciones
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
