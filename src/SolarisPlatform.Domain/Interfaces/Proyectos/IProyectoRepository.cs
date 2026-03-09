using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Interfaces.Proyectos;

// ──────────────────────────────────────────────────────────────────
// INTERFACES DE REPOSITORIOS - MÓDULO PROYECTOS
// Namespace: SolarisPlatform.Domain.Interfaces.Proyectos
// ──────────────────────────────────────────────────────────────────

/// <summary>Repositorio principal de proyectos</summary>
public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<Proyecto?>           GetByCodigoAsync(long empresaId, string codigo, CancellationToken ct = default);
    Task<IEnumerable<Proyecto>> GetByEmpresaAsync(long empresaId, CancellationToken ct = default);
    Task<IEnumerable<Proyecto>> GetByEstadoAsync(long empresaId, EstadoProyecto estado, CancellationToken ct = default);
    Task<Proyecto?>           GetWithFasesAsync(long id, CancellationToken ct = default);
    Task<Proyecto?>           GetWithWbsAsync(long id, CancellationToken ct = default);
    Task<Proyecto?>           GetDashboardAsync(long id, CancellationToken ct = default);
    Task<bool>                CodigoExisteAsync(long empresaId, string codigo, long? excluirId, CancellationToken ct = default);

    /// <summary>Proyectos paginados con filtros</summary>
    Task<(IEnumerable<Proyecto> Items, int Total)> GetPagedAsync(
        long empresaId,
        string? busqueda,
        EstadoProyecto? estado,
        TipoProyecto? tipo,
        long? gerenteId,
        int pagina,
        int elementosPorPagina,
        CancellationToken ct = default);
}

/// <summary>Repositorio de hitos del proyecto</summary>
public interface IProyectoHitoRepository : IRepository<ProyectoHito>
{
    Task<IEnumerable<ProyectoHito>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<ProyectoHito>> GetVencidosAsync(long empresaId, CancellationToken ct = default);
}

/// <summary>Repositorio de fases del proyecto</summary>
public interface IProyectoFaseRepository : IRepository<ProyectoFase>
{
    Task<IEnumerable<ProyectoFase>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<bool>                      CodigoExisteAsync(long proyectoId, string codigo, long? excluirId, CancellationToken ct = default);
}

/// <summary>Repositorio de documentos del proyecto</summary>
public interface IProyectoDocumentoRepository : IRepository<ProyectoDocumento>
{
    Task<IEnumerable<ProyectoDocumento>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
}

/// <summary>Repositorio de nodos WBS</summary>
public interface IWbsNodoRepository : IRepository<WbsNodo>
{
    Task<IEnumerable<WbsNodo>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<WbsNodo>> GetArbolAsync(long proyectoId, CancellationToken ct = default);
    Task<WbsNodo?>             GetWithHijosAsync(long id, CancellationToken ct = default);
    Task<bool>                 CodigoExisteAsync(long proyectoId, string codigo, long? excluirId, CancellationToken ct = default);
}

/// <summary>Repositorio de tareas</summary>
public interface ITareaRepository : IRepository<Tarea>
{
    Task<IEnumerable<Tarea>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<Tarea>> GetByFaseAsync(long faseId, CancellationToken ct = default);
    Task<IEnumerable<Tarea>> GetByCuadrillaAsync(long cuadrillaId, CancellationToken ct = default);
    Task<IEnumerable<Tarea>> GetVencidasAsync(long empresaId, CancellationToken ct = default);
    Task<Tarea?>             GetWithDependenciasAsync(long id, CancellationToken ct = default);
}

/// <summary>Repositorio de dependencias entre tareas</summary>
public interface ITareaDependenciaRepository : IRepository<TareaDependencia>
{
    Task<IEnumerable<TareaDependencia>> GetByTareaOrigenAsync(long tareaId, CancellationToken ct = default);
    Task<bool>                          ExisteAsync(long origenId, long destinoId, CancellationToken ct = default);
}

/// <summary>Repositorio de cuadrillas</summary>
public interface ICuadrillaRepository : IRepository<Cuadrilla>
{
    Task<IEnumerable<Cuadrilla>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<Cuadrilla?>             GetWithMiembrosAsync(long id, CancellationToken ct = default);
}

/// <summary>Repositorio de miembros de cuadrilla</summary>
public interface ICuadrillaMiembroRepository : IRepository<CuadrillaMiembro>
{
    Task<IEnumerable<CuadrillaMiembro>> GetByCuadrillaAsync(long cuadrillaId, CancellationToken ct = default);
    Task<bool>                           EmpleadoEnCuadrillaAsync(long cuadrillaId, long empleadoId, CancellationToken ct = default);
}

/// <summary>Repositorio de recursos del proyecto</summary>
public interface IRecursoProyectoRepository : IRepository<RecursoProyecto>
{
    Task<IEnumerable<RecursoProyecto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<RecursoProyecto>> GetByTareaAsync(long tareaId, CancellationToken ct = default);
}

/// <summary>Repositorio de presupuestos</summary>
public interface IPresupuestoRepository : IRepository<Presupuesto>
{
    Task<IEnumerable<Presupuesto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<Presupuesto?>             GetActivoAsync(long proyectoId, CancellationToken ct = default);
    Task<Presupuesto?>             GetWithPartidasAsync(long id, CancellationToken ct = default);
}

