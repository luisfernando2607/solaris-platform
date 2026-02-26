using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class AsistenciaService : IAsistenciaService
{
    private readonly SolarisDbContext _db;

    public AsistenciaService(SolarisDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<IEnumerable<AsistenciaDto>>> ObtenerPorEmpleadoMesAsync(
        long empleadoId, int anno, int mes)
    {
        // TODO: implementar consulta real a rrhh.AsistenciaEmpleado
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<AsistenciaDto>>.Ok(
            Enumerable.Empty<AsistenciaDto>(),
            "Asistencias obtenidas");
    }

    public async Task<ApiResponse<IEnumerable<AsistenciaDto>>> ObtenerPorEmpresaFechaAsync(
        long empresaId, DateOnly fecha)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<AsistenciaDto>>.Ok(
            Enumerable.Empty<AsistenciaDto>(),
            "Asistencias obtenidas");
    }

    public async Task<ApiResponse<AsistenciaDto>> MarcarAsync(
        long empresaId, MarcarAsistenciaRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<AsistenciaDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<AsistenciaDto>> CorregirAsync(
        long id, long empresaId, AsistenciaDto dto)
    {
        await Task.CompletedTask;
        return ApiResponse<AsistenciaDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<IEnumerable<SolicitudAusenciaDto>>> ObtenerAusenciasAsync(
        long empresaId, long? empleadoId = null, short? estado = null,
        int pagina = 1, int tamano = 20)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<SolicitudAusenciaDto>>.Ok(
            Enumerable.Empty<SolicitudAusenciaDto>(),
            "Ausencias obtenidas");
    }

    public async Task<ApiResponse<SolicitudAusenciaDto>> SolicitarAusenciaAsync(
        long empresaId, CrearSolicitudAusenciaRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<SolicitudAusenciaDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse> AprobarRechazarAusenciaAsync(
        long id, long aprobadorId, AprobarRechazarAusenciaRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<SaldoVacacionesDto>> ObtenerSaldoVacacionesAsync(
        long empleadoId, int anno)
    {
        await Task.CompletedTask;
        return ApiResponse<SaldoVacacionesDto>.Fail("Funcionalidad en desarrollo");
    }
}
