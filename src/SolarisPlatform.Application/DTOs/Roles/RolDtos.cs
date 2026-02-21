namespace SolarisPlatform.Application.DTOs.Roles;

/// <summary>
/// DTO completo de rol
/// </summary>
public class RolDto
{
    public long Id { get; set; }
    public long? EmpresaId { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EsSistema { get; set; }
    public int Nivel { get; set; }
    public string? Color { get; set; }
    public string? Icono { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int CantidadUsuarios { get; set; }
    public List<PermisoDto> Permisos { get; set; } = new();
}

/// <summary>
/// DTO para listado de roles
/// </summary>
public class RolListDto
{
    public long Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EsSistema { get; set; }
    public int Nivel { get; set; }
    public string? Color { get; set; }
    public bool Activo { get; set; }
    public int CantidadUsuarios { get; set; }
}

/// <summary>
/// Request para crear rol
/// </summary>
public class CrearRolRequest
{
    public long? EmpresaId { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Nivel { get; set; } = 0;
    public string? Color { get; set; }
    public string? Icono { get; set; }
    public List<long> PermisosIds { get; set; } = new();
}

/// <summary>
/// Request para actualizar rol
/// </summary>
public class ActualizarRolRequest
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Nivel { get; set; }
    public string? Color { get; set; }
    public string? Icono { get; set; }
    public bool Activo { get; set; }
    public List<long> PermisosIds { get; set; } = new();
}

/// <summary>
/// DTO de permiso
/// </summary>
public class PermisoDto
{
    public long Id { get; set; }
    public long ModuloId { get; set; }
    public string ModuloNombre { get; set; } = null!;
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string TipoPermiso { get; set; } = null!;
    public bool Activo { get; set; }
}

/// <summary>
/// DTO de módulo con permisos
/// </summary>
public class ModuloConPermisosDto
{
    public long Id { get; set; }
    public long? ModuloPadreId { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public List<PermisoDto> Permisos { get; set; } = new();
    public List<ModuloConPermisosDto> SubModulos { get; set; } = new();
}
