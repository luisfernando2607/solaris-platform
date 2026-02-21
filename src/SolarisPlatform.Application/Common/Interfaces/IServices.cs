using SolarisPlatform.Application.DTOs.Auth;
using SolarisPlatform.Application.DTOs.Usuarios;
using SolarisPlatform.Application.DTOs.Roles;
using SolarisPlatform.Application.Common.Models;

namespace SolarisPlatform.Application.Common.Interfaces;

/// <summary>
/// Servicio de autenticación
/// </summary>
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<RegistroResponse> RegistrarAsync(RegistroRequest request, CancellationToken cancellationToken = default);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(string token, CancellationToken cancellationToken = default);
    Task<Result> CambiarPasswordAsync(long usuarioId, CambiarPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> RecuperarPasswordAsync(RecuperarPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetearPasswordAsync(ResetearPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> VerificarEmailAsync(string token, CancellationToken cancellationToken = default);
    Task<UsuarioAutenticadoDto?> GetUsuarioActualAsync(long usuarioId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Servicio de usuarios
/// </summary>
public interface IUsuarioService
{
    Task<UsuarioDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<UsuarioDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PaginatedList<UsuarioListDto>> GetListAsync(FiltroUsuariosRequest filtro, CancellationToken cancellationToken = default);
    Task<Result<UsuarioDto>> CreateAsync(CrearUsuarioRequest request, long usuarioCreacion, CancellationToken cancellationToken = default);
    Task<Result<UsuarioDto>> UpdateAsync(ActualizarUsuarioRequest request, long usuarioModificacion, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(long id, long usuarioEliminacion, CancellationToken cancellationToken = default);
    Task<Result> ActivarAsync(long id, long usuarioModificacion, CancellationToken cancellationToken = default);
    Task<Result> DesactivarAsync(long id, long usuarioModificacion, CancellationToken cancellationToken = default);
    Task<Result> BloquearAsync(long id, int minutos, long usuarioModificacion, CancellationToken cancellationToken = default);
    Task<Result> DesbloquearAsync(long id, long usuarioModificacion, CancellationToken cancellationToken = default);
    Task<Result> ResetearPasswordAsync(long id, long usuarioModificacion, CancellationToken cancellationToken = default);
}

/// <summary>
/// Servicio de roles
/// </summary>
public interface IRolService
{
    Task<RolDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RolListDto>> GetAllAsync(long? empresaId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RolListDto>> GetDisponiblesAsync(long empresaId, CancellationToken cancellationToken = default);
    Task<Result<RolDto>> CreateAsync(CrearRolRequest request, long usuarioCreacion, CancellationToken cancellationToken = default);
    Task<Result<RolDto>> UpdateAsync(ActualizarRolRequest request, long usuarioModificacion, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(long id, long usuarioEliminacion, CancellationToken cancellationToken = default);
    Task<IEnumerable<ModuloConPermisosDto>> GetModulosConPermisosAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Servicio de tokens JWT
/// </summary>
public interface ITokenService
{
    string GenerarToken(UsuarioAutenticadoDto usuario);
    string GenerarRefreshToken();
    bool ValidarToken(string token);
    long? ObtenerUsuarioIdDesdeToken(string token);
}

/// <summary>
/// Servicio de contraseñas
/// </summary>
public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerarPasswordTemporal(int longitud = 12);
}

/// <summary>
/// Servicio para obtener usuario actual
/// </summary>
public interface ICurrentUserService
{
    long? UsuarioId { get; }
    long? EmpresaId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Permisos { get; }
    bool TienePermiso(string permiso);
    bool TieneRol(string rol);
}

/// <summary>
/// Servicio de fecha/hora
/// </summary>
public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

/// <summary>
/// Servicio de email
/// </summary>
public interface IEmailService
{
    Task<bool> EnviarAsync(string para, string asunto, string cuerpoHtml, CancellationToken cancellationToken = default);
    Task<bool> EnviarBienvenidaAsync(string para, string nombreUsuario, string passwordTemporal, CancellationToken cancellationToken = default);
    Task<bool> EnviarRecuperacionPasswordAsync(string para, string nombreUsuario, string token, CancellationToken cancellationToken = default);
    Task<bool> EnviarVerificacionEmailAsync(string para, string nombreUsuario, string token, CancellationToken cancellationToken = default);
}
