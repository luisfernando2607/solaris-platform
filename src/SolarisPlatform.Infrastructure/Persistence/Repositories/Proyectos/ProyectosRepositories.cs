using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Enums.Proyectos;
using SolarisPlatform.Domain.Interfaces.Proyectos;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Persistence.Repositories.Proyectos;

public class ProyectoRepository : Repository<Proyecto>, IProyectoRepository
{
    private readonly SolarisDbContext _db;
    public ProyectoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<Proyecto?> GetByCodigoAsync(long empresaId, string codigo, CancellationToken ct = default)
        => await _db.Set<Proyecto>().FirstOrDefaultAsync(p => p.EmpresaId == empresaId && p.Codigo == codigo, ct);

    public async Task<IEnumerable<Proyecto>> GetByEmpresaAsync(long empresaId, CancellationToken ct = default)
        => await _db.Set<Proyecto>().Where(p => p.EmpresaId == empresaId).ToListAsync(ct);

    public async Task<IEnumerable<Proyecto>> GetByEstadoAsync(long empresaId, EstadoProyecto estado, CancellationToken ct = default)
        => await _db.Set<Proyecto>().Where(p => p.EmpresaId == empresaId && p.Estado == estado).ToListAsync(ct);

    public async Task<Proyecto?> GetWithFasesAsync(long id, CancellationToken ct = default)
        => await _db.Set<Proyecto>().Include(p => p.Fases).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Proyecto?> GetWithWbsAsync(long id, CancellationToken ct = default)
        => await _db.Set<Proyecto>().Include(p => p.WbsNodos).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Proyecto?> GetDashboardAsync(long id, CancellationToken ct = default)
        => await _db.Set<Proyecto>()
            .Include(p => p.Fases)
            .Include(p => p.Hitos)
            .Include(p => p.Kpis)
            .Include(p => p.Alertas)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<bool> CodigoExisteAsync(long empresaId, string codigo, long? excluirId, CancellationToken ct = default)
        => await _db.Set<Proyecto>().AnyAsync(p => p.EmpresaId == empresaId && p.Codigo == codigo && p.Id != excluirId, ct);

    public async Task<(IEnumerable<Proyecto> Items, int Total)> GetPagedAsync(
        long empresaId, string? busqueda, EstadoProyecto? estado, TipoProyecto? tipo,
        long? gerenteId, int pagina, int elementosPorPagina, CancellationToken ct = default)
    {
        var q = _db.Set<Proyecto>().Where(p => p.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(busqueda))
            q = q.Where(p => p.Nombre.Contains(busqueda) || p.Codigo.Contains(busqueda));
        if (estado.HasValue)    q = q.Where(p => p.Estado == estado.Value);
        if (tipo.HasValue)      q = q.Where(p => p.TipoProyecto == tipo.Value);
        if (gerenteId.HasValue) q = q.Where(p => p.GerenteProyectoId == gerenteId.Value);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((pagina - 1) * elementosPorPagina).Take(elementosPorPagina).ToListAsync(ct);
        return (items, total);
    }
}

