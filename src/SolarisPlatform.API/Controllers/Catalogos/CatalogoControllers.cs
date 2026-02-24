// =====================================================
// API LAYER - Controllers de Catálogos
// Archivo: API/Controllers/Catalogos/
// =====================================================

using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.DTOs.Catalogos;

namespace SolarisPlatform.API.Controllers.Catalogos;

// ══════════════════════════════════════════════════════
// CONTROLLER: PAÍSES
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/paises")]
[Authorize]
public class PaisesController(
    IPaisService service,
    IValidator<CrearPaisRequest> crearValidator,
    IValidator<ActualizarPaisRequest> actualizarValidator) : ControllerBase
{
    /// <summary>Obtener todos los países</summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = false)
    {
        var result = await service.ObtenerTodosAsync(soloActivos);
        return Ok(result);
    }

    /// <summary>Obtener país por ID</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Crear nuevo país</summary>
    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearPaisRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    /// <summary>Actualizar país</summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarPaisRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Activar / Desactivar país</summary>
    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Eliminar país</summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: ESTADOS / PROVINCIAS
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/estados-provincias")]
[Authorize]
public class EstadosProvinciasController(
    IEstadoProvinciaService service,
    IValidator<CrearEstadoProvinciaRequest> crearValidator,
    IValidator<ActualizarEstadoProvinciaRequest> actualizarValidator) : ControllerBase
{
    /// <summary>Obtener todos los estados/provincias</summary>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = false)
    {
        var result = await service.ObtenerTodosAsync(soloActivos);
        return Ok(result);
    }

    /// <summary>Obtener estados/provincias por país</summary>
    [HttpGet("por-pais/{paisId:long}")]
    public async Task<IActionResult> ObtenerPorPais(long paisId, [FromQuery] bool soloActivos = false)
    {
        var result = await service.ObtenerPorPaisAsync(paisId, soloActivos);
        return Ok(result);
    }

    /// <summary>Obtener estado/provincia por ID</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Crear nueva provincia</summary>
    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearEstadoProvinciaRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    /// <summary>Actualizar provincia</summary>
    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEstadoProvinciaRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Activar / Desactivar provincia</summary>
    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Eliminar provincia</summary>
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: CIUDADES
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/ciudades")]
[Authorize]
public class CiudadesController(
    ICiudadService service,
    IValidator<CrearCiudadRequest> crearValidator,
    IValidator<ActualizarCiudadRequest> actualizarValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerTodosAsync(soloActivos));

    [HttpGet("por-estado/{estadoId:long}")]
    public async Task<IActionResult> ObtenerPorEstado(long estadoId, [FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerPorEstadoAsync(estadoId, soloActivos));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearCiudadRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarCiudadRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: MONEDAS
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/monedas")]
[Authorize]
public class MonedasController(
    IMonedaService service,
    IValidator<CrearMonedaRequest> crearValidator,
    IValidator<ActualizarMonedaRequest> actualizarValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerTodosAsync(soloActivos));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearMonedaRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarMonedaRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: TIPOS DE IDENTIFICACIÓN
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/tipos-identificacion")]
[Authorize]
public class TiposIdentificacionController(
    ITipoIdentificacionService service,
    IValidator<CrearTipoIdentificacionRequest> crearValidator,
    IValidator<ActualizarTipoIdentificacionRequest> actualizarValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerTodosAsync(soloActivos));

    [HttpGet("por-pais/{paisId:long}")]
    public async Task<IActionResult> ObtenerPorPais(long paisId, [FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerPorPaisAsync(paisId, soloActivos));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearTipoIdentificacionRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarTipoIdentificacionRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: IMPUESTOS
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/impuestos")]
[Authorize]
public class ImpuestosController(
    IImpuestoService service,
    IValidator<CrearImpuestoRequest> crearValidator,
    IValidator<ActualizarImpuestoRequest> actualizarValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] long? empresaId, [FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerTodosAsync(empresaId, soloActivos));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearImpuestoRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarImpuestoRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: FORMAS DE PAGO
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/formas-pago")]
[Authorize]
public class FormasPagoController(
    IFormaPagoService service,
    IValidator<CrearFormaPagoRequest> crearValidator,
    IValidator<ActualizarFormaPagoRequest> actualizarValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] long? empresaId, [FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerTodosAsync(empresaId, soloActivos));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearFormaPagoRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarFormaPagoRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// CONTROLLER: BANCOS
// ══════════════════════════════════════════════════════
[ApiController]
[Route("api/catalogos/bancos")]
[Authorize]
public class BancosController(
    IBancoService service,
    IValidator<CrearBancoRequest> crearValidator,
    IValidator<ActualizarBancoRequest> actualizarValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] long? paisId, [FromQuery] bool soloActivos = false) =>
        Ok(await service.ObtenerTodosAsync(paisId, soloActivos));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var result = await service.ObtenerPorIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Crear([FromBody] CrearBancoRequest request)
    {
        var validation = await crearValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.CrearAsync(request);
        return result.Success ? CreatedAtAction(nameof(ObtenerPorId), new { id = result.Data!.Id }, result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarBancoRequest request)
    {
        var validation = await actualizarValidator.ValidateAsync(request);
        if (!validation.IsValid) return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await service.ActualizarAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "SUPER_ADMIN,ADMIN_EMPRESA")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] CambiarEstadoRequest request)
    {
        var result = await service.CambiarEstadoAsync(id, request.Activo);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "SUPER_ADMIN")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var result = await service.EliminarAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// ══════════════════════════════════════════════════════
// REQUEST COMPARTIDO
// ══════════════════════════════════════════════════════

/// <summary>Request genérico para cambiar estado (activo/inactivo)</summary>
public record CambiarEstadoRequest(bool Activo);
