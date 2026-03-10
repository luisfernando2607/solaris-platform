// ══════════════════════════════════════════════════════════════════════
//  Integration/Common/SolarisWebApplicationFactory.cs
//  ─────────────────────────────────────────────────────────────────────
//  Levanta toda la API en memoria para integration tests.
//  Reemplaza PostgreSQL real con InMemory DB.
//  No necesita servidor corriendo — funciona en CI/CD.
// ══════════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Tests.Integration.Common;

/// <summary>
/// Factory que arranca la API completa en memoria.
/// Cada test recibe un cliente HTTP preconfigurado contra esta API.
/// </summary>
public class SolarisWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ── FIX: Remover TODOS los registros relacionados con DbContext/EF ──
            // Si no se hace esto, coexisten Npgsql + InMemory y EF explota.

            // 1. Remover DbContextOptions<SolarisDbContext>
            var optionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SolarisDbContext>));
            if (optionsDescriptor != null)
                services.Remove(optionsDescriptor);

            // 2. Remover el DbContext registrado directamente
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(SolarisDbContext));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            // 3. Remover IDbContextOptionsConfiguration (registros internos de Npgsql)
            var internalDescriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            (d.ServiceType.FullName.Contains("IDbContextOptionsConfiguration") ||
                             d.ServiceType.FullName.Contains("Npgsql")))
                .ToList();
            foreach (var d in internalDescriptors)
                services.Remove(d);

            // ── Registrar InMemory con BD aislada por ejecución ───────────────
            services.AddDbContext<SolarisDbContext>(options =>
            {
                options.UseInMemoryDatabase("SolarisTestDb");
            });

            // ── Inicializar datos semilla para los tests ──────────────────────
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SolarisDbContext>();
            db.Database.EnsureCreated();
            SolarisTestSeeder.Seed(db);
        });
    }
}
