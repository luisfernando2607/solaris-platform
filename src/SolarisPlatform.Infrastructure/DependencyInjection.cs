using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Infrastructure.Identity;
using SolarisPlatform.Infrastructure.Persistence.Context;
using SolarisPlatform.Infrastructure.Persistence.Repositories;
using SolarisPlatform.Infrastructure.Services;

namespace SolarisPlatform.Infrastructure;

/// <summary>
/// Configuración de inyección de dependencias de Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ==========================================
        // BASE DE DATOS
        // ==========================================
        services.AddDbContext<SolarisDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // ==========================================
        // REPOSITORIOS
        // ==========================================
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<ISesionUsuarioRepository, SesionUsuarioRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<ISucursalRepository, SucursalRepository>();
        services.AddScoped<IModuloRepository, ModuloRepository>();
        services.AddScoped<IPermisoRepository, PermisoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ==========================================
        // SERVICIOS DE APLICACIÓN
        // ==========================================
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IRolService, RolService>();

        // ==========================================
        // SERVICIOS DE INFRAESTRUCTURA
        // ==========================================
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ==========================================
        // AUTENTICACIÓN JWT
        // ==========================================
        var jwtSecretKey = configuration["Jwt:SecretKey"] 
            ?? throw new ArgumentNullException("Jwt:SecretKey no configurado");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "SolarisPlatform";
        var jwtAudience = configuration["Jwt:Audience"] ?? "SolarisPlatformUsers";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        return services;
    }
}
