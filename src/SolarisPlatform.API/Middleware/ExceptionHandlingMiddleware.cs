using System.Net;
using System.Text.Json;
using SolarisPlatform.Application.Common.Models;

namespace SolarisPlatform.API.Middleware;

/// <summary>
/// Middleware para manejo global de excepciones
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ha ocurrido un error en el servidor";
        List<string>? errors = null;

        switch (exception)
        {
            case FluentValidation.ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Error de validación";
                // ✅ CORREGIDO: Convertir a List<string> en lugar de Dictionary
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
                // En producción, no revelar detalles del error
                #if DEBUG
                message = exception.Message;
                #endif
                break;
        }

        var response = ApiResponse.Fail(
            message: message,
            errors: errors
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// Extensión para registrar el middleware de manejo de excepciones
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}