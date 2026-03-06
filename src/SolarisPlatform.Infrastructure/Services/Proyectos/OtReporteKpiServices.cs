using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Enums.Proyectos;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Domain.Interfaces.Proyectos;

namespace SolarisPlatform.Infrastructure.Services.Proyectos;

public class OrdenTrabajoService : IOrdenTrabajoService
{
    private readonly IOrdenTrabajoRepository _repo;
    private readonly IOtActividadRepository _actRepo;
    private readonly IOtMaterialRepository _matRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public OrdenTrabajoService(IOrdenTrabajoRepository repo, IOtActividadRepository actRepo,
        IOtMaterialRepository matRepo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _actRepo = actRepo; _matRepo = matRepo; _uow = uow; _mapper = mapper; }

    public async Task<(IEnumerable<OrdenTrabajoListDto> Items, int Total)> GetListAsync(
        FiltroOrdenesTrabajoRequest filtro, CancellationToken ct = default)
    {
        var (items, total) = await _repo.GetPagedAsync(
            filtro.EmpresaId!.Value, filtro.Busqueda, filtro.Estado,
            filtro.ProyectoId, filtro.CuadrillaId, filtro.Pagina, filtro.ElementosPorPagina, ct);
        return (_mapper.Map<IEnumerable<OrdenTrabajoListDto>>(items), total);
    }