/// <summary>Repositorio de partidas de presupuesto</summary>
public interface IPresupuestoPartidaRepository : IRepository<PresupuestoPartida>
{
    Task<IEnumerable<PresupuestoPartida>> GetByPresupuestoAsync(long presupuestoId, CancellationToken ct = default);
}

/// <summary>Repositorio de costos reales</summary>
public interface ICostoRealRepository : IRepository<CostoReal>
{
    Task<IEnumerable<CostoReal>> GetByPartidaAsync(long partidaId, CancellationToken ct = default);
    Task<IEnumerable<CostoReal>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<decimal>                GetTotalByProyectoAsync(long proyectoId, CancellationToken ct = default);
}

/// <summary>Repositorio de líneas base del Gantt</summary>
public interface IGanttLineaBaseRepository : IRepository<GanttLineaBase>
{
    Task<IEnumerable<GanttLineaBase>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<GanttLineaBase>> GetByTareaAsync(long tareaId, CancellationToken ct = default);
}

/// <summary>Repositorio de progreso del Gantt</summary>
public interface IGanttProgresoRepository : IRepository<GanttProgreso>
{
    Task<IEnumerable<GanttProgreso>> GetByTareaAsync(long tareaId, CancellationToken ct = default);
    Task<IEnumerable<GanttProgreso>> GetByFechaAsync(long proyectoId, DateOnly desde, DateOnly hasta, CancellationToken ct = default);
}

/// <summary>Repositorio de centros de costo</summary>
public interface ICentroCostoRepository : IRepository<CentroCosto>
{
    Task<IEnumerable<CentroCosto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<bool>                     CodigoExisteAsync(long proyectoId, string codigo, long? excluirId, CancellationToken ct = default);
}

/// <summary>Repositorio de asignaciones de centro de costo</summary>
public interface IAsignacionCentroCostoRepository : IRepository<AsignacionCentroCosto>
{
    Task<IEnumerable<AsignacionCentroCosto>> GetByCentroCostoAsync(long centroCostoId, CancellationToken ct = default);
}

/// <summary>Repositorio de órdenes de trabajo</summary>
public interface IOrdenTrabajoRepository : IRepository<OrdenTrabajo>
{
    Task<OrdenTrabajo?>             GetByNumeroAsync(long empresaId, string numero, CancellationToken ct = default);
    Task<IEnumerable<OrdenTrabajo>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<OrdenTrabajo>> GetByCuadrillaAsync(long cuadrillaId, CancellationToken ct = default);
    Task<IEnumerable<OrdenTrabajo>> GetByEstadoAsync(long empresaId, EstadoOrdenTrabajo estado, CancellationToken ct = default);
    Task<OrdenTrabajo?>             GetWithActividadesAsync(long id, CancellationToken ct = default);
    Task<bool>                      NumeroExisteAsync(long empresaId, string numero, long? excluirId, CancellationToken ct = default);

    Task<(IEnumerable<OrdenTrabajo> Items, int Total)> GetPagedAsync(
        long empresaId,
        string? busqueda,
        EstadoOrdenTrabajo? estado,
        long? proyectoId,
        long? cuadrillaId,
        int pagina,
        int elementosPorPagina,
        CancellationToken ct = default);
}

/// <summary>Repositorio de actividades de OT</summary>
public interface IOtActividadRepository : IRepository<OtActividad>
{
    Task<IEnumerable<OtActividad>> GetByOrdenTrabajoAsync(long ordenTrabajoId, CancellationToken ct = default);
}

/// <summary>Repositorio de materiales de OT</summary>
public interface IOtMaterialRepository : IRepository<OtMaterial>
{
    Task<IEnumerable<OtMaterial>> GetByOrdenTrabajoAsync(long ordenTrabajoId, CancellationToken ct = default);
}

/// <summary>Repositorio de reportes de avance</summary>
public interface IReporteAvanceRepository : IRepository<ReporteAvance>
{
    Task<IEnumerable<ReporteAvance>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<ReporteAvance?>             GetUltimoAsync(long proyectoId, CancellationToken ct = default);
    Task<ReporteAvance?>             GetWithFotosAsync(long id, CancellationToken ct = default);
}

/// <summary>Repositorio de KPIs del proyecto</summary>
public interface IKpiProyectoRepository : IRepository<KpiProyecto>
{
    Task<IEnumerable<KpiProyecto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<KpiProyecto>> GetUltimosAsync(long proyectoId, int cantidad, CancellationToken ct = default);
}

/// <summary>Repositorio de alertas del proyecto</summary>
public interface IAlertaProyectoRepository : IRepository<AlertaProyecto>
{
    Task<IEnumerable<AlertaProyecto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<AlertaProyecto>> GetNoLeidasAsync(long usuarioId, CancellationToken ct = default);
    Task<int>                         ContarNoLeidasAsync(long usuarioId, CancellationToken ct = default);
    Task                              MarcarLeidaAsync(long id, CancellationToken ct = default);
    Task                              MarcarTodasLeidasAsync(long proyectoId, long usuarioId, CancellationToken ct = default);
}
