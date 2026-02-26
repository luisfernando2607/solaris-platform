using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace SolarisPlatform.Infrastructure.Services.RRHH;

public class RrhhDashboardService : IRrhhDashboardService
{
    private readonly SolarisDbContext _db;
    public RrhhDashboardService(SolarisDbContext db) { _db = db; }

    public async Task<ApiResponse<RrhhDashboardDto>> ObtenerDashboardAsync(long empresaId)
    {
        await Task.CompletedTask;
        // TODO: implementar consultas reales al schema rrhh
        return ApiResponse<RrhhDashboardDto>.Fail("Funcionalidad en desarrollo");
    }
}