    public async Task<OrdenTrabajoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        // GetByIdAsync genérico - no hay GetWithDetallesAsync en esta entidad
        var e = await _repo.GetByIdAsync(id, ct);
        return e == null ? null : _mapper.Map<OrdenTrabajoDto>(e);
    }

    public async Task<IEnumerable<OrdenTrabajoListDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<OrdenTrabajoListDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<Result<OrdenTrabajoDto>> CreateAsync(CrearOrdenTrabajoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var numero = await GenerarNumeroOtAsync(request.ProyectoId, ct);
        var e = new OrdenTrabajo
        {
            ProyectoId           = request.ProyectoId,
            TareaId              = request.TareaId,
            CuadrillaId          = request.CuadrillaId,
            TecnicoResponsableId = request.TecnicoResponsableId,
            Numero               = numero,
            Titulo               = request.Titulo,
            Descripcion          = request.Descripcion,
            Estado               = EstadoOrdenTrabajo.Borrador,
            FechaAsignacion      = DateTime.UtcNow,
            FechaInicioPlan      = request.FechaInicioPlan,
            FechaFinPlan         = request.FechaFinPlan,
            AsignadoPorId        = usuarioId,
            Latitud              = request.Latitud,
            Longitud             = request.Longitud,
            DireccionSitio       = request.DireccionSitio,
            RequiereFirma        = request.RequiereFirma,
            RequiereFotos        = request.RequiereFotos,
            FotosRequeridas      = request.FotosRequeridas
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);

        foreach (var (act, i) in request.Actividades.Select((a, i) => (a, i)))
        {
            await _actRepo.AddAsync(new OtActividad
            {
                OrdenTrabajoId = e.Id, Nombre = act.Nombre,
                Descripcion = act.Descripcion, Orden = i + 1, Completada = false
            }, ct);
        }
        foreach (var mat in request.Materiales)
        {
            await _matRepo.AddAsync(new OtMaterial
            {
                OrdenTrabajoId = e.Id, NombreMaterial = mat.NombreMaterial,
                CodigoMaterial = mat.CodigoMaterial, UnidadMedida = mat.UnidadMedida,
                CantidadPlan = mat.CantidadPlan, CantidadReal = 0,
                CostoUnitario = mat.CostoUnitario, CostoTotal = mat.CantidadPlan * mat.CostoUnitario
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        return Result<OrdenTrabajoDto>.Success(_mapper.Map<OrdenTrabajoDto>(e));
    }

    public async Task<Result<OrdenTrabajoDto>> UpdateAsync(ActualizarOrdenTrabajoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<OrdenTrabajoDto>.Failure("Orden de trabajo no encontrada");
        e.Titulo = request.Titulo; e.Descripcion = request.Descripcion;
        e.CuadrillaId = request.CuadrillaId; e.TecnicoResponsableId = request.TecnicoResponsableId;
        e.FechaInicioPlan = request.FechaInicioPlan; e.FechaFinPlan = request.FechaFinPlan;
        e.Latitud = request.Latitud; e.Longitud = request.Longitud; e.DireccionSitio = request.DireccionSitio;
        e.RequiereFirma = request.RequiereFirma; e.RequiereFotos = request.RequiereFotos;
        e.FotosRequeridas = request.FotosRequeridas; e.ObservacionesSupervisor = request.ObservacionesSupervisor;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<OrdenTrabajoDto>.Success(_mapper.Map<OrdenTrabajoDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Orden de trabajo no encontrada");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> CambiarEstadoAsync(CambiarEstadoOtRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Orden de trabajo no encontrada");
        e.Estado = request.Estado;
        if (request.Estado == EstadoOrdenTrabajo.EnCurso && e.FechaInicioReal == null)
            e.FechaInicioReal = DateTime.UtcNow;
        if (request.Estado == EstadoOrdenTrabajo.Completada && e.FechaFinReal == null)
            e.FechaFinReal = DateTime.UtcNow;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> CompletarActividadAsync(CompletarActividadRequest request, long usuarioId, CancellationToken ct = default)
    {
        var actividad = await _actRepo.GetByIdAsync(request.ActividadId, ct);
        if (actividad == null || actividad.OrdenTrabajoId != request.OtId)
            return Result.Failure("Actividad no encontrada");
        actividad.Completada      = true;
        actividad.FechaComplecion = DateTime.UtcNow;
        actividad.CompletadaPorId = usuarioId;
        actividad.Observaciones   = request.Observaciones;
        await _actRepo.UpdateAsync(actividad, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RegistrarFirmaAsync(RegistrarFirmaOtRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Orden de trabajo no encontrada");
        e.FirmaBase64      = request.FirmaBase64;
        e.FirmadoPorNombre = request.FirmantNombre;
        e.FechaFirma       = DateTime.UtcNow;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<string> GenerarNumeroOtAsync(long proyectoId, CancellationToken ct)
    {
        var count = await _repo.CountAsync(o => o.ProyectoId == proyectoId, ct);
        return $"OT-{proyectoId:D6}-{(count + 1):D4}";
    }
}

public class ReporteAvanceService : IReporteAvanceService
{
    private readonly IReporteAvanceRepository _repo;
    private readonly IRepository<ReporteAvanceFoto> _fotoRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ReporteAvanceService(IReporteAvanceRepository repo, IRepository<ReporteAvanceFoto> fotoRepo,
        IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _fotoRepo = fotoRepo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<ReporteAvanceDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ReporteAvanceDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<ReporteAvanceDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetWithFotosAsync(id, ct);
        return e == null ? null : _mapper.Map<ReporteAvanceDto>(e);
    }

    public async Task<Result<ReporteAvanceDto>> CreateAsync(CrearReporteAvanceRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new ReporteAvance
        {
            ProyectoId           = request.ProyectoId,
            OrdenTrabajoId       = request.OrdenTrabajoId,
            FechaReporte         = request.FechaReporte.ToDateTime(TimeOnly.MinValue),
            Titulo               = request.Titulo,
            Descripcion          = request.Descripcion,
            PorcentajeAvancePlan = request.PorcentajeAvancePlan,
            PorcentajeAvanceReal = request.PorcentajeAvanceReal,
            Observaciones        = request.Observaciones,
            Latitud              = request.Latitud,
            Longitud             = request.Longitud,
            ReportadoPorId       = usuarioId
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ReporteAvanceDto>.Success(_mapper.Map<ReporteAvanceDto>(e));
    }

    public async Task<Result> AgregarFotoAsync(AgregarFotoReporteRequest request, long usuarioId, CancellationToken ct = default)
    {
        var reporte = await _repo.GetByIdAsync(request.ReporteId, ct);
        if (reporte == null) return Result.Failure("Reporte no encontrado");
        var foto = new ReporteAvanceFoto
        {
            ReporteId     = request.ReporteId,
            UrlStorage    = request.UrlStorage,
            NombreArchivo = request.NombreArchivo,
            Descripcion   = request.Descripcion,
            Latitud       = request.Latitud,
            Longitud      = request.Longitud,
            FechaFoto     = DateTime.UtcNow,
            Orden         = request.Orden
        };
        await _fotoRepo.AddAsync(foto, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> EliminarFotoAsync(long fotoId, long usuarioId, CancellationToken ct = default)
    {
        var foto = await _fotoRepo.GetByIdAsync(fotoId, ct);
        if (foto == null) return Result.Failure("Foto no encontrada");
        await _fotoRepo.DeleteAsync(foto, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class KpiService : IKpiService
{
    private readonly IKpiProyectoRepository _kpiRepo;
    private readonly IAlertaProyectoRepository _alertaRepo;
    private readonly IProyectoRepository _proyectoRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public KpiService(IKpiProyectoRepository kpiRepo, IAlertaProyectoRepository alertaRepo,
        IProyectoRepository proyectoRepo, IUnitOfWork uow, IMapper mapper)
    { _kpiRepo = kpiRepo; _alertaRepo = alertaRepo; _proyectoRepo = proyectoRepo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<KpiProyectoDto>> GetKpisAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<KpiProyectoDto>>(await _kpiRepo.GetByProyectoAsync(proyectoId, ct));

    public async Task CalcularKpisAsync(long proyectoId, CancellationToken ct = default)
    {
        var proyecto = await _proyectoRepo.GetDashboardAsync(proyectoId, ct);
        if (proyecto == null) return;

        var kpis = new List<KpiProyecto>();
        var bac = proyecto.PresupuestoTotal;
        var ev  = bac * (proyecto.PorcentajeAvanceReal / 100);
        var pv  = bac * (proyecto.PorcentajeAvancePlan / 100);
        var ac  = proyecto.CostoRealTotal;

        if (bac > 0)
        {
            kpis.Add(Kpi(proyectoId, TipoKpi.SPI, "Schedule Performance Index",
                pv > 0 ? Math.Round(ev / pv, 4) : 0, 1.0m));
            kpis.Add(Kpi(proyectoId, TipoKpi.CPI, "Cost Performance Index",
                ac > 0 ? Math.Round(ev / ac, 4) : 0, 1.0m));
            kpis.Add(Kpi(proyectoId, TipoKpi.CV, "Cost Variance", ev - ac, 0));
            kpis.Add(Kpi(proyectoId, TipoKpi.SV, "Schedule Variance", ev - pv, 0));
            var cpi = ac > 0 ? ev / ac : 1;
            kpis.Add(Kpi(proyectoId, TipoKpi.EAC, "Estimate At Completion",
                cpi > 0 ? Math.Round(bac / cpi, 2) : bac, null));
            kpis.Add(Kpi(proyectoId, TipoKpi.ETC, "Estimate To Complete",
                cpi > 0 ? Math.Round((bac - ev) / cpi, 2) : (bac - ev), null));
        }

        foreach (var kpi in kpis)
            await _kpiRepo.AddAsync(kpi, ct);

        await _uow.SaveChangesAsync(ct);
        await GenerarAlertasAutomaticasAsync(proyectoId, ct);
    }

    private KpiProyecto Kpi(long proyectoId, TipoKpi tipo, string nombre, decimal valor, decimal? meta)
        => new() { ProyectoId = proyectoId, TipoKpi = tipo, Nombre = nombre,
                   Valor = valor, ValorMeta = meta, FechaCalculo = DateTime.UtcNow };

    public async Task<IEnumerable<AlertaProyectoDto>> GetAlertasAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<AlertaProyectoDto>>(await _alertaRepo.GetByProyectoAsync(proyectoId, ct));

    public async Task<int> ContarNoLeidasAsync(long proyectoId, CancellationToken ct = default)
        => await _alertaRepo.CountAsync(a => a.ProyectoId == proyectoId && !a.Leida, ct);

    public async Task<Result> MarcarLeidaAsync(long alertaId, CancellationToken ct = default)
    {
        var alerta = await _alertaRepo.GetByIdAsync(alertaId, ct);
        if (alerta == null) return Result.Failure("Alerta no encontrada");
        alerta.Leida = true;
        await _alertaRepo.UpdateAsync(alerta, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> MarcarTodasLeidasAsync(long proyectoId, long usuarioId, CancellationToken ct = default)
    {
        var alertas = await _alertaRepo.FindAsync(a => a.ProyectoId == proyectoId && !a.Leida, ct);
        foreach (var a in alertas) { a.Leida = true; await _alertaRepo.UpdateAsync(a, ct); }
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task GenerarAlertasAutomaticasAsync(long proyectoId, CancellationToken ct)
    {
        var kpis = await _kpiRepo.GetByProyectoAsync(proyectoId, ct);
        var alertas = new List<AlertaProyecto>();

        var spi = kpis.FirstOrDefault(k => k.TipoKpi == TipoKpi.SPI);
        if (spi != null && spi.Valor < 0.9m)
            alertas.Add(new AlertaProyecto { ProyectoId = proyectoId, TipoAlerta = TipoAlerta.RetrasoFecha,
                Severidad = spi.Valor < 0.7m ? SeveridadAlerta.Critica : SeveridadAlerta.Warning,
                Titulo = "SPI por debajo del umbral", Mensaje = $"SPI = {spi.Valor:F2}",
                FechaAlerta = DateTime.UtcNow, Leida = false, Resuelta = false });

        var cpi = kpis.FirstOrDefault(k => k.TipoKpi == TipoKpi.CPI);
        if (cpi != null && cpi.Valor < 0.9m)
            alertas.Add(new AlertaProyecto { ProyectoId = proyectoId, TipoAlerta = TipoAlerta.SobrecostoPresup,
                Severidad = cpi.Valor < 0.7m ? SeveridadAlerta.Critica : SeveridadAlerta.Warning,
                Titulo = "CPI por debajo del umbral", Mensaje = $"CPI = {cpi.Valor:F2}",
                FechaAlerta = DateTime.UtcNow, Leida = false, Resuelta = false });

        foreach (var alerta in alertas)
            await _alertaRepo.AddAsync(alerta, ct);

        await _uow.SaveChangesAsync(ct);
    }
}
