namespace SolarisPlatform.Application.DTOs.Usuarios;

/// <summary>
/// DTO completo de usuario
/// </summary>
public class UsuarioDto
{
    public long Id { get; set; }
    public long EmpresaId { get; set; }
    public string? EmpresaNombre { get; set; }
    public long? SucursalId { get; set; }
    public string? SucursalNombre { get; set; }
    public string? Codigo { get; set; }
    public string Email { get; set; } = null!;
    public string? NombreUsuario { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string? NumeroIdentificacion { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Genero { get; set; }
    public string? FotoUrl { get; set; }
    public bool EmailVerificado { get; set; }
    public bool TwoFactorHabilitado { get; set; }
    public int Estado { get; set; }
    public string EstadoNombre { get; set; } = null!;
    public DateTime? UltimoAcceso { get; set; }
    public string IdiomaPreferido { get; set; } = "es";
    public string TemaPreferido { get; set; } = "light";
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public List<RolSimpleDto> Roles { get; set; } = new();
}

/// <summary>
/// DTO para listado de usuarios
/// </summary>
public class UsuarioListDto
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string? FotoUrl { get; set; }
    public int Estado { get; set; }
    public string EstadoNombre { get; set; } = null!;
    public DateTime? UltimoAcceso { get; set; }
    public bool Activo { get; set; }
    public string? RolPrincipal { get; set; }
}

/// <summary>
/// Rol simplificado
/// </summary>
public class RolSimpleDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Color { get; set; }
    public bool EsPrincipal { get; set; }
}

/// <summary>
/// Request para crear usuario
/// </summary>
public class CrearUsuarioRequest
{
    public long EmpresaId { get; set; }
    public long? SucursalId { get; set; }
    public string Email { get; set; } = null!;
    public string? NombreUsuario { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string? NumeroIdentificacion { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Genero { get; set; }
    public string Password { get; set; } = null!;
    public bool EnviarEmailBienvenida { get; set; } = true;
    public List<long> RolesIds { get; set; } = new();
}

/// <summary>
/// Request para actualizar usuario
/// </summary>
public class ActualizarUsuarioRequest
{
    public long Id { get; set; }
    public long? SucursalId { get; set; }
    public string? NombreUsuario { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string? NumeroIdentificacion { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Genero { get; set; }
    public string? FotoUrl { get; set; }
    public string IdiomaPreferido { get; set; } = "es";
    public string TemaPreferido { get; set; } = "light";
    public List<long> RolesIds { get; set; } = new();
}

/// <summary>
/// Request para filtrar usuarios
/// </summary>
public class FiltroUsuariosRequest
{
    public long? EmpresaId { get; set; }
    public long? SucursalId { get; set; }
    public string? Busqueda { get; set; }
    public int? Estado { get; set; }
    public bool? Activo { get; set; }
    public long? RolId { get; set; }
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 10;
    public string? OrdenarPor { get; set; }
    public bool OrdenDescendente { get; set; } = false;
}
