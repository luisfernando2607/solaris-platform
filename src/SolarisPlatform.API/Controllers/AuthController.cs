using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Interfaces;
using SolarisPlatform.Application.Common.Models;
using SolarisPlatform.Application.DTOs.Auth;

namespace SolarisPlatform.API.Controllers;

/// <summary>
/// Controller de autenticación
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Iniciar sesión
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.Exitoso)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(result.Mensaje ?? "Error al iniciar sesión"));
        }

        _logger.LogInformation("Usuario {Email} inició sesión exitosamente", request.Email);
        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login exitoso"));
    }

    /// <summary>
    /// Registrar nuevo usuario
    /// </summary>
    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RegistroResponse>>> Registro(
        [FromBody] RegistroRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegistrarAsync(request, cancellationToken);

        if (!result.Exitoso)
        {
            return BadRequest(ApiResponse<RegistroResponse>.Fail(result.Mensaje ?? "Error al registrar"));
        }

        _logger.LogInformation("Usuario {Email} registrado exitosamente", request.Email);
        return Ok(ApiResponse<RegistroResponse>.Ok(result, "Usuario registrado exitosamente"));
    }

    /// <summary>
    /// Refrescar token JWT
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponse>>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);

        if (!result.Exitoso)
        {
            return Unauthorized(ApiResponse<RefreshTokenResponse>.Fail(result.Mensaje ?? "Token inválido"));
        }

        return Ok(ApiResponse<RefreshTokenResponse>.Ok(result, "Token renovado"));
    }

    /// <summary>
    /// Cerrar sesión
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Logout(CancellationToken cancellationToken)
    {
        var token = HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");

        await _authService.LogoutAsync(token, cancellationToken);

        _logger.LogInformation("Usuario {UserId} cerró sesión", _currentUserService.UsuarioId);
        return Ok(ApiResponse.Ok(message: "Sesión cerrada exitosamente"));
    }

    /// <summary>
    /// Cambiar contraseña del usuario actual
    /// </summary>
    [HttpPost("cambiar-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> CambiarPassword(
        [FromBody] CambiarPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId;
        if (!usuarioId.HasValue)
        {
            return Unauthorized(ApiResponse.Fail("Usuario no autenticado"));
        }

        var result = await _authService.CambiarPasswordAsync(usuarioId.Value, request, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al cambiar contraseña"));
        }

        _logger.LogInformation("Usuario {UserId} cambió su contraseña", usuarioId);
        return Ok(ApiResponse.Ok(message: "Contraseña cambiada exitosamente"));
    }

    /// <summary>
    /// Solicitar recuperación de contraseña
    /// </summary>
    [HttpPost("recuperar-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> RecuperarPassword(
        [FromBody] RecuperarPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _authService.RecuperarPasswordAsync(request, cancellationToken);

        // Por seguridad, siempre devolvemos éxito
        return Ok(ApiResponse.Ok(message: "Si el email existe, recibirás instrucciones para recuperar tu contraseña"));
    }

    /// <summary>
    /// Resetear contraseña con token
    /// </summary>
    [HttpPost("resetear-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> ResetearPassword(
        [FromBody] ResetearPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ResetearPasswordAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Error al resetear contraseña"));
        }

        return Ok(ApiResponse.Ok(message: "Contraseña reseteada exitosamente"));
    }

    /// <summary>
    /// Verificar email con token
    /// </summary>
    [HttpGet("verificar-email/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse>> VerificarEmail(
        string token,
        CancellationToken cancellationToken)
    {
        var result = await _authService.VerificarEmailAsync(token, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.Fail(result.Error ?? "Token inválido o expirado"));
        }

        return Ok(ApiResponse.Ok(message: "Email verificado exitosamente"));
    }

    /// <summary>
    /// Obtener información del usuario autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UsuarioAutenticadoDto>>> GetUsuarioActual(
        CancellationToken cancellationToken)
    {
        var usuarioId = _currentUserService.UsuarioId;
        if (!usuarioId.HasValue)
        {
            return Unauthorized(ApiResponse<UsuarioAutenticadoDto>.Fail("Usuario no autenticado"));
        }

        var usuario = await _authService.GetUsuarioActualAsync(usuarioId.Value, cancellationToken);

        if (usuario == null)
        {
            return NotFound(ApiResponse<UsuarioAutenticadoDto>.Fail("Usuario no encontrado"));
        }

        return Ok(ApiResponse<UsuarioAutenticadoDto>.Ok(usuario));
    }
}