public class ProyectoHitoRepository : Repository<ProyectoHito>, IProyectoHitoRepository
{
    private readonly SolarisDbContext _db;
    public ProyectoHitoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<ProyectoHito>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<ProyectoHito>().Where(h => h.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<ProyectoHito>> GetVencidosAsync(long empresaId, CancellationToken ct = default)
        => await _db.Set<ProyectoHito>()
            .Where(h => h.EmpresaId == empresaId
                     && h.Estado != EstadoHito.Logrado
                     && h.FechaCompromiso < DateOnly.FromDateTime(DateTime.Today))
            .ToListAsync(ct);
}

public class ProyectoFaseRepository : Repository<ProyectoFase>, IProyectoFaseRepository
{
    private readonly SolarisDbContext _db;
    public ProyectoFaseRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<ProyectoFase>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<ProyectoFase>().Where(f => f.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<bool> CodigoExisteAsync(long proyectoId, string codigo, long? excluirId, CancellationToken ct = default)
        => await _db.Set<ProyectoFase>().AnyAsync(f => f.ProyectoId == proyectoId && f.Codigo == codigo && f.Id != excluirId, ct);
}

public class ProyectoDocumentoRepository : Repository<ProyectoDocumento>, IProyectoDocumentoRepository
{
    private readonly SolarisDbContext _db;
    public ProyectoDocumentoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<ProyectoDocumento>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<ProyectoDocumento>().Where(d => d.ProyectoId == proyectoId).ToListAsync(ct);
}

public class WbsNodoRepository : Repository<WbsNodo>, IWbsNodoRepository
{
    private readonly SolarisDbContext _db;
    public WbsNodoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<WbsNodo>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<WbsNodo>().Where(n => n.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<WbsNodo>> GetArbolAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<WbsNodo>().Where(n => n.ProyectoId == proyectoId).Include(n => n.Hijos).ToListAsync(ct);

    public async Task<WbsNodo?> GetWithHijosAsync(long id, CancellationToken ct = default)
        => await _db.Set<WbsNodo>().Include(n => n.Hijos).FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<bool> CodigoExisteAsync(long proyectoId, string codigo, long? excluirId, CancellationToken ct = default)
        // FIX: Codigo → CodigoWbs en la entidad WbsNodo
        => await _db.Set<WbsNodo>().AnyAsync(n => n.ProyectoId == proyectoId && n.CodigoWbs == codigo && n.Id != excluirId, ct);
}

public class TareaRepository : Repository<Tarea>, ITareaRepository
{
    private readonly SolarisDbContext _db;
    public TareaRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<Tarea>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<Tarea>().Where(t => t.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<Tarea>> GetByFaseAsync(long faseId, CancellationToken ct = default)
        // FIX: Tarea ya no tiene FaseId — devuelve vacío o se filtra por WbsNodoId
        // Si se necesita filtrar por fase, hacerlo a través del WbsNodo
        => await _db.Set<Tarea>().Where(t => t.WbsNodoId == faseId).ToListAsync(ct);

    public async Task<IEnumerable<Tarea>> GetByCuadrillaAsync(long cuadrillaId, CancellationToken ct = default)
        => await _db.Set<Tarea>().Where(t => t.CuadrillaId == cuadrillaId).ToListAsync(ct);

    public async Task<IEnumerable<Tarea>> GetVencidasAsync(long empresaId, CancellationToken ct = default)
        => await _db.Set<Tarea>()
            .Where(t => t.EmpresaId == empresaId
                     && t.FechaFinPlan < DateOnly.FromDateTime(DateTime.Today)
                     && t.Estado != EstadoTarea.Completada)
            .ToListAsync(ct);

    public async Task<Tarea?> GetWithDependenciasAsync(long id, CancellationToken ct = default)
        => await _db.Set<Tarea>()
            .Include(t => t.DependenciasOrigen)
            .Include(t => t.DependenciasDestino)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
}

public class TareaDependenciaRepository : Repository<TareaDependencia>, ITareaDependenciaRepository
{
    private readonly SolarisDbContext _db;
    public TareaDependenciaRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<TareaDependencia>> GetByTareaOrigenAsync(long tareaId, CancellationToken ct = default)
        => await _db.Set<TareaDependencia>().Where(d => d.TareaOrigenId == tareaId).ToListAsync(ct);

    public async Task<bool> ExisteAsync(long origenId, long destinoId, CancellationToken ct = default)
        => await _db.Set<TareaDependencia>().AnyAsync(d => d.TareaOrigenId == origenId && d.TareaDestinoId == destinoId, ct);
}

public class CuadrillaRepository : Repository<Cuadrilla>, ICuadrillaRepository
{
    private readonly SolarisDbContext _db;
    public CuadrillaRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<Cuadrilla>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<Cuadrilla>().Where(c => c.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<Cuadrilla?> GetWithMiembrosAsync(long id, CancellationToken ct = default)
        => await _db.Set<Cuadrilla>().Include(c => c.Miembros).FirstOrDefaultAsync(c => c.Id == id, ct);
}

public class CuadrillaMiembroRepository : Repository<CuadrillaMiembro>, ICuadrillaMiembroRepository
{
    private readonly SolarisDbContext _db;
    public CuadrillaMiembroRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<CuadrillaMiembro>> GetByCuadrillaAsync(long cuadrillaId, CancellationToken ct = default)
        => await _db.Set<CuadrillaMiembro>().Where(m => m.CuadrillaId == cuadrillaId).ToListAsync(ct);

