using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class NominaService : INominaService
{
    private readonly SolarisDbContext _db;

    public NominaService(SolarisDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<IEnumerable<PeriodoNominaDto>>> ObtenerPeriodosAsync(
        long empresaId, int anno)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<PeriodoNominaDto>>.Ok(
            Enumerable.Empty<PeriodoNominaDto>(),
            "Periodos obtenidos");
    }

    public async Task<ApiResponse<PeriodoNominaDto>> CrearPeriodoAsync(
        long empresaId, CrearPeriodoNominaRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<PeriodoNominaDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<IEnumerable<RolPagoDto>>> ObtenerRolesPagoAsync(
        long empresaId, long? periodoId = null, int pagina = 1, int tamano = 20)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<RolPagoDto>>.Ok(
            Enumerable.Empty<RolPagoDto>(),
            "Roles de pago obtenidos");
    }

    public async Task<ApiResponse<RolPagoDto>> ObtenerRolPagoAsync(long id)
    {
        await Task.CompletedTask;
        return ApiResponse<RolPagoDto>.Fail("Rol de pago no encontrado");
    }

    public async Task<ApiResponse<RolPagoDto>> CrearRolPagoAsync(
        long empresaId, CrearRolPagoRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<RolPagoDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<RolPagoDto>> CalcularRolPagoAsync(long id, long empresaId)
    {
        await Task.CompletedTask;
        return ApiResponse<RolPagoDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<RolPagoDto>> AprobarRolPagoAsync(long id, long aprobadorId)
    {
        await Task.CompletedTask;
        return ApiResponse<RolPagoDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse> MarcarPagadoAsync(long id, long empresaId)
    {
        await Task.CompletedTask;
        return ApiResponse.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<IEnumerable<RolPagoEmpleadoDto>>> ObtenerDetalleRolAsync(
        long rolPagoId)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<RolPagoEmpleadoDto>>.Ok(
            Enumerable.Empty<RolPagoEmpleadoDto>(),
            "Detalle obtenido");
    }

    public async Task<ApiResponse<IEnumerable<ParametroNominaDto>>> ObtenerParametrosAsync(
        long empresaId, string paisCodigo, int anno)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<ParametroNominaDto>>.Ok(
            Enumerable.Empty<ParametroNominaDto>(),
            "Parámetros obtenidos");
    }

    public async Task<ApiResponse<ParametroNominaDto>> GuardarParametroAsync(
        long empresaId, string paisCodigo, int anno, GuardarParametroRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<ParametroNominaDto>.Fail("Funcionalidad en desarrollo");
    }
}
