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
using SolarisPlatform.Domain.Interfaces.Catalogos;
using SolarisPlatform.Infrastructure.Services.Catalogos;
using SolarisPlatform.Infrastructure.Persistence.Repositories.Catalogos;
// ── RRHH ──────────────────────────────────────────────────────────────
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Services.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Repositories.RRHH;
using SolarisPlatform.Domain.Interfaces.RRHH;

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
        // REPOSITORIOS — Core
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
        // SERVICIOS DE APLICACIÓN — Core
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
        var jwtIssuer    = configuration["Jwt:Issuer"]    ?? "SolarisPlatform";
        var jwtAudience  = configuration["Jwt:Audience"]  ?? "SolarisPlatformUsers";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                ValidateIssuer           = true,
                ValidIssuer              = jwtIssuer,
                ValidateAudience         = true,
                ValidAudience            = jwtAudience,
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers["Token-Expired"] = "true";
                    return Task.CompletedTask;
                }
            };
        });

        // ==========================================
        // CATÁLOGOS — Servicios
        // ==========================================
        services.AddScoped<IPaisService,                PaisService>();
        services.AddScoped<IEstadoProvinciaService,     EstadoProvinciaService>();
        services.AddScoped<ICiudadService,              CiudadService>();
        services.AddScoped<IMonedaService,              MonedaService>();
        services.AddScoped<ITipoIdentificacionService,  TipoIdentificacionService>();
        services.AddScoped<IImpuestoService,            ImpuestoService>();
        services.AddScoped<IFormaPagoService,           FormaPagoService>();
        services.AddScoped<IBancoService,               BancoService>();

        // CATÁLOGOS — Repositorios
        services.AddScoped<IPaisRepository,                PaisRepository>();
        services.AddScoped<IEstadoProvinciaRepository,     EstadoProvinciaRepository>();
        services.AddScoped<ICiudadRepository,              CiudadRepository>();
        services.AddScoped<IMonedaRepository,              MonedaRepository>();
        services.AddScoped<ITipoIdentificacionRepository,  TipoIdentificacionRepository>();
        services.AddScoped<IImpuestoRepository,            ImpuestoRepository>();
        services.AddScoped<IFormaPagoRepository,           FormaPagoRepository>();
        services.AddScoped<IBancoRepository,               BancoRepository>();

        // ==========================================
        // RRHH — Servicios
        // ==========================================
        services.AddScoped<IDepartamentoService,    DepartamentoService>();
        services.AddScoped<IPuestoService,          PuestoService>();
        services.AddScoped<IEmpleadoService,        EmpleadoService>();
        services.AddScoped<IAsistenciaService,      AsistenciaService>();
        services.AddScoped<IConceptoNominaService,  ConceptoNominaService>();
        services.AddScoped<INominaService,          NominaService>();
        services.AddScoped<IPrestamoService,        PrestamoService>();
        services.AddScoped<IEvaluacionService,      EvaluacionService>();
        services.AddScoped<ICapacitacionService,    CapacitacionService>();
        services.AddScoped<IRrhhDashboardService,   RrhhDashboardService>();

        // RRHH — Repositorios
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<IPuestoRepository,       PuestoRepository>();
        services.AddScoped<IEmpleadoRepository,     EmpleadoRepository>();

        // ==========================================
        // MISC
        // ==========================================
        
        // ==========================================
        // RRHH — Repositorios
        // ==========================================
        services.AddScoped<IDepartamentoRepository,         DepartamentoRepository>();
        services.AddScoped<IPuestoRepository,               PuestoRepository>();
        services.AddScoped<IEmpleadoRepository,             EmpleadoRepository>();
        services.AddScoped<IAsistenciaRepository,           AsistenciaRepository>();
        services.AddScoped<ISolicitudAusenciaRepository,    SolicitudAusenciaRepository>();
        services.AddScoped<ISaldoVacacionesRepository,      SaldoVacacionesRepository>();
        services.AddScoped<IConceptoNominaRepository,       ConceptoNominaRepository>();
        services.AddScoped<IPeriodoNominaRepository,        PeriodoNominaRepository>();
        services.AddScoped<IRolPagoRepository,              RolPagoRepository>();
        services.AddScoped<IParametroNominaRepository,      ParametroNominaRepository>();
        services.AddScoped<IPrestamoRepository,             PrestamoRepository>();
        services.AddScoped<IEvaluacionRepository,           EvaluacionRepository>();
        services.AddScoped<ICapacitacionRepository,         CapacitacionRepository>();

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        return services;
    }
}