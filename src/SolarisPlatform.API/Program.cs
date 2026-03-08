using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Application;
using SolarisPlatform.Infrastructure;
using SolarisPlatform.Infrastructure.Persistence.Context;
using SolarisPlatform.API.Middleware;

// ============================================================
// BUILDER — Configuración de servicios
// ============================================================
var builder = WebApplication.CreateBuilder(args);

// Capas Clean Architecture
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers con JSON camelCase y sin nulls
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // CamelCase en respuestas (serialización)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        // Acepta tanto camelCase como PascalCase en requests (deserialización)
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Swagger con autenticación JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Solaris Platform API",
        Version = "v1",
        Description = "API del ERP Solaris Platform"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT. Ejemplo: eyJhbGciOiJIUzI1..."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS — en producción restringir al dominio del frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ============================================================
// APP — Pipeline de middlewares (el orden importa)
// ============================================================
var app = builder.Build();

// 1. Errores globales — debe ir primero
app.UseExceptionHandling();

// 2. Swagger UI — solo en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Solaris Platform API v1");
        c.RoutePrefix = "swagger";
    });
}

// 3. Archivos estáticos (index.html, health.html, api.html)
app.UseDefaultFiles();   // → / sirve index.html
app.UseStaticFiles();    // → /health.html, /api.html, etc.

// 4. HTTPS deshabilitado en desarrollo Linux
// app.UseHttpsRedirection();

// 5. CORS antes de Auth
app.UseCors("AllowAll");

// 6. Auth
app.UseAuthentication();
app.UseAuthorization();

// 7. Controllers
app.MapControllers();

// ============================================================
// HEALTH CHECKS
// ============================================================
// ESTRATEGIA DE RUTAS:
//   /health          → redirige al dashboard visual (health.html)
//   /health/full     → redirige al dashboard visual (health.html)
//   /health/data     → JSON con estado de API + BD  ← usado por health.html e index.html
//   /health/api      → JSON solo API
//   /health/db       → JSON solo BD
// ============================================================

// Rutas visuales — redirigen al dashboard HTML
app.MapGet("/health",      () => Results.Redirect("/pages/health.html"));
app.MapGet("/health/full", () => Results.Redirect("/pages/health.html"));

// ── /health/data — JSON completo (consumido por health.html e index.html) ──
// IMPORTANTE: Este es el único endpoint que devuelve datos reales de ambos servicios
// Tanto index.html como health.html deben hacer fetch a ESTA ruta
app.MapGet("/health/data", async (SolarisDbContext db) =>
{
    var sw      = System.Diagnostics.Stopwatch.StartNew();
    var dbOk    = false;
    var dbError = "";
    var dbName  = "";
    var dbMs    = 0L;

    try
    {
        dbOk   = await db.Database.CanConnectAsync();
        dbName = db.Database.GetDbConnection().Database;
    }
    catch (Exception ex)
    {
        dbError = ex.Message;
    }
    finally
    {
        dbMs = sw.ElapsedMilliseconds;
    }

    var allHealthy = dbOk;

    return Results.Json(new
    {
        status      = allHealthy ? "Healthy" : "Degraded",
        version     = "1.0.0",
        environment = app.Environment.EnvironmentName,
        timestamp   = DateTime.UtcNow,
        checks = new object[]
        {
            new { name = "api",      status = "Healthy",                        latencyMs = 0   },
            new { name = "database", status = dbOk ? "Healthy" : "Unhealthy",
                  database = dbName, latencyMs = dbMs,
                  error = string.IsNullOrEmpty(dbError) ? null : dbError }
        }
    }, statusCode: allHealthy ? 200 : 503);
});

// ── /health/api — JSON solo API (sin tocar la BD) ──
app.MapGet("/health/api", () => Results.Ok(new
{
    status      = "Healthy",
    service     = "Solaris Platform API",
    version     = "1.0.0",
    timestamp   = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));

// ── /health/db — JSON solo BD ──
app.MapGet("/health/db", async (SolarisDbContext db) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        sw.Stop();

        if (!canConnect)
            return Results.Json(new { status = "Unhealthy", database = "Sin conexión", latencyMs = sw.ElapsedMilliseconds }, statusCode: 503);

        return Results.Ok(new
        {
            status    = "Healthy",
            database  = db.Database.GetDbConnection().Database,
            server    = db.Database.GetDbConnection().DataSource,
            latencyMs = sw.ElapsedMilliseconds
        });
    }
    catch (Exception ex)
    {
        sw.Stop();
        return Results.Json(new { status = "Unhealthy", error = ex.Message, latencyMs = sw.ElapsedMilliseconds }, statusCode: 503);
    }
});

// ============================================================
// API INFO
// ============================================================
// Solo mantenemos el JSON en /api/info — eliminamos el redirect /api → api.html
// porque no aportaba valor diferente al Swagger
// ============================================================
// Redirige /test al panel de pruebas
app.MapGet("/test", () => Results.Redirect("/test.html"));

app.MapGet("/api/info", () => Results.Ok(new
{
    name        = "Solaris Platform API",
    version     = "1.0.0",
    description = "ERP empresarial on-premise",
    endpoints   = new
    {
        swagger    = "/swagger",
        healthData = "/health/data",
        healthApi  = "/health/api",
        healthDb   = "/health/db",
        auth       = "/api/auth",
        usuarios   = "/api/usuarios",
        roles      = "/api/roles",
        empresas   = "/api/empresas"
    }
}));

app.Run();