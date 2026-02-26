using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class EvaluacionService : IEvaluacionService
{
    private readonly SolarisDbContext _db;
    public EvaluacionService(SolarisDbContext db) { _db = db; }

    public async Task<ApiResponse<IEnumerable<EvaluacionProcesoDto>>> ObtenerProcesosAsync(long empresaId)
    { await Task.CompletedTask; return ApiResponse<IEnumerable<EvaluacionProcesoDto>>.Ok(Enumerable.Empty<EvaluacionProcesoDto>()); }

    public async Task<ApiResponse<EvaluacionProcesoDto>> CrearProcesoAsync(long empresaId, EvaluacionProcesoDto dto)
    { await Task.CompletedTask; return ApiResponse<EvaluacionProcesoDto>.Fail("Funcionalidad en desarrollo"); }

    public async Task<ApiResponse> LanzarProcesoAsync(long procesoId, long empresaId)
    { await Task.CompletedTask; return ApiResponse.Fail("Funcionalidad en desarrollo"); }

    public async Task<ApiResponse<IEnumerable<EvaluacionDto>>> ObtenerMisEvaluacionesAsync(long evaluadorId, long empresaId)
    { await Task.CompletedTask; return ApiResponse<IEnumerable<EvaluacionDto>>.Ok(Enumerable.Empty<EvaluacionDto>()); }

    public async Task<ApiResponse<EvaluacionDto>> ObtenerEvaluacionAsync(long id)
    { await Task.CompletedTask; return ApiResponse<EvaluacionDto>.Fail("No encontrado"); }

    public async Task<ApiResponse> CompletarEvaluacionAsync(long id, CompletarEvaluacionRequest request)
    { await Task.CompletedTask; return ApiResponse.Fail("Funcionalidad en desarrollo"); }

    public async Task<ApiResponse<IEnumerable<EvaluacionDto>>> ObtenerResultadosProcesoAsync(long procesoId)
    { await Task.CompletedTask; return ApiResponse<IEnumerable<EvaluacionDto>>.Ok(Enumerable.Empty<EvaluacionDto>()); }
}
