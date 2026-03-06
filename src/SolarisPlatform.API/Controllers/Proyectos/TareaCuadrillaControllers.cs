using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;

namespace SolarisPlatform.Api.Controllers.Proyectos;

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — TAREAS
// GET    /api/proy/proyectos/{proyectoId}/tareas
// GET    /api/proy/proyectos/{proyectoId}/tareas/{id}
// POST   /api/proy/proyectos/{proyectoId}/tareas
// PUT    /api/proy/proyectos/{proyectoId}/tareas/{id}
// DELETE /api/proy/proyectos/{proyectoId}/tareas/{id}
// PATCH  /api/proy/proyectos/{proyectoId}/tareas/{id}/estado
// PATCH  /api/proy/proyectos/{proyectoId}/tareas/{id}/avance
// POST   /api/proy/proyectos/{proyectoId}/tareas/{id}/dependencias
// DELETE /api/proy/proyectos/{proyectoId}/tareas/{tareaId}/dependencias/{depId}
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/tareas")]
public class TareasController : BaseProyectosController
{
    private readonly ITareaService _service;
    public TareasController(ITareaService service) => _service = service;

    /// <summary>Lista todas las tareas del proyecto</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TareaListDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Lista tareas de una fase específica</summary>
    [HttpGet("por-fase/{faseId:long}")]
    public async Task<IActionResult> GetByFase(long proyectoId, long faseId, CancellationToken ct)
        => OkResult(await _service.GetByFaseAsync(faseId, ct));

    /// <summary>Obtiene el detalle de una tarea con sus dependencias</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<TareaDto>), 200)]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Tarea {id} no encontrada.") : OkResult(dto);
    }

    /// <summary>Crea una nueva tarea en el proyecto</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TareaDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearTareaRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<TareaDto>.Ok(result.Data));
    }

    /// <summary>Actualiza datos de una tarea</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long proyectoId, long id, [FromBody] ActualizarTareaRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina una tarea</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));

    /// <summary>Cambia el estado de la tarea (Pendiente → En Proceso → Completada, etc.)</summary>
    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long proyectoId, long id, [FromBody] CambiarEstadoTareaRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.CambiarEstadoAsync(request, UsuarioId, ct));
    }

    /// <summary>Registra avance de horas y porcentaje completado en una tarea</summary>
    [HttpPatch("{id:long}/avance")]
    public async Task<IActionResult> ActualizarAvance(long proyectoId, long id, [FromBody] ActualizarAvanceTareaRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.ActualizarAvanceAsync(request, UsuarioId, ct));
    }

    /// <summary>Agrega una dependencia entre dos tareas (FinInicio, InicioInicio, FinFin, InicioFin)</summary>
    [HttpPost("{id:long}/dependencias")]
    [ProducesResponseType(typeof(ApiResponse<TareaDependenciaDto>), 201)]
    public async Task<IActionResult> AgregarDependencia(long proyectoId, long id, [FromBody] CrearDependenciaTareaRequest request, CancellationToken ct)
    {
        request.TareaOrigenId = id;
        var result = await _service.AgregarDependenciaAsync(request, UsuarioId, ct);
        return FromResult(result);
    }

    /// <summary>Elimina una dependencia entre tareas</summary>
    [HttpDelete("{tareaId:long}/dependencias/{dependenciaId:long}")]
    public async Task<IActionResult> EliminarDependencia(long proyectoId, long tareaId, long dependenciaId, CancellationToken ct)
        => FromResult(await _service.EliminarDependenciaAsync(dependenciaId, UsuarioId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — CUADRILLAS
// GET    /api/proy/proyectos/{proyectoId}/cuadrillas
// GET    /api/proy/proyectos/{proyectoId}/cuadrillas/{id}
// POST   /api/proy/proyectos/{proyectoId}/cuadrillas
// PUT    /api/proy/proyectos/{proyectoId}/cuadrillas/{id}
// DELETE /api/proy/proyectos/{proyectoId}/cuadrillas/{id}
// POST   /api/proy/proyectos/{proyectoId}/cuadrillas/{id}/miembros
// DELETE /api/proy/proyectos/{proyectoId}/cuadrillas/{id}/miembros
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/cuadrillas")]
public class CuadrillasController : BaseProyectosController
{
    private readonly ICuadrillaService _service;
    public CuadrillasController(ICuadrillaService service) => _service = service;

    /// <summary>Lista todas las cuadrillas del proyecto con sus miembros activos</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CuadrillaDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Obtiene detalle de una cuadrilla con todos sus miembros</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<CuadrillaDto>), 200)]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Cuadrilla {id} no encontrada.") : OkResult(dto);
    }

    /// <summary>Crea una nueva cuadrilla de trabajo de campo</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CuadrillaDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearCuadrillaRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<CuadrillaDto>.Ok(result.Data));
    }

    /// <summary>Actualiza datos de una cuadrilla (nombre, líder, capacidad)</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long proyectoId, long id, [FromBody] ActualizarCuadrillaRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina una cuadrilla</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));

    /// <summary>Agrega un empleado como miembro de la cuadrilla (valida capacidad máxima)</summary>
    [HttpPost("{id:long}/miembros")]
    public async Task<IActionResult> AgregarMiembro(long proyectoId, long id, [FromBody] AgregarMiembroCuadrillaRequest request, CancellationToken ct)
    {
        request.CuadrillaId = id;
        return FromResult(await _service.AgregarMiembroAsync(request, UsuarioId, ct));
    }

    /// <summary>Remueve un empleado de la cuadrilla registrando su fecha de salida</summary>
    [HttpDelete("{id:long}/miembros")]
    public async Task<IActionResult> RemoverMiembro(long proyectoId, long id, [FromBody] RemoverMiembroCuadrillaRequest request, CancellationToken ct)
    {
        request.CuadrillaId = id;
        return FromResult(await _service.RemoverMiembroAsync(request, UsuarioId, ct));
    }
}
