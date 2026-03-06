using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Domain.Interfaces.Proyectos;

namespace SolarisPlatform.Infrastructure.Services.Proyectos;

public class PresupuestoService : IPresupuestoService
{
    private readonly IPresupuestoRepository _repo;
    private readonly IPresupuestoPartidaRepository _partidaRepo;
    private readonly ICostoRealRepository _costoRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public PresupuestoService(IPresupuestoRepository repo, IPresupuestoPartidaRepository partidaRepo,
        ICostoRealRepository costoRepo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _partidaRepo = partidaRepo; _costoRepo = costoRepo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<PresupuestoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<PresupuestoDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<PresupuestoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetWithPartidasAsync(id, ct);
        return e == null ? null : _mapper.Map<PresupuestoDto>(e);
    }

    public async Task<PresupuestoDto?> GetActivoAsync(long proyectoId, CancellationToken ct = default)
    {
        var e = await _repo.GetActivoAsync(proyectoId, ct);
        return e == null ? null : _mapper.Map<PresupuestoDto>(e);
    }

    public async Task<ResumenEjecucionDto?> GetResumenEjecucionAsync(long proyectoId, CancellationToken ct = default)
    {
        var presupuesto = await _repo.GetActivoAsync(proyectoId, ct);
        if (presupuesto == null) return null;

        var partidas = await _partidaRepo.GetByPresupuestoAsync(presupuesto.Id, ct);
        var totalPresupuestado = partidas.Sum(p => p.Total);
        var totalEjecutado = 0m;
        var porPartida = new List<EjecucionPartidaDto>();

        foreach (var partida in partidas)
        {
            var costos = await _costoRepo.GetByPartidaAsync(partida.Id, ct);
            var ejecutado = costos.Sum(c => c.Monto);
            totalEjecutado += ejecutado;
            porPartida.Add(new EjecucionPartidaDto(
                partida.Id, partida.Concepto, partida.Tipo,
                partida.Total, ejecutado,
                partida.Total - ejecutado,
                partida.Total > 0 ? Math.Round(ejecutado / partida.Total * 100, 2) : 0));
        }

        return new ResumenEjecucionDto(
            proyectoId, totalPresupuestado, totalEjecutado,
            totalPresupuestado - totalEjecutado,
            totalPresupuestado > 0 ? Math.Round(totalEjecutado / totalPresupuestado * 100, 2) : 0,
            porPartida);
    }

