using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Entities.Catalogos;
using SolarisPlatform.Domain.Common;
using SolarisPlatform.Application.Common.Interfaces;

namespace SolarisPlatform.Infrastructure.Persistence.Context;

/// <summary>
/// Contexto de base de datos principal
/// </summary>
public class SolarisDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDateTimeService? _dateTimeService;

    public SolarisDbContext(DbContextOptions<SolarisDbContext> options) : base(options)
    {
    }

    public SolarisDbContext(
        DbContextOptions<SolarisDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeService dateTimeService) : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    // ==========================================
    // SEGURIDAD
    // ==========================================
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
    public DbSet<UsuarioSucursal> UsuarioSucursales => Set<UsuarioSucursal>();
    public DbSet<SesionUsuario> SesionesUsuario => Set<SesionUsuario>();

    // ==========================================
    // EMPRESAS
    // ==========================================
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Sucursal> Sucursales => Set<Sucursal>();

    // ==========================================
    // CATÁLOGOS
    // ==========================================
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<EstadoProvincia> EstadosProvincias => Set<EstadoProvincia>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();
    public DbSet<Moneda> Monedas => Set<Moneda>();
    public DbSet<TipoIdentificacion> TiposIdentificacion => Set<TipoIdentificacion>();
    public DbSet<Impuesto> Impuestos => Set<Impuesto>();
    public DbSet<FormaPago> FormasPago => Set<FormaPago>();
    public DbSet<Banco> Bancos => Set<Banco>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas las configuraciones del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SolarisDbContext).Assembly);

        // Configurar esquemas
        modelBuilder.HasDefaultSchema("public");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.FechaCreacion = _dateTimeService?.UtcNow ?? DateTime.UtcNow;
                    entry.Entity.UsuarioCreacion = _currentUserService?.UsuarioId;
                    break;

                case EntityState.Modified:
                    entry.Entity.FechaModificacion = _dateTimeService?.UtcNow ?? DateTime.UtcNow;
                    entry.Entity.UsuarioModificacion = _currentUserService?.UsuarioId;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<SoftDeletableEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.Eliminado = true;
                entry.Entity.FechaEliminacion = _dateTimeService?.UtcNow ?? DateTime.UtcNow;
                entry.Entity.UsuarioEliminacion = _currentUserService?.UsuarioId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
