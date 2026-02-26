using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class ConceptoNominaService : IConceptoNominaService
{
    private readonly SolarisDbContext _db;

    public ConceptoNominaService(SolarisDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse<IEnumerable<ConceptoNominaDto>>> ObtenerAsync(long empresaId)
    {
        await Task.CompletedTask;
        return ApiResponse<IEnumerable<ConceptoNominaDto>>.Ok(
            Enumerable.Empty<ConceptoNominaDto>(),
            "Conceptos obtenidos");
    }

    public async Task<ApiResponse<ConceptoNominaDto>> ObtenerPorIdAsync(long id)
    {
        await Task.CompletedTask;
        return ApiResponse<ConceptoNominaDto>.Fail("Concepto no encontrado");
    }

    public async Task<ApiResponse<ConceptoNominaDto>> CrearAsync(
        long empresaId, CrearConceptoNominaRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<ConceptoNominaDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse<ConceptoNominaDto>> ActualizarAsync(
        long id, CrearConceptoNominaRequest request)
    {
        await Task.CompletedTask;
        return ApiResponse<ConceptoNominaDto>.Fail("Funcionalidad en desarrollo");
    }

    public async Task<ApiResponse> EliminarAsync(long id)
    {
        await Task.CompletedTask;
        return ApiResponse.Fail("Funcionalidad en desarrollo");
    }
}
