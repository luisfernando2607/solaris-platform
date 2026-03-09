using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;

namespace SolarisPlatform.Application.Common.Interfaces.Proyectos;

public interface IProyectoService
{
    Task<(IEnumerable<ProyectoListDto> Items, int Total)> GetListAsync(FiltroProyectosRequest filtro, CancellationToken ct = default);
    Task<ProyectoDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ProyectoDashboardDto?> GetDashboardAsync(long id, CancellationToken ct = default);
    Task<Result<ProyectoDto>> CreateAsync(CrearProyectoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<ProyectoDto>> UpdateAsync(ActualizarProyectoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
    Task<Result> CambiarEstadoAsync(CambiarEstadoProyectoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> ActualizarAvanceAsync(ActualizarAvanceProyectoRequest request, long usuarioId, CancellationToken ct = default);
}

public interface IProyectoFaseService
{
    Task<IEnumerable<ProyectoFaseDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<ProyectoFaseDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<ProyectoFaseDto>> CreateAsync(CrearProyectoFaseRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<ProyectoFaseDto>> UpdateAsync(ActualizarProyectoFaseRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
}

public interface IProyectoHitoService
{
    Task<IEnumerable<ProyectoHitoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<ProyectoHitoDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<ProyectoHitoDto>> CreateAsync(CrearProyectoHitoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<ProyectoHitoDto>> UpdateAsync(ActualizarProyectoHitoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
    Task<Result> MarcarLogradoAsync(MarcarHitoLogradoRequest request, long usuarioId, CancellationToken ct = default);
}

public interface IProyectoDocumentoService
{
    Task<IEnumerable<ProyectoDocumentoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<Result<ProyectoDocumentoDto>> CreateAsync(CrearProyectoDocumentoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
}

public interface IWbsService
{
    Task<IEnumerable<WbsNodoDto>> GetArbolAsync(long proyectoId, CancellationToken ct = default);
    Task<WbsNodoDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<WbsNodoDto>> CreateNodoAsync(CrearWbsNodoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<WbsNodoDto>> UpdateNodoAsync(ActualizarWbsNodoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteNodoAsync(long id, long usuarioId, CancellationToken ct = default);
}

public interface ITareaService
{
    Task<IEnumerable<TareaListDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<TareaListDto>> GetByFaseAsync(long faseId, CancellationToken ct = default);
    Task<TareaDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<TareaDto>> CreateAsync(CrearTareaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<TareaDto>> UpdateAsync(ActualizarTareaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
    Task<Result> CambiarEstadoAsync(CambiarEstadoTareaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> ActualizarAvanceAsync(ActualizarAvanceTareaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<TareaDependenciaDto>> AgregarDependenciaAsync(CrearDependenciaTareaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> EliminarDependenciaAsync(long dependenciaId, long usuarioId, CancellationToken ct = default);
}

public interface ICuadrillaService
{
    Task<IEnumerable<CuadrillaDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<CuadrillaDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<CuadrillaDto>> CreateAsync(CrearCuadrillaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<CuadrillaDto>> UpdateAsync(ActualizarCuadrillaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
    Task<Result> AgregarMiembroAsync(AgregarMiembroCuadrillaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> RemoverMiembroAsync(RemoverMiembroCuadrillaRequest request, long usuarioId, CancellationToken ct = default);
}

public interface IPresupuestoService
{
    Task<IEnumerable<PresupuestoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<PresupuestoDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<PresupuestoDto?> GetActivoAsync(long proyectoId, CancellationToken ct = default);
    Task<ResumenEjecucionDto?> GetResumenEjecucionAsync(long proyectoId, CancellationToken ct = default);
    Task<Result<PresupuestoDto>> CreateAsync(CrearPresupuestoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<PresupuestoPartidaDto>> AgregarPartidaAsync(AgregarPartidaRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> AprobarAsync(long presupuestoId, long usuarioId, CancellationToken ct = default);
    Task<Result<CostoRealDto>> RegistrarCostoRealAsync(RegistrarCostoRealRequest request, long usuarioId, CancellationToken ct = default);
    Task<IEnumerable<CostoRealDto>> GetCostosRealesAsync(long proyectoId, CancellationToken ct = default);
}

public interface IGanttService
{
    Task<GanttDto?> GetGanttAsync(long proyectoId, CancellationToken ct = default);
    Task<Result> CapturarLineaBaseAsync(long proyectoId, long usuarioId, CancellationToken ct = default);
    Task<Result> RegistrarProgresoAsync(RegistrarProgresoGanttRequest request, long usuarioId, CancellationToken ct = default);
}

public interface ICentroCostoService
{
    Task<IEnumerable<CentroCostoDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<CentroCostoDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<CentroCostoDto>> CreateAsync(CrearCentroCostoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<CentroCostoDto>> UpdateAsync(ActualizarCentroCostoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
    Task<Result> AsignarCostoAsync(AsignarCostoACentroRequest request, long usuarioId, CancellationToken ct = default);
}

public interface IOrdenTrabajoService
{
    Task<(IEnumerable<OrdenTrabajoListDto> Items, int Total)> GetListAsync(FiltroOrdenesTrabajoRequest filtro, CancellationToken ct = default);
    Task<OrdenTrabajoDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<OrdenTrabajoListDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<Result<OrdenTrabajoDto>> CreateAsync(CrearOrdenTrabajoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result<OrdenTrabajoDto>> UpdateAsync(ActualizarOrdenTrabajoRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> DeleteAsync(long id, long usuarioId, CancellationToken ct = default);
    Task<Result> CambiarEstadoAsync(CambiarEstadoOtRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> CompletarActividadAsync(CompletarActividadRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> RegistrarFirmaAsync(RegistrarFirmaOtRequest request, long usuarioId, CancellationToken ct = default);
}

public interface IReporteAvanceService
{
    Task<IEnumerable<ReporteAvanceDto>> GetByProyectoAsync(long proyectoId, CancellationToken ct = default);
    Task<ReporteAvanceDto?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<ReporteAvanceDto>> CreateAsync(CrearReporteAvanceRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> AgregarFotoAsync(AgregarFotoReporteRequest request, long usuarioId, CancellationToken ct = default);
    Task<Result> EliminarFotoAsync(long fotoId, long usuarioId, CancellationToken ct = default);
}

public interface IKpiService
{
    Task<IEnumerable<KpiProyectoDto>> GetKpisAsync(long proyectoId, CancellationToken ct = default);
    Task CalcularKpisAsync(long proyectoId, CancellationToken ct = default);
    Task<IEnumerable<AlertaProyectoDto>> GetAlertasAsync(long proyectoId, CancellationToken ct = default);
    Task<int> ContarNoLeidasAsync(long usuarioId, CancellationToken ct = default);
    Task<Result> MarcarLeidaAsync(long alertaId, CancellationToken ct = default);
    Task<Result> MarcarTodasLeidasAsync(long proyectoId, long usuarioId, CancellationToken ct = default);
}
