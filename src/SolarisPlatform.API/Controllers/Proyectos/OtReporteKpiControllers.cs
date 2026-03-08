using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;

namespace SolarisPlatform.Api.Controllers.Proyectos;

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — ÓRDENES DE TRABAJO
// GET    /api/proy/ordenes-trabajo                     → Paginado global
// GET    /api/proy/ordenes-trabajo/{id}                → Detalle con actividades
// POST   /api/proy/ordenes-trabajo                     → Crear con actividades/materiales
// PUT    /api/proy/ordenes-trabajo/{id}                → Actualizar
// DELETE /api/proy/ordenes-trabajo/{id}                → Eliminar (solo Borrador)
// PATCH  /api/proy/ordenes-trabajo/{id}/estado         → Cambiar estado
// PATCH  /api/proy/ordenes-trabajo/{id}/actividades    → Completar actividad
// PATCH  /api/proy/ordenes-trabajo/{id}/firma          → Registrar firma digital
// GET    /api/proy/proyectos/{proyectoId}/ordenes-trabajo → Por proyecto
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/ordenes-trabajo")]
public class OrdenesTrabajoController : BaseProyectosController
{
    private readonly IOrdenTrabajoService _service;
    public OrdenesTrabajoController(IOrdenTrabajoService service) => _service = service;

    /// <summary>Listado paginado de órdenes de trabajo con filtros (estado, proyecto, cuadrilla)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> GetList([FromQuery] FiltroOrdenesTrabajoRequest filtro, CancellationToken ct)
    {
        filtro.EmpresaId = EmpresaId;
        var (items, total) = await _service.GetListAsync(filtro, ct);
        return OkResult(new { Items = items, Total = total, Pagina = filtro.Pagina, PorPagina = filtro.ElementosPorPagina });
    }

    /// <summary>Detalle completo de una OT con actividades checklist y materiales</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<OrdenTrabajoDto>), 200)]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Orden de trabajo {id} no encontrada.") : OkResult(dto);
    }

    /// <summary>
    /// Crea una OT con actividades checklist y materiales en una sola transacción.
    /// Genera número correlativo automático (OT-YYYY-NNNNNN).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrdenTrabajoDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CrearOrdenTrabajoRequest request, CancellationToken ct)
    {
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id },
            ApiResponse<OrdenTrabajoDto>.Ok(result.Data));
    }

    /// <summary>Actualiza datos generales de la OT (no estado ni actividades)</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ActualizarOrdenTrabajoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina una OT (solo si está en estado Borrador)</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));

    /// <summary>
    /// Cambia el estado de la OT siguiendo la máquina de estados:
    /// Borrador → Asignada → En Curso → Completada/Rechazada
    /// En Curso → Pausada → En Curso
    /// </summary>
    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoOtRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.CambiarEstadoAsync(request, UsuarioId, ct));
    }

    /// <summary>Marca una actividad del checklist como completada</summary>
    [HttpPatch("{id:long}/actividades")]
    public async Task<IActionResult> CompletarActividad(long id, [FromBody] CompletarActividadRequest request, CancellationToken ct)
    {
        request.OtId = id;
        return FromResult(await _service.CompletarActividadAsync(request, UsuarioId, ct));
    }

    /// <summary>Registra la firma digital del cliente/supervisor (base64). Requiere RequiereFirma = true.</summary>
    [HttpPatch("{id:long}/firma")]
    public async Task<IActionResult> RegistrarFirma(long id, [FromBody] RegistrarFirmaOtRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.RegistrarFirmaAsync(request, UsuarioId, ct));
    }
}

/// <summary>Endpoint adicional: OTs por proyecto (nested route)</summary>
[Route("api/proy/proyectos/{proyectoId:long}/ordenes-trabajo")]
public class OrdenesPorProyectoController : BaseProyectosController
{
    private readonly IOrdenTrabajoService _service;
    public OrdenesPorProyectoController(IOrdenTrabajoService service) => _service = service;

    /// <summary>Lista todas las OTs de un proyecto específico</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrdenTrabajoListDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — REPORTES DE AVANCE
// GET    /api/proy/proyectos/{proyectoId}/reportes
// GET    /api/proy/proyectos/{proyectoId}/reportes/{id}
// POST   /api/proy/proyectos/{proyectoId}/reportes
// POST   /api/proy/proyectos/{proyectoId}/reportes/{id}/fotos
// DELETE /api/proy/proyectos/{proyectoId}/reportes/{id}/fotos/{fotoId}
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/reportes")]
public class ReportesAvanceController : BaseProyectosController
{
    private readonly IReporteAvanceService _service;
    public ReportesAvanceController(IReporteAvanceService service) => _service = service;

