using AutoMapper;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;
using SolarisPlatform.Domain.Enums.Proyectos;
using SolarisPlatform.Domain.Interfaces;
using SolarisPlatform.Domain.Interfaces.Proyectos;

namespace SolarisPlatform.Infrastructure.Services.Proyectos;

public class WbsService : IWbsService
{
    private readonly IWbsNodoRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public WbsService(IWbsNodoRepository repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<WbsNodoDto>> GetArbolAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<WbsNodoDto>>(await _repo.GetArbolAsync(proyectoId, ct));

    public async Task<WbsNodoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetWithHijosAsync(id, ct);
        return e == null ? null : _mapper.Map<WbsNodoDto>(e);
    }

    public async Task<Result<WbsNodoDto>> CreateNodoAsync(CrearWbsNodoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new WbsNodo
        {
            ProyectoId       = request.ProyectoId,
            FaseId           = request.FaseId,
            PadreId          = request.PadreId,
            Codigo           = request.Codigo,
            Nombre           = request.Nombre,
            Descripcion      = request.Descripcion,
            TipoNodo         = request.TipoNodo,
            Orden            = request.Orden,
            PorcentajePeso   = request.PorcentajePeso,
            PorcentajeAvance = 0,
            EsHoja           = true,
            Nivel            = 1
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<WbsNodoDto>.Success(_mapper.Map<WbsNodoDto>(e));
    }

    public async Task<Result<WbsNodoDto>> UpdateNodoAsync(ActualizarWbsNodoRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<WbsNodoDto>.Failure("Nodo WBS no encontrado");
        e.Nombre = request.Nombre; e.Descripcion = request.Descripcion;
        e.TipoNodo = request.TipoNodo; e.Orden = request.Orden;
        e.PorcentajePeso = request.PorcentajePeso;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<WbsNodoDto>.Success(_mapper.Map<WbsNodoDto>(e));
    }

    public async Task<Result> DeleteNodoAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Nodo WBS no encontrado");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class TareaService : ITareaService
{
    private readonly ITareaRepository _repo;
    private readonly ITareaDependenciaRepository _depRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public TareaService(ITareaRepository repo, ITareaDependenciaRepository depRepo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _depRepo = depRepo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<TareaListDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<TareaListDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<IEnumerable<TareaListDto>> GetByFaseAsync(long faseId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<TareaListDto>>(await _repo.GetByFaseAsync(faseId, ct));

    public async Task<TareaDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetWithDependenciasAsync(id, ct);
        return e == null ? null : _mapper.Map<TareaDto>(e);
    }

    public async Task<Result<TareaDto>> CreateAsync(CrearTareaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new Tarea
        {
            ProyectoId       = request.ProyectoId,
            WbsNodoId        = request.WbsNodoId,
            FaseId           = request.FaseId,
            CuadrillaId      = request.CuadrillaId,
            ResponsableId    = request.ResponsableId,
            Nombre           = request.Nombre,
            Descripcion      = request.Descripcion,
            Prioridad        = request.Prioridad,
            FechaInicioPlan  = request.FechaInicioPlan,
            FechaFinPlan     = request.FechaFinPlan,
            DuracionDias     = (decimal)(request.DuracionDias ?? 0),
            HorasEstimadas   = (decimal)(request.HorasEstimadas ?? 0),
            Latitud          = request.Latitud,
            Longitud         = request.Longitud,
            Ubicacion        = request.Ubicacion,
            Estado           = EstadoTarea.Pendiente,
            PorcentajeAvance = 0,
            HorasReales      = 0
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<TareaDto>.Success(_mapper.Map<TareaDto>(e));
    }

    public async Task<Result<TareaDto>> UpdateAsync(ActualizarTareaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<TareaDto>.Failure("Tarea no encontrada");
        e.Nombre = request.Nombre; e.Descripcion = request.Descripcion;
        e.Prioridad = request.Prioridad; e.WbsNodoId = request.WbsNodoId;
        e.FaseId = request.FaseId; e.CuadrillaId = request.CuadrillaId;
        e.ResponsableId = request.ResponsableId;
        e.FechaInicioPlan = request.FechaInicioPlan; e.FechaFinPlan = request.FechaFinPlan;
        e.FechaInicioReal = request.FechaInicioReal; e.FechaFinReal = request.FechaFinReal;
        e.DuracionDias    = (decimal)(request.DuracionDias ?? 0);
        e.HorasEstimadas  = (decimal)(request.HorasEstimadas ?? e.HorasEstimadas);
        e.Latitud = request.Latitud; e.Longitud = request.Longitud; e.Ubicacion = request.Ubicacion;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<TareaDto>.Success(_mapper.Map<TareaDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Tarea no encontrada");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> CambiarEstadoAsync(CambiarEstadoTareaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Tarea no encontrada");
        e.Estado = request.Estado;
        if (request.Estado == EstadoTarea.EnCurso && e.FechaInicioReal == null)
            e.FechaInicioReal = DateOnly.FromDateTime(DateTime.UtcNow);
        if (request.Estado == EstadoTarea.Completada)
        {
            e.PorcentajeAvance = 100;
            if (e.FechaFinReal == null) e.FechaFinReal = DateOnly.FromDateTime(DateTime.UtcNow);
        }
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ActualizarAvanceAsync(ActualizarAvanceTareaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result.Failure("Tarea no encontrada");
        e.PorcentajeAvance = request.Porcentaje;
        e.HorasReales += request.HorasTrabajadas;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<TareaDependenciaDto>> AgregarDependenciaAsync(CrearDependenciaTareaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new TareaDependencia
        {
            TareaOrigenId   = request.TareaOrigenId,
            TareaDestinoId  = request.TareaDestinoId,
            TipoDependencia = request.TipoDependencia,
            Desfase         = request.Desfase ?? 0
        };
        await _depRepo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<TareaDependenciaDto>.Success(_mapper.Map<TareaDependenciaDto>(e));
    }

    public async Task<Result> EliminarDependenciaAsync(long dependenciaId, long usuarioId, CancellationToken ct = default)
    {
        var e = await _depRepo.GetByIdAsync(dependenciaId, ct);
        if (e == null) return Result.Failure("Dependencia no encontrada");
        await _depRepo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CuadrillaService : ICuadrillaService
{
    private readonly ICuadrillaRepository _repo;
    private readonly ICuadrillaMiembroRepository _miembroRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public CuadrillaService(ICuadrillaRepository repo, ICuadrillaMiembroRepository miembroRepo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _miembroRepo = miembroRepo; _uow = uow; _mapper = mapper; }

    public async Task<IEnumerable<CuadrillaDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default)
        => _mapper.Map<IEnumerable<CuadrillaDto>>(await _repo.GetByProyectoAsync(proyectoId, ct));

    public async Task<CuadrillaDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var e = await _repo.GetWithMiembrosAsync(id, ct);
        return e == null ? null : _mapper.Map<CuadrillaDto>(e);
    }

    public async Task<Result<CuadrillaDto>> CreateAsync(CrearCuadrillaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = new Cuadrilla
        {
            ProyectoId   = request.ProyectoId,
            Nombre       = request.Nombre,
            Descripcion  = request.Descripcion,
            LiderId      = request.LiderId,
            CapacidadMax = request.CapacidadMax
        };
        await _repo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CuadrillaDto>.Success(_mapper.Map<CuadrillaDto>(e));
    }

    public async Task<Result<CuadrillaDto>> UpdateAsync(ActualizarCuadrillaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(request.Id, ct);
        if (e == null) return Result<CuadrillaDto>.Failure("Cuadrilla no encontrada");
        e.Nombre = request.Nombre; e.Descripcion = request.Descripcion;
        e.LiderId = request.LiderId; e.CapacidadMax = request.CapacidadMax;
        await _repo.UpdateAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<CuadrillaDto>.Success(_mapper.Map<CuadrillaDto>(e));
    }

    public async Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        if (e == null) return Result.Failure("Cuadrilla no encontrada");
        await _repo.DeleteAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> AgregarMiembroAsync(AgregarMiembroCuadrillaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var existente = await _miembroRepo.FindAsync(
            m => m.CuadrillaId == request.CuadrillaId && m.EmpleadoId == request.EmpleadoId && m.FechaSalida == null, ct);
        if (existente.Any()) return Result.Failure("El empleado ya es miembro activo de esta cuadrilla");
        var e = new CuadrillaMiembro
        {
            CuadrillaId  = request.CuadrillaId,
            EmpleadoId   = request.EmpleadoId,
            FechaIngreso = request.FechaIngreso,
            Rol          = request.Rol
        };
        await _miembroRepo.AddAsync(e, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RemoverMiembroAsync(RemoverMiembroCuadrillaRequest request, long usuarioId, CancellationToken ct = default)
    {
        var miembro = await _miembroRepo.FirstOrDefaultAsync(
            m => m.CuadrillaId == request.CuadrillaId && m.EmpleadoId == request.EmpleadoId && m.FechaSalida == null, ct);
        if (miembro == null) return Result.Failure("Miembro no encontrado en la cuadrilla");
        miembro.FechaSalida = request.FechaSalida;
        await _miembroRepo.UpdateAsync(miembro, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