    public async Task<bool> EmpleadoEnCuadrillaAsync(long cuadrillaId, long empleadoId, CancellationToken ct = default)
        => await _db.Set<CuadrillaMiembro>().AnyAsync(m => m.CuadrillaId == cuadrillaId && m.EmpleadoId == empleadoId, ct);
}

public class RecursoProyectoRepository : Repository<RecursoProyecto>, IRecursoProyectoRepository
{
    private readonly SolarisDbContext _db;
    public RecursoProyectoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<RecursoProyecto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<RecursoProyecto>().Where(r => r.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<RecursoProyecto>> GetByTareaAsync(long tareaId, CancellationToken ct = default)
        => await _db.Set<RecursoProyecto>().Where(r => r.TareaId == tareaId).ToListAsync(ct);
}

public class PresupuestoRepository : Repository<Presupuesto>, IPresupuestoRepository
{
    private readonly SolarisDbContext _db;
    public PresupuestoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<Presupuesto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<Presupuesto>().Where(p => p.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<Presupuesto?> GetActivoAsync(long proyectoId, CancellationToken ct = default)
        // FIX: EsActivo no existe — "activo" = Estado == 1 (Borrador) o Estado == 2 (Aprobado)
        // Devolvemos el más reciente (mayor versión)
        => await _db.Set<Presupuesto>()
            .Where(p => p.ProyectoId == proyectoId)
            .OrderByDescending(p => p.Version)
            .FirstOrDefaultAsync(ct);

    public async Task<Presupuesto?> GetWithPartidasAsync(long id, CancellationToken ct = default)
        => await _db.Set<Presupuesto>().Include(p => p.Partidas).FirstOrDefaultAsync(p => p.Id == id, ct);
}

public class PresupuestoPartidaRepository : Repository<PresupuestoPartida>, IPresupuestoPartidaRepository
{
    private readonly SolarisDbContext _db;
    public PresupuestoPartidaRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<PresupuestoPartida>> GetByPresupuestoAsync(long presupuestoId, CancellationToken ct = default)
        => await _db.Set<PresupuestoPartida>().Where(p => p.PresupuestoId == presupuestoId).ToListAsync(ct);
}

public class CostoRealRepository : Repository<CostoReal>, ICostoRealRepository
{
    private readonly SolarisDbContext _db;
    public CostoRealRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<CostoReal>> GetByPartidaAsync(long partidaId, CancellationToken ct = default)
        => await _db.Set<CostoReal>().Where(c => c.PartidaId == partidaId).ToListAsync(ct);

    public async Task<IEnumerable<CostoReal>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<CostoReal>().Where(c => c.Presupuesto.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<decimal> GetTotalByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<CostoReal>()
            .Where(c => c.Presupuesto.ProyectoId == proyectoId)
            .SumAsync(c => c.Monto, ct);
}

public class GanttLineaBaseRepository : Repository<GanttLineaBase>, IGanttLineaBaseRepository
{
    private readonly SolarisDbContext _db;
    public GanttLineaBaseRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<GanttLineaBase>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<GanttLineaBase>().Where(g => g.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<GanttLineaBase>> GetByTareaAsync(long tareaId, CancellationToken ct = default)
        => await _db.Set<GanttLineaBase>().Where(g => g.TareaId == tareaId).ToListAsync(ct);
}

public class GanttProgresoRepository : Repository<GanttProgreso>, IGanttProgresoRepository
{
    private readonly SolarisDbContext _db;
    public GanttProgresoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<GanttProgreso>> GetByTareaAsync(long tareaId, CancellationToken ct = default)
        => await _db.Set<GanttProgreso>().Where(g => g.TareaId == tareaId).ToListAsync(ct);

