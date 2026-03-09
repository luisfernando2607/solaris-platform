using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces.Proyectos;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Proyectos;

namespace SolarisPlatform.Api.Controllers.Proyectos;

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — FASES
// GET    /api/proy/proyectos/{proyectoId}/fases
// GET    /api/proy/proyectos/{proyectoId}/fases/{id}
// POST   /api/proy/proyectos/{proyectoId}/fases
// PUT    /api/proy/proyectos/{proyectoId}/fases/{id}
// DELETE /api/proy/proyectos/{proyectoId}/fases/{id}
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/fases")]
public class FasesController : BaseProyectosController
{
    private readonly IProyectoFaseService _service;
    public FasesController(IProyectoFaseService service) => _service = service;

    /// <summary>Lista todas las fases de un proyecto ordenadas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProyectoFaseDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Obtiene una fase específica</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ProyectoFaseDto>), 200)]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Fase {id} no encontrada.") : OkResult(dto);
    }

    /// <summary>Crea una nueva fase en el proyecto</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProyectoFaseDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearProyectoFaseRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<ProyectoFaseDto>.Ok(result.Data));
    }

    /// <summary>Actualiza una fase (nombre, fechas, estado, avance)</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long proyectoId, long id, [FromBody] ActualizarProyectoFaseRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina (soft delete) una fase</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — HITOS
// GET    /api/proy/proyectos/{proyectoId}/hitos
// GET    /api/proy/proyectos/{proyectoId}/hitos/{id}
// POST   /api/proy/proyectos/{proyectoId}/hitos
// PUT    /api/proy/proyectos/{proyectoId}/hitos/{id}
// DELETE /api/proy/proyectos/{proyectoId}/hitos/{id}
// PATCH  /api/proy/proyectos/{proyectoId}/hitos/{id}/logrado
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/hitos")]
public class HitosController : BaseProyectosController
{
    private readonly IProyectoHitoService _service;
    public HitosController(IProyectoHitoService service) => _service = service;

    /// <summary>Lista todos los hitos del proyecto ordenados por fecha compromiso</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProyectoHitoDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Obtiene un hito específico</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Hito {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>Crea un nuevo hito</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProyectoHitoDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearProyectoHitoRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<ProyectoHitoDto>.Ok(result.Data));
    }

    /// <summary>Actualiza un hito</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long proyectoId, long id, [FromBody] ActualizarProyectoHitoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina un hito</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));

    /// <summary>Marca un hito como logrado registrando la fecha real</summary>
    [HttpPatch("{id:long}/logrado")]
    public async Task<IActionResult> MarcarLogrado(long proyectoId, long id, [FromBody] MarcarHitoLogradoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.MarcarLogradoAsync(request, UsuarioId, ct));
    }
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — DOCUMENTOS
// GET    /api/proy/proyectos/{proyectoId}/documentos
// POST   /api/proy/proyectos/{proyectoId}/documentos
// DELETE /api/proy/proyectos/{proyectoId}/documentos/{id}
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/documentos")]
public class DocumentosController : BaseProyectosController
{
    private readonly IProyectoDocumentoService _service;
    public DocumentosController(IProyectoDocumentoService service) => _service = service;

    /// <summary>Lista todos los documentos adjuntos del proyecto</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProyectoDocumentoDto>>), 200)]
    public async Task<IActionResult> GetByProyecto(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetByProyectoAsync(proyectoId, ct));

    /// <summary>Registra un documento (URL ya subida a storage)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProyectoDocumentoDto>), 201)]
    public async Task<IActionResult> Create(long proyectoId, [FromBody] CrearProyectoDocumentoRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateAsync(request, UsuarioId, ct);
        return FromResult(result);
    }

    /// <summary>Elimina el registro de un documento</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, UsuarioId, ct));
}

// ══════════════════════════════════════════════════════════════════
// CONTROLLER — WBS
// GET    /api/proy/proyectos/{proyectoId}/wbs          → Árbol completo
// GET    /api/proy/proyectos/{proyectoId}/wbs/{id}     → Nodo con hijos
// POST   /api/proy/proyectos/{proyectoId}/wbs          → Crear nodo
// PUT    /api/proy/proyectos/{proyectoId}/wbs/{id}     → Actualizar nodo
// DELETE /api/proy/proyectos/{proyectoId}/wbs/{id}     → Eliminar nodo
// ══════════════════════════════════════════════════════════════════

[Route("api/proy/proyectos/{proyectoId:long}/wbs")]
public class WbsController : BaseProyectosController
{
    private readonly IWbsService _service;
    public WbsController(IWbsService service) => _service = service;

    /// <summary>Obtiene el árbol WBS completo del proyecto (estructura jerárquica)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WbsNodoDto>>), 200)]
    public async Task<IActionResult> GetArbol(long proyectoId, CancellationToken ct)
        => OkResult(await _service.GetArbolAsync(proyectoId, ct));

    /// <summary>Obtiene un nodo WBS con sus hijos directos</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long proyectoId, long id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFoundResult($"Nodo WBS {id} no encontrado.") : OkResult(dto);
    }

    /// <summary>Crea un nuevo nodo en el WBS (puede ser raíz o hijo de otro nodo)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WbsNodoDto>), 201)]
    public async Task<IActionResult> CreateNodo(long proyectoId, [FromBody] CrearWbsNodoRequest request, CancellationToken ct)
    {
        request.ProyectoId = proyectoId;
        request.EmpresaId = EmpresaId;
        var result = await _service.CreateNodoAsync(request, UsuarioId, ct);
        if (!result.Succeeded) return BadRequestResult(result.Error!);
        return CreatedAtAction(nameof(GetById), new { proyectoId, id = result.Data!.Id },
            ApiResponse<WbsNodoDto>.Ok(result.Data));
    }

    /// <summary>Actualiza nombre, tipo y peso de un nodo WBS</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateNodo(long proyectoId, long id, [FromBody] ActualizarWbsNodoRequest request, CancellationToken ct)
    {
        request.Id = id;
        return FromResult(await _service.UpdateNodoAsync(request, UsuarioId, ct));
    }

    /// <summary>Elimina un nodo WBS (solo si no tiene hijos activos)</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteNodo(long proyectoId, long id, CancellationToken ct)
        => FromResult(await _service.DeleteNodoAsync(id, UsuarioId, ct));
}
