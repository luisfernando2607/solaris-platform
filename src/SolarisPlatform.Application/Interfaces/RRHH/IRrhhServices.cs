using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;

namespace SolarisPlatform.Application.Interfaces.RRHH;

public interface IDepartamentoService
{
    Task<ApiResponse<IEnumerable<DepartamentoDto>>> ObtenerArbolAsync(long empresaId);
    Task<ApiResponse<DepartamentoDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<DepartamentoDto>> CrearAsync(long empresaId, CrearDepartamentoRequest request);
    Task<ApiResponse<DepartamentoDto>> ActualizarAsync(long id, ActualizarDepartamentoRequest request);
    Task<ApiResponse> EliminarAsync(long id);
}

public interface IPuestoService
{
    Task<ApiResponse<IEnumerable<PuestoDto>>> ObtenerAsync(long empresaId, long? departamentoId = null);
    Task<ApiResponse<PuestoDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<PuestoDto>> CrearAsync(long empresaId, CrearPuestoRequest request);
    Task<ApiResponse<PuestoDto>> ActualizarAsync(long id, ActualizarPuestoRequest request);
    Task<ApiResponse> EliminarAsync(long id);
}

public interface IEmpleadoService
{
    Task<ApiResponse<PaginatedList<EmpleadoListaDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        string? busqueda = null, long? departamentoId = null,
        long? puestoId = null, short? estado = null);
    Task<ApiResponse<EmpleadoFichaDto>> ObtenerFichaAsync(long id);
    Task<ApiResponse<EmpleadoFichaDto>> CrearAsync(long empresaId, CrearEmpleadoRequest request);
    Task<ApiResponse<EmpleadoFichaDto>> ActualizarAsync(long id, ActualizarEmpleadoRequest request);
    Task<ApiResponse> EgresarAsync(long id, EgresarEmpleadoRequest request);
    Task<ApiResponse> CambiarSalarioAsync(long id, CambiarSalarioRequest request);
    Task<ApiResponse> CambiarPuestoAsync(long id, CambiarPuestoRequest request);
    Task<ApiResponse<IEnumerable<EmpleadoHistorialDto>>> ObtenerHistorialAsync(long id);
    Task<ApiResponse<IEnumerable<EmpleadoDocumentoDto>>> ObtenerDocumentosAsync(long id);
    Task<ApiResponse<EmpleadoDocumentoDto>> AgregarDocumentoAsync(long id, AgregarDocumentoRequest request);
    Task<ApiResponse> EliminarDocumentoAsync(long documentoId);
}

public interface IAsistenciaService
{
    Task<ApiResponse<IEnumerable<AsistenciaDto>>> ObtenerPorEmpleadoMesAsync(long empleadoId, int anno, int mes);
    Task<ApiResponse<IEnumerable<AsistenciaDto>>> ObtenerPorEmpresaFechaAsync(long empresaId, DateOnly fecha);
    Task<ApiResponse<AsistenciaDto>> MarcarAsync(long empresaId, MarcarAsistenciaRequest request);
    Task<ApiResponse<AsistenciaDto>> CorregirAsync(long id, long empresaId, AsistenciaDto dto);
    Task<ApiResponse<IEnumerable<SolicitudAusenciaDto>>> ObtenerAusenciasAsync(
        long empresaId, long? empleadoId = null, short? estado = null, int pagina = 1, int tamano = 20);
    Task<ApiResponse<SolicitudAusenciaDto>> SolicitarAusenciaAsync(long empresaId, CrearSolicitudAusenciaRequest request);
    Task<ApiResponse> AprobarRechazarAusenciaAsync(long id, long aprobadorId, AprobarRechazarAusenciaRequest request);
    Task<ApiResponse<SaldoVacacionesDto>> ObtenerSaldoVacacionesAsync(long empleadoId, int anno);
}

public interface IConceptoNominaService
{
    Task<ApiResponse<IEnumerable<ConceptoNominaDto>>> ObtenerAsync(long empresaId);
    Task<ApiResponse<ConceptoNominaDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<ConceptoNominaDto>> CrearAsync(long empresaId, CrearConceptoNominaRequest request);
    Task<ApiResponse<ConceptoNominaDto>> ActualizarAsync(long id, CrearConceptoNominaRequest request);
    Task<ApiResponse> EliminarAsync(long id);
}