    public async Task<IEnumerable<GanttProgreso>> GetByFechaAsync(long proyectoId, DateOnly desde, DateOnly hasta, CancellationToken ct = default)
        => await _db.Set<GanttProgreso>()
            .Where(g => g.Tarea.ProyectoId == proyectoId
                     && g.FechaProgreso >= desde
                     && g.FechaProgreso <= hasta)
            .ToListAsync(ct);
}

public class CentroCostoRepository : Repository<CentroCosto>, ICentroCostoRepository
{
    private readonly SolarisDbContext _db;
    public CentroCostoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<CentroCosto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<CentroCosto>().Where(c => c.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<bool> CodigoExisteAsync(long proyectoId, string codigo, long? excluirId, CancellationToken ct = default)
        => await _db.Set<CentroCosto>().AnyAsync(c => c.ProyectoId == proyectoId && c.Codigo == codigo && c.Id != excluirId, ct);
}

public class AsignacionCentroCostoRepository : Repository<AsignacionCentroCosto>, IAsignacionCentroCostoRepository
{
    private readonly SolarisDbContext _db;
    public AsignacionCentroCostoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<AsignacionCentroCosto>> GetByCentroCostoAsync(long centroCostoId, CancellationToken ct = default)
        => await _db.Set<AsignacionCentroCosto>().Where(a => a.CentroCostoId == centroCostoId).ToListAsync(ct);
}

public class OrdenTrabajoRepository : Repository<OrdenTrabajo>, IOrdenTrabajoRepository
{
    private readonly SolarisDbContext _db;
    public OrdenTrabajoRepository(SolarisDbContext db) : base(db) { _db = db; }

    // FIX: GetByNumeroAsync → GetByCodigoAsync internamente (Numero → Codigo)
    public async Task<OrdenTrabajo?> GetByNumeroAsync(long empresaId, string numero, CancellationToken ct = default)
        => await _db.Set<OrdenTrabajo>().FirstOrDefaultAsync(o => o.EmpresaId == empresaId && o.Codigo == numero, ct);