    /// <summary>Lista todos los reportes de avance del proyecto ordenados por fecha</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReporteAvanceDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Obtiene un reporte con sus fotos de evidencia</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ReporteAvanceDto>), 200)]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Reporte {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>
    /// Crea un reporte de avance. Calcula automáticamente métricas EVM (CPI, SPI)
    /// y actualiza el porcentaje de avance real del proyecto.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReporteAvanceDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearReporteAvanceRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<ReporteAvanceDto>.Ok(result.Data));
    }

    /// <summary>Agrega una foto de evidencia a un reporte (URL ya subida a storage)</summary>
    [HttpPost("{id:long}/fotos")]
    public async Task<IActionResult> AgregarFoto(long proyectoId, long id, [FromBody] AgregarFotoReporteRequest request, CancellationToken ct)
    {
        request.ReporteId = id;
        return FromResult(await _service.AgregarFotoAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina una foto de evidencia</summary>
    [HttpDelete("{id:long}/fotos/{fotoId:long}")]
    public async Task<IActionResult> EliminarFoto(long proyectoId, long id, long fotoId, CancellationToken ct)
        => FromResult(await _service.EliminarFotoAsync(fotoId, UsuarioId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — KPIs Y ALERTAS
// GET    /api/proy/proyectos/{proyectoId}/kpis
// POST   /api/proy/proyectos/{proyectoId}/kpis/calcular
// GET    /api/proy/proyectos/{proyectoId}/alertas
// PATCH  /api/proy/proyectos/{proyectoId}/alertas/{id}/leida
// PATCH  /api/proy/proyectos/{proyectoId}/alertas/marcar-todas-leidas
// GET    /api/proy/alertas/no-leidas-count
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/kpis")]
public class KpisController : BaseProyectosController
{
    private readonly IKpiService _service;
    public KpisController(IKpiService service) => _service = service;

    /// <summary>Lista el historial de KPIs del proyecto (CPI, SPI, CV, SV, EAC, VAC)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<KpiProyectoDto>>), 200)]
    public async Task<IActionResult> GetKpis(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetKpisAsync(proyectoId, ct));

    /// <summary>
    /// Calcula y persiste los KPIs EVM del proyecto en este momento.
    /// Genera alertas automáticas si CPI &lt; 0.8 o SPI &lt; 0.8.
    /// </summary>
    [HttpPost("calcular")]
    public async Task<IActionResult> Calcular(long proyectoId, CancellationToken ct)
    {
        await _service.CalcularKpisAsync(proyectoId, ct);
        return OkResult("KPIs calculados y alertas generadas exitosamente.");
    }
}

[Route("api/proy/proyectos/{proyectoId:long}/alertas")]
public class AlertasController : BaseProyectosController
{
    private readonly IKpiService _service;
    public AlertasController(IKpiService service) => _service = service;

    /// <summary>Lista todas las alertas del proyecto (leídas y no leídas)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AlertaProyectoDto>>), 200)]
    public async Task<IActionResult> GetAlertas(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetAlertasAsync(proyectoId, ct));

    /// <summary>Marca una alerta específica como leída</summary>
    [HttpPatch("{id:long}/leida")]
    public async Task<IActionResult> MarcarLeida(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.MarcarLeidaAsync(id, ct));

    /// <summary>Marca todas las alertas del proyecto como leídas</summary>
    [HttpPatch("marcar-todas-leidas")]
    public async Task<IActionResult> MarcarTodasLeidas(long proyectoId, CancellationToken ct)
        => FromResult(await _service.MarcarTodasLeidasAsync(proyectoId, UsuarioId, ct));
}

/// <summary>Endpoint global para el badge de notificaciones en la UI</summary>
[Route("api/proy/alertas")]
public class AlertasGlobalController : BaseProyectosController
{
    private readonly IKpiService _service;
    public AlertasGlobalController(IKpiService service) => _service = service;

    /// <summary>Cuenta las alertas no leídas del usuario actual (para badge en sidebar)</summary>
    [HttpGet("no-leidas-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<IActionResult> ContarNoLeidas(CancellationToken ct)
        => OkResult(await _service.ContarNoLeidasAsync(UsuarioId, ct));
}
