using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class CapacitacionService : ICapacitacionService
{
    private readonly SolarisDbContext _db;
    public CapacitacionService(SolarisDbContext db) { _db = db; }

    public async Task<ApiResponse<PaginatedList<CapacitacionDto>>> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, short? estado = null)
    { await Task.CompletedTask; return ApiResponse<PaginatedList<CapacitacionDto>>.Ok(PaginatedList<CapacitacionDto>.Empty(pagina, tamano)); }

    public async Task<ApiResponse<CapacitacionDetalleDto>> ObtenerDetalleAsync(long id)
    { await Task.CompletedTask; return ApiResponse<CapacitacionDetalleDto>.Fail("No encontrado"); }

    public async Task<ApiResponse<CapacitacionDto>> CrearAsync(long empresaId, CrearCapacitacionRequest request)
    { await Task.CompletedTask; return ApiResponse<CapacitacionDto>.Fail("Funcionalidad en desarrollo"); }

    public async Task<ApiResponse<CapacitacionDto>> ActualizarAsync(long id, CrearCapacitacionRequest request)
    { await Task.CompletedTask; return ApiResponse<CapacitacionDto>.Fail("Funcionalidad en desarrollo"); }

    public async Task<ApiResponse> InscribirEmpleadosAsync(long id, long empresaId, InscribirEmpleadosRequest request)
    { await Task.CompletedTask; return ApiResponse.Fail("Funcionalidad en desarrollo"); }

    public async Task<ApiResponse> ActualizarParticipanteAsync(long capacitacionId, long empleadoId, ActualizarParticipanteRequest request)
    { await Task.CompletedTask; return ApiResponse.Fail("Funcionalidad en desarrollo"); }
}
