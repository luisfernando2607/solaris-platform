using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Usuarios;

namespace SolarisPlatform.API.Controllers;

/// <summary>
/// Controller de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioService usuarioService,
        ICurrentUserService currentUserService,
        ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener lista paginada de usuarios
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedApiResponse<UsuarioListDto>>> GetAll(
        [FromQuery] FiltroUsuariosRequest filtro,
        CancellationToken cancellationToken)
    {
        // Si no es admin, filtrar por empresa del usuario actual
        if (!_currentUserService.TieneRol("SUPER_ADMIN"))
        {
            filtro.EmpresaId = _currentUserService.EmpresaId;
        }

        var result = await _usuarioService.GetListAsync(filtro, cancellationToken);
        return Ok(PaginatedApiResponse<UsuarioListDto>.FromPaginatedList(result));
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);

        if (usuario == null)
        {
            return NotFound(ApiResponse<UsuarioDto>.Fail("Usuario no encontrado"));
        }

        // Verificar acceso a la empresa
        if (!_currentUserService.TieneRol("SUPER_ADMIN") && 
            usuario.EmpresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        return Ok(ApiResponse<UsuarioDto>.Ok(usuario));
    }

    /// <summary>
    /// Crear nuevo usuario
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> Create(
        [FromBody] CrearUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;

        // Si no es admin, solo puede crear en su empresa
        if (!_currentUserService.TieneRol("SUPER_ADMIN"))
        {
            request.EmpresaId = _currentUserService.EmpresaId ?? request.EmpresaId;
        }

        var result = await _usuarioService.CreateAsync(request, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<UsuarioDto>.Fail(result.Error ?? "Error al crear usuario", result.Errors));
        }

        _logger.LogInformation("Usuario {Email} creado por {CreadorId}", request.Email, usuarioId);
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            ApiResponse<UsuarioDto>.Ok(result.Data, "Usuario creado exitosamente"));
    }

    /// <summary>
    /// Actualizar usuario
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> Update(
        long id,
        [FromBody] ActualizarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest(ApiResponse<UsuarioDto>.Fail("El ID no coincide"));
        }

        var usuarioId = _currentUserService.UsuarioId ?? 0;

        // Verificar que el usuario existe y tiene acceso
        var usuarioExistente = await _usuarioService.GetByIdAsync(id, cancellationToken);
        if (usuarioExistente == null)
        {
            return NotFound(ApiResponse<UsuarioDto>.Fail("Usuario no encontrado"));
        }

        if (!_currentUserService.TieneRol("SUPER_ADMIN") && 
            usuarioExistente.EmpresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var result = await _usuarioService.UpdateAsync(request, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<UsuarioDto>.Fail(result.Error ?? "Error al actualizar usuario", result.Errors));
        }

        _logger.LogInformation("Usuario {Id} actualizado por {ModificadorId}", id, usuarioId);
        return Ok(ApiResponse<UsuarioDto>.Ok(result.Data!, "Usuario actualizado exitosamente"));
    }

    /// <summary>
    /// Eliminar usuario (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;

        // No puede eliminarse a sí mismo
        if (id == usuarioId)
        {
            return BadRequest(ApiResponse.Fail("No puede eliminarse a sí mismo"));
        }

        // Verificar acceso
        var usuario = await _usuarioService.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return NotFound(ApiResponse.Fail("Usuario no encontrado"));
        }

        if (!_currentUserService.TieneRol("SUPER_ADMIN") && 
            usuario.EmpresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var result = await _usuarioService.DeleteAsync(id, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al eliminar usuario"));
        }

        _logger.LogInformation("Usuario {Id} eliminado por {EliminadorId}", id, usuarioId);
        return Ok(ApiResponse.Ok(message: "Usuario eliminado exitosamente"));
    }

    /// <summary>
    /// Activar usuario
    /// </summary>
    [HttpPost("{id}/activar")]
    public async Task<ActionResult<ApiResponse>> Activar(
        long id,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;
        var result = await _usuarioService.ActivarAsync(id, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al activar usuario"));
        }

        _logger.LogInformation("Usuario {Id} activado por {ActivadorId}", id, usuarioId);
        return Ok(ApiResponse.Ok(message: "Usuario activado exitosamente"));
    }

    /// <summary>
    /// Desactivar usuario
    /// </summary>
    [HttpPost("{id}/desactivar")]
    public async Task<ActionResult<ApiResponse>> Desactivar(
        long id,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;

        if (id == usuarioId)
        {
            return BadRequest(ApiResponse.Fail("No puede desactivarse a sí mismo"));
        }

        var result = await _usuarioService.DesactivarAsync(id, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al desactivar usuario"));
        }

        _logger.LogInformation("Usuario {Id} desactivado por {DesactivadorId}", id, usuarioId);
        return Ok(ApiResponse.Ok(message: "Usuario desactivado exitosamente"));
    }

    /// <summary>
    /// Bloquear usuario temporalmente
    /// </summary>
    [HttpPost("{id}/bloquear")]
    public async Task<ActionResult<ApiResponse>> Bloquear(
        long id,
        [FromQuery] int minutos = 30,
        CancellationToken cancellationToken = default)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;

        if (id == usuarioId)
        {
            return BadRequest(ApiResponse.Fail("No puede bloquearse a sí mismo"));
        }

        var result = await _usuarioService.BloquearAsync(id, minutos, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al bloquear usuario"));
        }

        _logger.LogInformation("Usuario {Id} bloqueado por {BloqueadorId} por {Minutos} minutos", id, usuarioId, minutos);
        return Ok(ApiResponse.Ok(message: $"Usuario bloqueado por {minutos} minutos"));
    }

    /// <summary>
    /// Desbloquear usuario
    /// </summary>
    [HttpPost("{id}/desbloquear")]
    public async Task<ActionResult<ApiResponse>> Desbloquear(
        long id,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;
        var result = await _usuarioService.DesbloquearAsync(id, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al desbloquear usuario"));
        }

        _logger.LogInformation("Usuario {Id} desbloqueado por {DesbloqueadorId}", id, usuarioId);
        return Ok(ApiResponse.Ok(message: "Usuario desbloqueado exitosamente"));
    }

    /// <summary>
    /// Resetear contraseña de usuario
    /// </summary>
    [HttpPost("{id}/resetear-password")]
    public async Task<ActionResult<ApiResponse>> ResetearPassword(
        long id,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;
        var result = await _usuarioService.ResetearPasswordAsync(id, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al resetear contraseña"));
        }

        _logger.LogInformation("Contraseña de usuario {Id} reseteada por {ResetadorId}", id, usuarioId);
        return Ok(ApiResponse.Ok(message: "Contraseña reseteada. Se enviará un email con la nueva contraseña temporal."));
    }
}
