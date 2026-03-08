using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Roles;

namespace SolarisPlatform.API.Controllers;

/// <summary>
/// Controller de roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRolService rolService,
        ICurrentUserService currentUserService,
        ILogger<RolesController> logger)
    {
        _rolService = rolService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los roles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RolListDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var empresaId = _currentUserService.TieneRol("SUPER_ADMIN") 
            ? null 
            : _currentUserService.EmpresaId;

        var roles = await _rolService.GetAllAsync(empresaId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RolListDto>>.Ok(roles));
    }

    /// <summary>
    /// Obtener roles disponibles para asignar
    /// </summary>
    [HttpGet("disponibles")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RolListDto>>>> GetDisponibles(
        CancellationToken cancellationToken)
    {
        var empresaId = _currentUserService.EmpresaId ?? 0;
        var roles = await _rolService.GetDisponiblesAsync(empresaId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RolListDto>>.Ok(roles));
    }

    /// <summary>
    /// Obtener rol por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RolDto>>> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var rol = await _rolService.GetByIdAsync(id, cancellationToken);

        if (rol == null)
        {
            return NotFound(ApiResponse<RolDto>.Fail("Rol no encontrado"));
        }

        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") && 
            rol.EmpresaId.HasValue && 
            rol.EmpresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        return Ok(ApiResponse<RolDto>.Ok(rol));
    }

    /// <summary>
    /// Crear nuevo rol
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RolDto>>> Create(
        [FromBody] CrearRolRequest request,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;

        // Si no es admin, el rol pertenece a su empresa
        if (!_currentUserService.TieneRol("SUPER_ADMIN"))
        {
            request.EmpresaId = _currentUserService.EmpresaId;
        }

        var result = await _rolService.CreateAsync(request, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<RolDto>.Fail(result.Error ?? "Error al crear rol", result.Errors));
        }

        _logger.LogInformation("Rol {Codigo} creado por {CreadorId}", request.Codigo, usuarioId);
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data!.Id },
            ApiResponse<RolDto>.Ok(result.Data, "Rol creado exitosamente"));
    }

    /// <summary>
    /// Actualizar rol
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<RolDto>>> Update(
        long id,
        [FromBody] ActualizarRolRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;
        if (id != request.Id)
        {
            return BadRequest(ApiResponse<RolDto>.Fail("El ID no coincide"));
        }

        var usuarioId = _currentUserService.UsuarioId ?? 0;

        // Verificar que el rol existe
        var rolExistente = await _rolService.GetByIdAsync(id, cancellationToken);
        if (rolExistente == null)
        {
            return NotFound(ApiResponse<RolDto>.Fail("Rol no encontrado"));
        }

        // No se pueden modificar roles de sistema
        if (rolExistente.EsSistema)
        {
            return BadRequest(ApiResponse<RolDto>.Fail("No se pueden modificar roles de sistema"));
        }

        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") && 
            rolExistente.EmpresaId.HasValue && 
            rolExistente.EmpresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var result = await _rolService.UpdateAsync(request, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<RolDto>.Fail(result.Error ?? "Error al actualizar rol", result.Errors));
        }

        _logger.LogInformation("Rol {Id} actualizado por {ModificadorId}", id, usuarioId);
        return Ok(ApiResponse<RolDto>.Ok(result.Data!, "Rol actualizado exitosamente"));
    }

    /// <summary>
    /// Eliminar rol
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(
        long id,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId ?? 0;

        // Verificar que el rol existe
        var rol = await _rolService.GetByIdAsync(id, cancellationToken);
        if (rol == null)
        {
            return NotFound(ApiResponse.Fail("Rol no encontrado"));
        }

        // No se pueden eliminar roles de sistema
        if (rol.EsSistema)
        {
            return BadRequest(ApiResponse.Fail("No se pueden eliminar roles de sistema"));
        }

        // Verificar que no tenga usuarios asignados
        if (rol.CantidadUsuarios > 0)
        {
            return BadRequest(ApiResponse.Fail($"No se puede eliminar el rol porque tiene {rol.CantidadUsuarios} usuarios asignados"));
        }

        // Verificar acceso
        if (!_currentUserService.TieneRol("SUPER_ADMIN") && 
            rol.EmpresaId.HasValue && 
            rol.EmpresaId != _currentUserService.EmpresaId)
        {
            return Forbid();
        }

        var result = await _rolService.DeleteAsync(id, usuarioId, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al eliminar rol"));
        }

        _logger.LogInformation("Rol {Id} eliminado por {EliminadorId}", id, usuarioId);
        return Ok(ApiResponse.Ok(message: "Rol eliminado exitosamente"));
    }

    /// <summary>
    /// Obtener árbol de módulos con permisos
    /// </summary>
    [HttpGet("modulos-permisos")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ModuloConPermisosDto>>>> GetModulosConPermisos(
        CancellationToken cancellationToken)
    {
        var modulos = await _rolService.GetModulosConPermisosAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<ModuloConPermisosDto>>.Ok(modulos));
    }
}
