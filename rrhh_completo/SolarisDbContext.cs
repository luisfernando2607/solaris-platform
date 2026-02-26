using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.Seguridad;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Entities.Catalogos;
using SolarisPlatform.Domain.Entities.RRHH;
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

    // ==========================================
    // RRHH — Estructura organizacional
    // ==========================================
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Puesto> Puestos => Set<Puesto>();

    // RRHH — Empleados
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<EmpleadoHistorial> EmpleadoHistorial => Set<EmpleadoHistorial>();
    public DbSet<EmpleadoDocumento> EmpleadoDocumentos => Set<EmpleadoDocumento>();
    public DbSet<EmpleadoConcepto> EmpleadoConceptos => Set<EmpleadoConcepto>();

    // RRHH — Reclutamiento
    public DbSet<RequisicionPersonal> RequisicionesPersonal => Set<RequisicionPersonal>();
    public DbSet<Candidato> Candidatos => Set<Candidato>();
    public DbSet<ProcesoSeleccion> ProcesosSeleccion => Set<ProcesoSeleccion>();
    public DbSet<Postulacion> Postulaciones => Set<Postulacion>();
    public DbSet<Entrevista> Entrevistas => Set<Entrevista>();

    // RRHH — Tiempo y asistencia
    public DbSet<Horario> Horarios => Set<Horario>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();
    public DbSet<SolicitudAusencia> SolicitudesAusencia => Set<SolicitudAusencia>();
    public DbSet<SaldoVacaciones> SaldosVacaciones => Set<SaldoVacaciones>();

    // RRHH — Nómina
    public DbSet<ConceptoNomina> ConceptosNomina => Set<ConceptoNomina>();
    public DbSet<PeriodoNomina> PeriodosNomina => Set<PeriodoNomina>();
    public DbSet<RolPago> RolesPago => Set<RolPago>();
    public DbSet<RolPagoEmpleado> RolesPagoEmpleado => Set<RolPagoEmpleado>();
    public DbSet<RolPagoDetalle> RolesPagoDetalle => Set<RolPagoDetalle>();

    // RRHH — Préstamos
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();
    public DbSet<PrestamoCuota> PrestamoCuotas => Set<PrestamoCuota>();

    // RRHH — Evaluación
    public DbSet<PlantillaEvaluacion> PlantillasEvaluacion => Set<PlantillaEvaluacion>();
    public DbSet<PlantillaCriterio> PlantillasCriterio => Set<PlantillaCriterio>();
    public DbSet<EvaluacionProceso> EvaluacionProcesos => Set<EvaluacionProceso>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<EvaluacionRespuesta> EvaluacionRespuestas => Set<EvaluacionRespuesta>();

    // RRHH — Capacitación
    public DbSet<Capacitacion> Capacitaciones => Set<Capacitacion>();
    public DbSet<CapacitacionParticipante> CapacitacionParticipantes => Set<CapacitacionParticipante>();

    // RRHH — Nómina parámetros
    public DbSet<ParametroNomina> ParametrosNomina => Set<ParametroNomina>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SolarisDbContext).Assembly);
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