public interface INominaService
{
    Task<ApiResponse<IEnumerable<PeriodoNominaDto>>> ObtenerPeriodosAsync(long empresaId, int anno);
    Task<ApiResponse<PeriodoNominaDto>> CrearPeriodoAsync(long empresaId, CrearPeriodoNominaRequest request);
    Task<ApiResponse<IEnumerable<RolPagoDto>>> ObtenerRolesPagoAsync(long empresaId, long? periodoId = null, int pagina = 1, int tamano = 20);
    Task<ApiResponse<RolPagoDto>> ObtenerRolPagoAsync(long id);
    Task<ApiResponse<RolPagoDto>> CrearRolPagoAsync(long empresaId, CrearRolPagoRequest request);
    Task<ApiResponse<RolPagoDto>> CalcularRolPagoAsync(long id, long empresaId);
    Task<ApiResponse<RolPagoDto>> AprobarRolPagoAsync(long id, long aprobadorId);
    Task<ApiResponse> MarcarPagadoAsync(long id, long empresaId);
    Task<ApiResponse<IEnumerable<RolPagoEmpleadoDto>>> ObtenerDetalleRolAsync(long rolPagoId);
    Task<ApiResponse<IEnumerable<ParametroNominaDto>>> ObtenerParametrosAsync(long empresaId, string paisCodigo, int anno);
    Task<ApiResponse<ParametroNominaDto>> GuardarParametroAsync(long empresaId, string paisCodigo, int anno, GuardarParametroRequest request);
}

public interface IPrestamoService
{
    Task<ApiResponse<PaginatedList<PrestamoDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? empleadoId = null, short? estado = null);
    Task<ApiResponse<PrestamoDto>> ObtenerPorIdAsync(long id);
    Task<ApiResponse<PrestamoDto>> CrearAsync(long empresaId, CrearPrestamoRequest request);
    Task<ApiResponse<PrestamoDto>> AprobarAsync(long id, long aprobadorId, AprobarPrestamoRequest request);
    Task<ApiResponse> RechazarAsync(long id, string? motivo);
}

public interface IEvaluacionService
{
    Task<ApiResponse<IEnumerable<EvaluacionProcesoDto>>> ObtenerProcesosAsync(long empresaId);
    Task<ApiResponse<EvaluacionProcesoDto>> CrearProcesoAsync(long empresaId, EvaluacionProcesoDto dto);
    Task<ApiResponse> LanzarProcesoAsync(long procesoId, long empresaId);
    Task<ApiResponse<IEnumerable<EvaluacionDto>>> ObtenerMisEvaluacionesAsync(long evaluadorId, long empresaId);
    Task<ApiResponse<EvaluacionDto>> ObtenerEvaluacionAsync(long id);
    Task<ApiResponse> CompletarEvaluacionAsync(long id, CompletarEvaluacionRequest request);
    Task<ApiResponse<IEnumerable<EvaluacionDto>>> ObtenerResultadosProcesoAsync(long procesoId);
}

public interface ICapacitacionService
{
    Task<ApiResponse<PaginatedList<CapacitacionDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, short? estado = null);
    Task<ApiResponse<CapacitacionDetalleDto>> ObtenerDetalleAsync(long id);
    Task<ApiResponse<CapacitacionDto>> CrearAsync(long empresaId, CrearCapacitacionRequest request);
    Task<ApiResponse<CapacitacionDto>> ActualizarAsync(long id, CrearCapacitacionRequest request);
    Task<ApiResponse> InscribirEmpleadosAsync(long id, long empresaId, InscribirEmpleadosRequest request);
    Task<ApiResponse> ActualizarParticipanteAsync(long capacitacionId, long empleadoId, ActualizarParticipanteRequest request);
}

public interface IRrhhDashboardService
{
    Task<ApiResponse<RrhhDashboardDto>> ObtenerDashboardAsync(long empresaId);
}
