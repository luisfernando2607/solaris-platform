using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;

namespace SolarisPlatform.Api.Controllers.Proyectos;

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — PRESUPUESTO
// GET    /api/proy/proyectos/{proyectoId}/presupuesto
// GET    /api/proy/proyectos/{proyectoId}/presupuesto/activo
// GET    /api/proy/proyectos/{proyectoId}/presupuesto/{id}
// GET    /api/proy/proyectos/{proyectoId}/presupuesto/ejecucion
// POST   /api/proy/proyectos/{proyectoId}/presupuesto
// POST   /api/proy/proyectos/{proyectoId}/presupuesto/{id}/partidas
// PATCH  /api/proy/proyectos/{proyectoId}/presupuesto/{id}/aprobar
// POST   /api/proy/proyectos/{proyectoId}/presupuesto/costos
// GET    /api/proy/proyectos/{proyectoId}/presupuesto/costos
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/presupuesto")]
public class PresupuestoController : BaseProyectosController
{
    private readonly IPresupuestoService _service;
    public PresupuestoController(IPresupuestoService service) => _service = service;

    /// <summary>Lista todas las versiones de presupuesto del proyecto</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PresupuestoDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Obtiene el presupuesto activo con sus partidas y costos ejecutados</summary>
    [HttpGet("activo")]
    [ProducesResponseType(typeof(ApiResponse<PresupuestoDto>), 200)]
    public async Task<IActionResult> GetActivo(long proyectoId, CancellationToken ct)
    {
        var dto = await _service.GetActivoAsync(proyectoId, ct);
        return dto is null ? NotFoundResult("No hay presupuesto activo para este proyecto.") : OkResult(dto);
    }

    /// <summary>Obtiene una versión de presupuesto específica con partidas y costos</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<PresupuestoDto>), 200)]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Presupuesto {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>Resumen de ejecución presupuestaria: presupuestado vs ejecutado por partida</summary>
    [HttpGet("ejecucion")]
    [ProducesResponseType(typeof(ApiResponse<ResumenEjecucionDto>), 200)]
    public async Task<IActionResult> GetResumenEjecucion(long proyectoId, CancellationToken ct)
    {
        var dto = await _service.GetResumenEjecucionAsync(proyectoId, ct);
        return dto is null ? NotFoundResult("No hay presupuesto activo.") : OkResult(dto);
    }

    /// <summary>Crea una nueva versión de presupuesto (desactiva la anterior automáticamente)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PresupuestoDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearPresupuestoRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<PresupuestoDto>.Ok(result.Data));
    }

    /// <summary>Agrega una partida presupuestaria (ingreso, egreso o contingencia)</summary>
    [HttpPost("{id:long}/partidas")]
    [ProducesResponseType(typeof(ApiResponse<PresupuestoPartidaDto>), 201)]
    public async Task<IActionResult> AgregarPartida(long proyectoId, long id, [FromBody] AgregarPartidaRequest request, CancellationToken ct)
    {
        request.PresupuestoId = id;
        return FromResult(await _service.AgregarPartidaAsync(request, UsuarioId, ct));
    }

    /// <summary>Aprueba el presupuesto (bloquea edición de partidas)</summary>
    [HttpPatch("{id:long}/aprobar")]
    public async Task<IActionResult> Aprobar(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.AprobarAsync(id, UsuarioId, ct));

    /// <summary>Registra un costo real contra una partida presupuestaria</summary>
    [HttpPost("costos")]
    [ProducesResponseType(typeof(ApiResponse<CostoRealDto>), 201)]
    public async Task<IActionResult> RegistrarCosto(long proyectoId, [FromBody] RegistrarCostoRealRequest request, CancellationToken ct)
        => FromResult(await _service.RegistrarCostoRealAsync(request, UsuarioId, ct));

    /// <summary>Lista todos los costos reales del proyecto</summary>
    [HttpGet("costos")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CostoRealDto>>), 200)]
    public async Task<IActionResult> GetCostosReales(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetCostosRealesAsync(proyectoId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — GANTT
// GET    /api/proy/proyectos/{proyectoId}/gantt
// POST   /api/proy/proyectos/{proyectoId}/gantt/linea-base
// POST   /api/proy/proyectos/{proyectoId}/gantt/progreso
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/gantt")]
public class GanttController : BaseProyectosController
{
    private readonly IGanttService _service;
    public GanttController(IGanttService service) => _service = service;

    /// <summary>
    /// Obtiene datos completos del Gantt: fases, tareas, fechas plan/real/base, 
    /// avances y dependencias — listo para renderizar en la vista de Gantt
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GanttDto>), 200)]
    public async Task<IActionResult> GetGantt(long proyectoId, CancellationToken ct)
    {
        var dto = await _service.GetGanttAsync(proyectoId, ct);
        return dto is null ? NotFoundResult("Proyecto no encontrado.") : OkResult(dto);
    }

    /// <summary>
    /// Captura la línea base del Gantt para todas las tareas actuales
    /// (snapshot de fechas planificadas — sirve para comparar contra real)
    /// </summary>
    [HttpPost("linea-base")]
    public async Task<IActionResult> CapturarLineaBase(long proyectoId, CancellationToken ct)
        => FromResult(await _service.CapturarLineaBaseAsync(proyectoId, UsuarioId, ct));

    /// <summary>Registra el progreso diario de una tarea (% avance + horas trabajadas)</summary>
    [HttpPost("progreso")]
    public async Task<IActionResult> RegistrarProgreso(long proyectoId, [FromBody] RegistrarProgresoGanttRequest request, CancellationToken ct)
        => FromResult(await _service.RegistrarProgresoAsync(request, UsuarioId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — CENTROS DE COSTO
// GET    /api/proy/proyectos/{proyectoId}/centros-costo
// GET    /api/proy/proyectos/{proyectoId}/centros-costo/{id}
// POST   /api/proy/proyectos/{proyectoId}/centros-costo
// PUT    /api/proy/proyectos/{proyectoId}/centros-costo/{id}
// DELETE /api/proy/proyectos/{proyectoId}/centros-costo/{id}
// POST   /api/proy/proyectos/{proyectoId}/centros-costo/{id}/asignaciones
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/centros-costo")]
public class CentrosCostoController : BaseProyectosController
{
    private readonly ICentroCostoService _service;
    public CentrosCostoController(ICentroCostoService service) => _service = service;

    /// <summary>Lista los centros de costo del proyecto con su ejecución actual</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CentroCostoDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Obtiene un centro de costo específico</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<CentroCostoDto>), 200)]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Centro de costo {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>Crea un nuevo centro de costo con presupuesto asignado</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CentroCostoDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearCentroCostoRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<CentroCostoDto>.Ok(result.Data));
    }

    /// <summary>Actualiza nombre y presupuesto de un centro de costo</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long proyectoId, long id, [FromBody] ActualizarCentroCostoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina un centro de costo</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));

    /// <summary>Asigna un costo real o una OT a este centro de costo (con porcentaje de prorrateo)</summary>
    [HttpPost("{id:long}/asignaciones")]
    public async Task<IActionResult> AsignarCosto(long proyectoId, long id, [FromBody] AsignarCostoACentroRequest request, CancellationToken ct)
    {
        request.CentroCostoId = id;
        return FromResult(await _service.AsignarCostoAsync(request, UsuarioId, ct));
    }
}