    public async Task<IEnumerable<OrdenTrabajo>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<OrdenTrabajo>().Where(o => o.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<OrdenTrabajo>> GetByCuadrillaAsync(long cuadrillaId, CancellationToken ct = default)
        => await _db.Set<OrdenTrabajo>().Where(o => o.CuadrillaId == cuadrillaId).ToListAsync(ct);

    public async Task<IEnumerable<OrdenTrabajo>> GetByEstadoAsync(long empresaId, EstadoOrdenTrabajo estado, CancellationToken ct = default)
        => await _db.Set<OrdenTrabajo>().Where(o => o.EmpresaId == empresaId && o.Estado == estado).ToListAsync(ct);

    public async Task<OrdenTrabajo?> GetWithActividadesAsync(long id, CancellationToken ct = default)
        => await _db.Set<OrdenTrabajo>().Include(o => o.Actividades).FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<bool> NumeroExisteAsync(long empresaId, string numero, long? excluirId, CancellationToken ct = default)
        // FIX: Numero → Codigo
        => await _db.Set<OrdenTrabajo>().AnyAsync(o => o.EmpresaId == empresaId && o.Codigo == numero && o.Id != excluirId, ct);

    public async Task<(IEnumerable<OrdenTrabajo> Items, int Total)> GetPagedAsync(
        long empresaId, string? busqueda, EstadoOrdenTrabajo? estado,
        long? proyectoId, long? cuadrillaId, int pagina, int elementosPorPagina, CancellationToken ct = default)
    {
        var q = _db.Set<OrdenTrabajo>().Where(o => o.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(busqueda))
            // FIX: Numero → Codigo, Titulo → Descripcion
            q = q.Where(o => o.Codigo.Contains(busqueda) || (o.Descripcion != null && o.Descripcion.Contains(busqueda)));
        if (estado.HasValue)      q = q.Where(o => o.Estado == estado.Value);
        if (proyectoId.HasValue)  q = q.Where(o => o.ProyectoId == proyectoId.Value);
        if (cuadrillaId.HasValue) q = q.Where(o => o.CuadrillaId == cuadrillaId.Value);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((pagina - 1) * elementosPorPagina).Take(elementosPorPagina).ToListAsync(ct);
        return (items, total);
    }
}

public class OtActividadRepository : Repository<OtActividad>, IOtActividadRepository
{
    private readonly SolarisDbContext _db;
    public OtActividadRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<OtActividad>> GetByOrdenTrabajoAsync(long ordenTrabajoId, CancellationToken ct = default)
        => await _db.Set<OtActividad>().Where(a => a.OrdenTrabajoId == ordenTrabajoId).ToListAsync(ct);
}

public class OtMaterialRepository : Repository<OtMaterial>, IOtMaterialRepository
{
    private readonly SolarisDbContext _db;
    public OtMaterialRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<OtMaterial>> GetByOrdenTrabajoAsync(long ordenTrabajoId, CancellationToken ct = default)
        => await _db.Set<OtMaterial>().Where(m => m.OrdenTrabajoId == ordenTrabajoId).ToListAsync(ct);
}

public class ReporteAvanceRepository : Repository<ReporteAvance>, IReporteAvanceRepository
{
    private readonly SolarisDbContext _db;
    public ReporteAvanceRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<ReporteAvance>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<ReporteAvance>().Where(r => r.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<ReporteAvance?> GetUltimoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<ReporteAvance>()
            .Where(r => r.ProyectoId == proyectoId)
            .OrderByDescending(r => r.FechaReporte)
            .FirstOrDefaultAsync(ct);

    public async Task<ReporteAvance?> GetWithFotosAsync(long id, CancellationToken ct = default)
        => await _db.Set<ReporteAvance>().Include(r => r.Fotos).FirstOrDefaultAsync(r => r.Id == id, ct);
}

public class KpiProyectoRepository : Repository<KpiProyecto>, IKpiProyectoRepository
{
    private readonly SolarisDbContext _db;
    public KpiProyectoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<KpiProyecto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<KpiProyecto>().Where(k => k.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<KpiProyecto>> GetUltimosAsync(long proyectoId, int cantidad, CancellationToken ct = default)
        => await _db.Set<KpiProyecto>()
            .Where(k => k.ProyectoId == proyectoId)
            .OrderByDescending(k => k.FechaCalculo)
            .Take(cantidad)
            .ToListAsync(ct);
}

public class AlertaProyectoRepository : Repository<AlertaProyecto>, IAlertaProyectoRepository
{
    private readonly SolarisDbContext _db;
    public AlertaProyectoRepository(SolarisDbContext db) : base(db) { _db = db; }

    public async Task<IEnumerable<AlertaProyecto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => await _db.Set<AlertaProyecto>().Where(a => a.ProyectoId == proyectoId).ToListAsync(ct);

    public async Task<IEnumerable<AlertaProyecto>> GetNoLeidasAsync(long usuarioId, CancellationToken ct = default)
        => await _db.Set<AlertaProyecto>().Where(a => !a.Leida && a.UsuarioId == usuarioId).ToListAsync(ct);

    public async Task<int> ContarNoLeidasAsync(long usuarioId, CancellationToken ct = default)
        => await _db.Set<AlertaProyecto>().CountAsync(a => !a.Leida && a.UsuarioId == usuarioId, ct);

    public async Task MarcarLeidaAsync(long id, CancellationToken ct = default)
    {
        var alerta = await _db.Set<AlertaProyecto>().FindAsync(new object[] { id }, ct);
        if (alerta != null) { alerta.Leida = true; alerta.FechaLeida = DateTime.UtcNow; await _db.SaveChangesAsync(ct); }
    }

    public async Task MarcarTodasLeidasAsync(long proyectoId, long usuarioId, CancellationToken ct = default)
    {
        var alertas = await _db.Set<AlertaProyecto>()
            .Where(a => a.ProyectoId == proyectoId && a.UsuarioId == usuarioId && !a.Leida)
            .ToListAsync(ct);
        var now = DateTime.UtcNow;
        alertas.ForEach(a => { a.Leida = true; a.FechaLeida = now; });
        await _db.SaveChangesAsync(ct);
    }
}
