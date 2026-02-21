namespace SolarisPlatform.Application.DTOs.Auth;

/// <summary>
/// Request para login
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool RecordarMe { get; set; } = false;
}

/// <summary>
/// Response del login
/// </summary>
public class LoginResponse
{
    public bool Exitoso { get; set; }
    public string? Mensaje { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expiracion { get; set; }
    public UsuarioAutenticadoDto? Usuario { get; set; }
}

/// <summary>
/// Datos del usuario autenticado
/// </summary>
public class UsuarioAutenticadoDto
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string? FotoUrl { get; set; }
    public long EmpresaId { get; set; }
    public string EmpresaNombre { get; set; } = null!;
    public long? SucursalId { get; set; }
    public string? SucursalNombre { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permisos { get; set; } = new();
    public string IdiomaPreferido { get; set; } = "es";
    public string TemaPreferido { get; set; } = "light";
    public bool RequiereCambioPassword { get; set; }
    public bool TwoFactorHabilitado { get; set; }
}

/// <summary>
/// Request para registro
/// </summary>
public class RegistroRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmarPassword { get; set; } = null!;
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string? Telefono { get; set; }
    public long EmpresaId { get; set; }
    public long? SucursalId { get; set; }
    public List<long>? RolIds { get; set; }
}

/// <summary>
/// Response del registro
/// </summary>
public class RegistroResponse
{
    public bool Exitoso { get; set; }
    public string? Mensaje { get; set; }
    public long? UsuarioId { get; set; }
}

/// <summary>
/// Request para refresh token
/// </summary>
public class RefreshTokenRequest
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

/// <summary>
/// Response del refresh token
/// </summary>
public class RefreshTokenResponse
{
    public bool Exitoso { get; set; }
    public string? Mensaje { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expiracion { get; set; }
}

/// <summary>
/// Request para cambio de contraseña
/// </summary>
public class CambiarPasswordRequest
{
    public string PasswordActual { get; set; } = null!;
    public string NuevoPassword { get; set; } = null!;
    public string ConfirmarPassword { get; set; } = null!;
}

/// <summary>
/// Request para recuperar contraseña
/// </summary>
public class RecuperarPasswordRequest
{
    public string Email { get; set; } = null!;
}

/// <summary>
/// Request para resetear contraseña
/// </summary>
public class ResetearPasswordRequest
{
    public string Token { get; set; } = null!;
    public string NuevoPassword { get; set; } = null!;
    public string ConfirmarPassword { get; set; } = null!;
}
