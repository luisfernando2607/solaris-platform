using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Enums.Proyectos;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Domain.Interfaces.Proyectos;

namespace SolarisPlatform.Infrastructure.Services.Proyectos;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ProyectoService(IProyectoRepository repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<(IEnumerable<ProyectoListDto> Items, int Total)> GetListAsync(
        FiltroProyectosRequest filtro, CancellationToken ct = default)
    {
        var (items, total) = await _repo.GetPagedAsync(
            filtro.EmpresaId!.Value, filtro.Busqueda, filtro.Estado, filtro.Tipo,
            filtro.GerenteId, filtro.Pagina, filtro.ElementosPorPagina, ct);
        return (_mapper.Map<IEnumerable<ProyectoListDto>>(items), total);
    }

    public async Task<ProyectoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetWithFasesAsync(id, ct);
        return e == null ? null : _mapper.Map<ProyectoDto>(e);
    }

    public async Task<ProyectoDashboardDto?> GetDashboardAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetDashboardAsync(id, ct);
        return e == null ? null : _mapper.Map<ProyectoDashboardDto>(e);
    }

    public async Task<Result<ProyectoDto>> CreateAsync(CrearProyectoRequest request, long usuarioId, CancellationToken ct = default)
    {
        if (await _repo.CodigoExisteAsync(request.EmpresaId, request.Codigo, null, ct))
            return Result<ProyectoDto>.Failure($"Ya existe un proyecto con código '{request.Codigo}'");
        var e = new Proyecto
        {
            EmpresaId = request.EmpresaId, Codigo = request.Codigo, Nombre = request.Nombre,
            Descripcion = request.Descripcion, TipoProyecto = request.TipoProyecto,
            Prioridad = request.Prioridad, FechaInicioPlan = request.FechaInicioPlan,
            FechaFinPlan = request.FechaFinPlan, MonedaId = request.MonedaId,
            PresupuestoTotal = request.PresupuestoTotal, ClienteId = request.ClienteId,
            GerenteProyectoId = request.GerenteProyectoId, ResponsableId = request.ResponsableId,
            SucursalId = request.SucursalId, Latitud = request.Latitud, Longitud = request.Longitud,
            Direccion = request.Direccion, Estado = EstadoProyecto.Borrador,
            PorcentajeAvancePlan = 0, PorcentajeAvanceReal = 0, CostoRealTotal = 0
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoDto>.Success(_mapper.Map<ProyectoDto>(e));
    }

    public async Task<Result<ProyectoDto>> UpdateAsync(ActualizarProyectoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<ProyectoDto>.Failure("Proyecto no encontrado");
        e.Nombre = request.Nombre; e.Descripcion = request.Descripcion;
        e.TipoProyecto = request.TipoProyecto; e.Prioridad = request.Prioridad;
        e.FechaInicioPlan = request.FechaInicioPlan; e.FechaFinPlan = request.FechaFinPlan;
        e.FechaInicioReal = request.FechaInicioReal; e.FechaFinReal = request.FechaFinReal;
        e.MonedaId = request.MonedaId; e.PresupuestoTotal = request.PresupuestoTotal;
        e.ClienteId = request.ClienteId; e.GerenteProyectoId = request.GerenteProyectoId;
        e.ResponsableId = request.ResponsableId; e.SucursalId = request.SucursalId;
        e.Latitud = request.Latitud; e.Longitud = request.Longitud; e.Direccion = request.Direccion;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoDto>.Success(_mapper.Map<ProyectoDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Proyecto no encontrado");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> CambiarEstadoAsync(CambiarEstadoProyectoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Proyecto no encontrado");
        e.Estado = request.Estado;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ActualizarAvanceAsync(ActualizarAvanceProyectoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Proyecto no encontrado");
        e.PorcentajeAvanceReal = request.AvanceReal;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ProyectoFaseService : IProyectoFaseService
{
    private readonly IProyectoFaseRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ProyectoFaseService(IProyectoFaseRepository repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<ProyectoFaseDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ProyectoFaseDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<ProyectoFaseDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e == null ? null : _mapper.Map<ProyectoFaseDto>(e);
    }

    public async Task<Result<ProyectoFaseDto>> CreateAsync(CrearProyectoFaseRequest request, long usuarioId, CancellationToken ct = default)
    {
        if (await _repo.CodigoExisteAsync(request.ProyectoId, request.Codigo, null, ct))
            return Result<ProyectoFaseDto>.Failure($"Ya existe una fase con código '{request.Codigo}'");
        var e = new ProyectoFase
        {
            EmpresaId = request.EmpresaId, ProyectoId = request.ProyectoId, Codigo = request.Codigo, Nombre = request.Nombre,
            Descripcion = request.Descripcion, Orden = request.Orden,
            FechaInicioPlan = request.FechaInicioPlan, FechaFinPlan = request.FechaFinPlan,
            Estado = EstadoFase.Pendiente, PorcentajeAvance = 0
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoFaseDto>.Success(_mapper.Map<ProyectoFaseDto>(e));
    }

    public async Task<Result<ProyectoFaseDto>> UpdateAsync(ActualizarProyectoFaseRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<ProyectoFaseDto>.Failure("Fase no encontrada");
        e.Nombre = request.Nombre; e.Descripcion = request.Descripcion; e.Orden = request.Orden;
        e.FechaInicioPlan = request.FechaInicioPlan; e.FechaFinPlan = request.FechaFinPlan;
        e.FechaInicioReal = request.FechaInicioReal; e.FechaFinReal = request.FechaFinReal;
        e.PorcentajeAvance = request.PorcentajeAvance; e.Estado = request.Estado;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoFaseDto>.Success(_mapper.Map<ProyectoFaseDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Fase no encontrada");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ProyectoHitoService : IProyectoHitoService
{
    private readonly IProyectoHitoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ProyectoHitoService(IProyectoHitoRepository repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<ProyectoHitoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ProyectoHitoDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<ProyectoHitoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e == null ? null : _mapper.Map<ProyectoHitoDto>(e);
    }

    public async Task<Result<ProyectoHitoDto>> CreateAsync(CrearProyectoHitoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new ProyectoHito
        {
            EmpresaId = request.EmpresaId, ProyectoId = request.ProyectoId, Nombre = request.Nombre, Descripcion = request.Descripcion,
            FechaCompromiso = request.FechaCompromiso, PorcentajePeso = request.PorcentajePeso,
            ResponsableId = request.ResponsableId, Orden = request.Orden, Estado = EstadoHito.Pendiente
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoHitoDto>.Success(_mapper.Map<ProyectoHitoDto>(e));
    }

    public async Task<Result<ProyectoHitoDto>> UpdateAsync(ActualizarProyectoHitoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<ProyectoHitoDto>.Failure("Hito no encontrado");
        e.Nombre = request.Nombre; e.Descripcion = request.Descripcion;
        e.FechaCompromiso = request.FechaCompromiso; e.PorcentajePeso = request.PorcentajePeso;
        e.ResponsableId = request.ResponsableId; e.Orden = request.Orden; e.Estado = request.Estado;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoHitoDto>.Success(_mapper.Map<ProyectoHitoDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Hito no encontrado");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> MarcarLogradoAsync(MarcarHitoLogradoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Hito no encontrado");
        e.Estado = EstadoHito.Logrado;
        e.FechaReal = request.FechaReal;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ProyectoDocumentoService : IProyectoDocumentoService
{
    private readonly IProyectoDocumentoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public ProyectoDocumentoService(IProyectoDocumentoRepository repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<ProyectoDocumentoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<ProyectoDocumentoDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<Result<ProyectoDocumentoDto>> CreateAsync(CrearProyectoDocumentoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new ProyectoDocumento
        {
            ProyectoId = request.ProyectoId, TipoDocumento = request.TipoDocumento,
            Nombre = request.Nombre, Descripcion = request.Descripcion,
            UrlStorage = request.UrlStorage, NombreArchivoOriginal = request.NombreArchivoOriginal,
            Extension = request.Extension, TamanoBytes = request.TamanoBytes,
            FechaSubida = DateTime.UtcNow
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<ProyectoDocumentoDto>.Success(_mapper.Map<ProyectoDocumentoDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Documento no encontrado");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