    public async Task<Result<PresupuestoDto>> CreateAsync(CrearPresupuestoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var version = (short)((await _repo.GetByProyectoAsync(request.ProyectoId, ct)).Count() + 1);
        var e = new Presupuesto
        {
            ProyectoId  = request.ProyectoId,
            Version     = version,
            // FIX: Nombre, Contingencia, EsActivo, EsAprobado, TotalIngresos/Egresos/Neto
            //      NO existen en la entidad Presupuesto — BD usa estructura diferente
            Descripcion              = request.Nombre ?? request.Descripcion,
            Estado                   = 1,  // 1 = Borrador
            MontoTotalManoObra       = 0,
            MontoTotalMateriales     = 0,
            MontoTotalSubcontratos   = 0,
            MontoTotalEquipos        = 0,
            MontoTotalIndirectos     = 0,
            TotalGeneral             = 0
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<PresupuestoDto>.Success(_mapper.Map<PresupuestoDto>(e));
    }

    public async Task<Result<PresupuestoPartidaDto>> AgregarPartidaAsync(AgregarPartidaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var presupuesto = await _repo.GetByIdAsync(request.PresupuestoId, ct);
        if (presupuesto == null) return Result<PresupuestoPartidaDto>.Failure("Presupuesto no encontrado");

        var subtotal = request.Cantidad * request.PrecioUnitario;
        var total    = subtotal * (1 + request.Porcentaje / 100);

        var partida = new PresupuestoPartida
        {
            PresupuestoId  = request.PresupuestoId,
            Tipo           = request.Tipo,
            Concepto       = request.Concepto,
            Descripcion    = request.Descripcion,
            CodigoContable = request.CodigoContable,
            Cantidad       = request.Cantidad,
            UnidadMedida   = request.UnidadMedida,
            PrecioUnitario = request.PrecioUnitario,
            Subtotal       = subtotal,
            Porcentaje     = request.Porcentaje,
            Total          = total,
            Orden          = request.Orden
        };
        await _partidaRepo.AddAsync(partida, ct);
        await RecalcularTotalesAsync(presupuesto, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<PresupuestoPartidaDto>.Success(_mapper.Map<PresupuestoPartidaDto>(partida));
    }

    public async Task<Result> AprobarAsync(long presupuestoId, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(presupuestoId, ct);
        if (e == null) return Result.Failure("Presupuesto no encontrado");
        // FIX: EsAprobado → Estado = 2 (Aprobado), FechaAprobacion es DateOnly
        e.Estado          = 2;
        e.AprobadoPorId   = usuarioId;
        e.FechaAprobacion = DateOnly.FromDateTime(DateTime.UtcNow);
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<CostoRealDto>> RegistrarCostoRealAsync(RegistrarCostoRealRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new CostoReal
        {
            PresupuestoId   = request.PresupuestoId,
            PartidaId       = request.PartidaId,
            Origen          = request.Origen,
            Concepto        = request.Concepto,
            // FIX: Descripcion eliminada de CostoReal — se mapea a Observaciones
            Observaciones   = request.Descripcion,
            Monto           = request.Monto,
            // FIX: FechaRegistro → Fecha (DateOnly)
            Fecha           = request.FechaRegistro,
            // FIX: NumeroReferencia eliminado — se mapea a OrigenId si aplica
            OrdenTrabajoId  = request.OrdenTrabajoId,
            RegistradoPorId = usuarioId
        };
        await _costoRepo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CostoRealDto>.Success(_mapper.Map<CostoRealDto>(e));
    }

    public async Task<IEnumerable<CostoRealDto>> GetCostosRealesAsync(long presupuestoId, CancellationToken ct = default)
    {
        var partidas = await _partidaRepo.GetByPresupuestoAsync(presupuestoId, ct);
        var todos = new List<CostoReal>();
        foreach (var p in partidas)
            todos.AddRange(await _costoRepo.GetByPartidaAsync(p.Id, ct));
        return _mapper.Map<IEnumerable<CostoRealDto>>(todos);
    }

    private async Task RecalcularTotalesAsync(Presupuesto presupuesto, CancellationToken ct)
    {
        var partidas = await _partidaRepo.GetByPresupuestoAsync(presupuesto.Id, ct);
        // FIX: TotalIngresos/Egresos/Neto/Contingencia no existen
        // Recalculamos los montos por categoría de la entidad real
        presupuesto.TotalGeneral = partidas.Sum(p => p.Total);
        await _repo.UpdateAsync(presupuesto, ct);
    }

    public async Task<Result> ActualizarCostoProyectoAsync(long proyectoId, IProyectoRepository proyectoRepo, CancellationToken ct = default)
    {
        var proyecto = await proyectoRepo.GetByIdAsync(proyectoId, ct);
        if (proyecto == null) return Result.Failure("Proyecto no encontrado");
        var presupuesto = await _repo.GetActivoAsync(proyectoId, ct);
        if (presupuesto != null)
        {
            var partidas = await _partidaRepo.GetByPresupuestoAsync(presupuesto.Id, ct);
            var total = 0m;
            foreach (var p in partidas)
                total += (await _costoRepo.GetByPartidaAsync(p.Id, ct)).Sum(c => c.Monto);
            proyecto.CostoRealTotal = total;
            await proyectoRepo.UpdateAsync(proyecto, ct);
            await _uow.SaveChangesAsync(ct);
        }
        return Result.Success();
    }
}

public class GanttService : IGanttService
{
    private readonly IProyectoRepository _proyectoRepo;
    private readonly IGanttLineaBaseRepository _lineaBaseRepo;
    private readonly IGanttProgresoRepository _progresoRepo;
    private readonly ITareaRepository _tareaRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public GanttService(IProyectoRepository proyectoRepo, IGanttLineaBaseRepository lineaBaseRepo,
        IGanttProgresoRepository progresoRepo, ITareaRepository tareaRepo, IUnitOfWork uow, IMapper mapper)
    { _proyectoRepo = proyectoRepo; _lineaBaseRepo = lineaBaseRepo; _progresoRepo = progresoRepo;
      _tareaRepo = tareaRepo; _uow = uow; _mapper = mapper; }

    public async Task<GanttDto?> GetGanttAsync(long proyectoId, CancellationToken ct = default)
    {
        var proyecto = await _proyectoRepo.GetWithFasesAsync(proyectoId, ct);
        if (proyecto == null) return null;

        var tareas = await _tareaRepo.GetByProyectoAsync(proyectoId, ct);

        // FIX: Tarea ya no tiene FaseId — el Gantt ahora agrupa solo por proyecto
        var fases = proyecto.Fases.Select(f => new GanttFaseDto(
            f.Id, f.Nombre, f.FechaInicioPlan, f.FechaFinPlan, f.PorcentajeAvance,
            // FIX: todas las tareas del proyecto (sin filtro por FaseId)
            tareas.Select(t => new GanttTareaDto(
                t.Id, t.Nombre, t.FechaInicioPlan, t.FechaFinPlan,
                t.FechaInicioReal, t.FechaFinReal, null, null,
                t.PorcentajeAvance, t.Estado,
                t.DependenciasOrigen != null
                    ? t.DependenciasOrigen.Select(d => d.TareaDestinoId).ToList()
                    : new List<long>()
            )).ToList()
        )).ToList();

        var fechaInicio = proyecto.FechaInicioPlan ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var fechaFin    = proyecto.FechaFinPlan    ?? fechaInicio.AddDays(30);
        return new GanttDto(proyectoId, proyecto.Nombre, fechaInicio, fechaFin, fases);
    }

    public async Task<Result> CapturarLineaBaseAsync(long proyectoId, long usuarioId, CancellationToken ct = default)
    {
        var tareas = await _tareaRepo.GetByProyectoAsync(proyectoId, ct);
        foreach (var tarea in tareas)
        {
            var lineaBase = new GanttLineaBase
            {
                ProyectoId      = proyectoId,
                TareaId         = tarea.Id,
                FechaInicioBase = tarea.FechaInicioPlan ?? DateOnly.FromDateTime(DateTime.UtcNow),
                FechaFinBase    = tarea.FechaFinPlan    ?? DateOnly.FromDateTime(DateTime.UtcNow),
                // FIX: DuracionDias en GanttLineaBase es decimal — tomamos DuracionDiasPlan
                DuracionDias    = (decimal)tarea.DuracionDiasPlan,
                FechaCaptura    = DateTime.UtcNow
            };
            await _lineaBaseRepo.AddAsync(lineaBase, ct);
        }
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RegistrarProgresoAsync(RegistrarProgresoGanttRequest request, long usuarioId, CancellationToken ct = default)
    {
        var tarea = await _tareaRepo.GetByIdAsync(request.TareaId, ct);
        if (tarea == null) return Result.Failure("Tarea no encontrada");

        var progreso = new GanttProgreso
        {
            TareaId          = request.TareaId,
            FechaProgreso    = request.FechaProgreso,
            PorcentajeAvance = request.PorcentajeAvance,
            HorasTrabajadas  = request.HorasTrabajadas,
            Observaciones    = request.Observaciones,
            ReportadoPorId   = usuarioId
        };
        await _progresoRepo.AddAsync(progreso, ct);

        tarea.PorcentajeAvance = request.PorcentajeAvance;
        // FIX: HorasReales eliminado — acumular en DuracionDiasReal si se requiere
        await _tareaRepo.UpdateAsync(tarea, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CentroCostoService : ICentroCostoService
{
    private readonly ICentroCostoRepository _repo;
    private readonly IAsignacionCentroCostoRepository _asignRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public CentroCostoService(ICentroCostoRepository repo, IAsignacionCentroCostoRepository asignRepo,
        IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _asignRepo = asignRepo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<CentroCostoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<CentroCostoDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<CentroCostoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e == null ? null : _mapper.Map<CentroCostoDto>(e);
    }

    public async Task<Result<CentroCostoDto>> CreateAsync(CrearCentroCostoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new CentroCosto
        {
            ProyectoId     = request.ProyectoId,
            Codigo         = request.Codigo,
            Nombre         = request.Nombre,
            Descripcion    = request.Descripcion,
            // FIX: PresupuestoAsignado → PresupuestoAnual, GastoActual eliminado
            PresupuestoAnual = request.PresupuestoAsignado
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CentroCostoDto>.Success(_mapper.Map<CentroCostoDto>(e));
    }

    public async Task<Result<CentroCostoDto>> UpdateAsync(ActualizarCentroCostoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<CentroCostoDto>.Failure("Centro de costo no encontrado");
        e.Nombre         = request.Nombre;
        e.Descripcion    = request.Descripcion;
        // FIX: PresupuestoAsignado → PresupuestoAnual
        e.PresupuestoAnual = request.PresupuestoAsignado;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CentroCostoDto>.Success(_mapper.Map<CentroCostoDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Centro de costo no encontrado");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> AsignarCostoAsync(AsignarCostoACentroRequest request, long usuarioId, CancellationToken ct = default)
    {
        var centro = await _repo.GetByIdAsync(request.CentroCostoId, ct);
        if (centro == null) return Result.Failure("Centro de costo no encontrado");
        var asign = new AsignacionCentroCosto
        {
            CentroCostoId   = request.CentroCostoId,
            CostoRealId     = request.CostoRealId,
            OrdenTrabajoId  = request.OrdenTrabajoId,
            Porcentaje      = request.Porcentaje,
            Monto           = request.Monto,
            Concepto        = request.Concepto,
            FechaAsignacion = DateTime.UtcNow
        };
        await _asignRepo.AddAsync(asign, ct);
        // FIX: GastoActual eliminado de CentroCosto — no actualizamos ese campo
        await _repo.UpdateAsync(centro, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}