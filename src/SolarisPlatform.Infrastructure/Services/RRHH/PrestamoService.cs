using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class PrestamoService : IPrestamoService
{
    private readonly SolarisDbContext _db;

    public PrestamoService(SolarisDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<PaginatedList<PrestamoDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        long? empleadoId = null, short? estado = null)
    {
        await Task.CompletedTask;
        return ApiResponse<PaginatedList<PrestamoDto>>.Ok(
            PaginatedList<PrestamoDto>.Empty(pagina, tamano),
            "Préstamos obtenidos");
    }

    public async Task<ApiResponse<PrestamoDto>> ObtenerPorIdAsync(long id)
    {
        await Task.CompletedTask;
        return ApiResponse<PrestamoDto>.Fail("Préstamo no encontrado");
    }

    public async Task<ApiResponse<PrestamoDto>> CrearAsync(
        long empresaId, CrearPrestamoRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<PrestamoDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<PrestamoDto>> AprobarAsync(
        long id, long aprobadorId, AprobarPrestamoRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<PrestamoDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse> RechazarAsync(long id, string? motivo)
    {
        await Task.CompletedTask;
        return ApiResponse.Fail("Funcionalidad en desarrollo");
    }
}
