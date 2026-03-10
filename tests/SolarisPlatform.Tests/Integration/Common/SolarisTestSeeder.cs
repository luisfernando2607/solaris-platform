using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Tests.Integration.Common;

public static class SolarisTestSeeder
{
    public static void Seed(SolarisDbContext db)
    {
        if (db.Usuarios.Any()) return;

        // Empresa requerida (EmpresaId es long, not nullable)
        var empresa = new Empresa
        {
            Codigo               = "TEST",
            RazonSocial          = "Empresa Test S.A.",
            TipoIdentificacion   = "RUC",
            NumeroIdentificacion = "9999999999001",
        };
        db.Empresas.Add(empresa);
        db.SaveChanges();

        // FIX: usar BCrypt igual que PasswordService de producción.
        // Antes usaba PasswordHasher<Usuario> de ASP.NET Identity,
        // que produce un hash incompatible con BCrypt.Net.BCrypt.Verify().
        var admin = new Usuario
        {
            EmpresaId       = empresa.Id,
            NombreUsuario   = "admin",
            Email           = TestConstants.AdminEmail,
            Nombres         = "Administrador",
            Apellidos       = "Sistema",
            Estado          = EstadoUsuario.Activo,
            EmailVerificado = true,
            PasswordHash    = BCrypt.Net.BCrypt.HashPassword(TestConstants.AdminPassword),
        };
        db.Usuarios.Add(admin);

        var testUser = new Usuario
        {
            EmpresaId       = empresa.Id,
            NombreUsuario   = "testuser",
            Email           = TestConstants.TestUserEmail,
            Nombres         = "Usuario",
            Apellidos       = "Prueba",
            Estado          = EstadoUsuario.Activo,
            EmailVerificado = true,
            PasswordHash    = BCrypt.Net.BCrypt.HashPassword(TestConstants.TestUserPassword),
        };
        db.Usuarios.Add(testUser);

        db.SaveChanges();
    }
}