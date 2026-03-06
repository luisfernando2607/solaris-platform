using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;
using System.Security.Claims;

namespace SolarisPlatform.Api.Controllers.Proyectos;

// ══════════════════════════════════════════════════════════════════
// BASE CONTROLLER — MÓDULO PROYECTOS
// ══════════════════════════════════════════════════════════════════

[ApiController]
[Authorize]
[Route("api/proy/[controller]")]
[Produces("application/json")]
public abstract class BaseProyectosController : ControllerBase
{
    protected long UsuarioId =>
        long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    protected long EmpresaId =>
        long.TryParse(User.FindFirstValue("empresaId"), out var id) ? id : 0;

    protected IActionResult OkResult<T>(T data, string? mensaje = null) =>
        Ok(ApiResponse<T>.Ok(data, mensaje));

    protected IActionResult OkResult(string mensaje) =>
        Ok(ApiResponse.Ok(mensaje));

    protected IActionResult NotFoundResult(string mensaje) =>
        NotFound(ApiResponse.Fail(mensaje));

    protected IActionResult BadRequestResult(string mensaje) =>
        BadRequest(ApiResponse.Fail(mensaje));

    protected IActionResult FromResult<T>(Result<T> result) =>
        result.Succeeded
            ? Ok(ApiResponse<T>.Ok(result.Data!))
            : BadRequest(ApiResponse.Fail(result.Error!));

    protected IActionResult FromResult(Result result) =>
        result.Succeeded
            ? Ok(ApiResponse.Ok("Operación completada exitosamente."))
            : BadRequest(ApiResponse.Fail(result.Error!));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — PROYECTOS
// GET    /api/proy/proyectos                  → Listado paginado
// GET    /api/proy/proyectos/{id}             → Detalle
// GET    /api/proy/proyectos/{id}/dashboard   → Dashboard
// POST   /api/proy/proyectos                  → Crear
// PUT    /api/proy/proyectos/{id}             → Actualizar
// DELETE /api/proy/proyectos/{id}             → Eliminar
// PATCH  /api/proy/proyectos/{id}/estado      → Cambiar estado
// PATCH  /api/proy/proyectos/{id}/avance      → Actualizar avance
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos")]
public class ProyectosController : BaseProyectosController
{
    private readonly IProyectoService _service;

    public ProyectosController(IProyectoService service) => _service = service;

    /// <summary>Obtiene listado paginado de proyectos con filtros</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> GetList([FromQuery] FiltroProyectosRequest filtro, CancellationToken ct)
    {
        filtro.EmpresaId = EmpresaId;
        var (items, total) = await _service.GetListAsync(filtro, ct);
        return OkResult(new { Items = items, Total = total, Pagina = filtro.Pagina, PorPagina = filtro.ElementosPorPagina });
    }

    /// <summary>Obtiene detalle completo de un proyecto</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ProyectoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Proyecto {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>Obtiene el dashboard resumido de un proyecto (KPIs, alertas, hitos próximos)</summary>
    [HttpGet("{id:long}/dashboard")]
    [ProducesResponseType(typeof(ApiResponse<ProyectoDashboardDto>), 200)]
    public async Task<IActionResult> GetDashboard(long id, CancellationToken ct)
    {
        var dto = await _service.GetDashboardAsync(id, ct);
        return dto is null ? NotFoundResult($"Proyecto {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>Crea un nuevo proyecto</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProyectoDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CrearProyectoRequest request, CancellationToken ct)
    {
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id },
            ApiResponse<ProyectoDto>.Ok(result.Data));
    }

    /// <summary>Actualiza datos generales de un proyecto</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ProyectoDto>), 200)]
    public async Task<IActionResult> Update(long id, [FromBody] ActualizarProyectoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina (soft delete) un proyecto</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));

    /// <summary>Cambia el estado del proyecto (Borrador → Planificación → Activo → etc.)</summary>
    [HttpPatch("{id:long}/estado")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoProyectoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.CambiarEstadoAsync(request, UsuarioId, ct));
    }

    /// <summary>Actualiza el porcentaje de avance real del proyecto</summary>
    [HttpPatch("{id:long}/avance")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ActualizarAvance(long id, [FromBody] ActualizarAvanceProyectoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.ActualizarAvanceAsync(request, UsuarioId, ct));
    }
}
