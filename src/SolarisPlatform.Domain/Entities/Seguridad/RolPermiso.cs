using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Entities.Empresas;

namespace SolarisPlatform.Domain.Entities.Seguridad;

public class Rol : SoftDeletableEntity
{
    public long? EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EsSistema { get; set; } = false;
    public int Nivel { get; set; } = 0;
    public string? Color { get; set; }
    public string? Icono { get; set; }
    
    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
}

public class Modulo : BaseEntity
{
    public long? ModuloPadreId { get; set; }
    public Modulo? ModuloPadre { get; set; }
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public string? Ruta { get; set; }
    public int Orden { get; set; } = 0;
    public bool EsMenu { get; set; } = true;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Modulo> SubModulos { get; set; } = new List<Modulo>();
    public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
}

public class Permiso : BaseEntity
{
    public long ModuloId { get; set; }
    public Modulo Modulo { get; set; } = null!;
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string TipoPermiso { get; set; } = "ACCION";
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}

public class RolPermiso : BaseEntity
{
    public long RolId { get; set; }
    public Rol Rol { get; set; } = null!;
    
    public long PermisoId { get; set; }
    public Permiso Permiso { get; set; } = null!;
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public long? UsuarioCreacion { get; set; }
}

public class UsuarioRol : BaseEntity
{
    public long UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public long RolId { get; set; }
    public Rol Rol { get; set; } = null!;
    
    public bool EsPrincipal { get; set; } = false;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public long? UsuarioCreacion { get; set; }
}

public class UsuarioSucursal : BaseEntity
{
    public long UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public long SucursalId { get; set; }
    public Sucursal Sucursal { get; set; } = null!;
    
    public bool EsPrincipal { get; set; } = false;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public long? UsuarioCreacion { get; set; }
}

public class SesionUsuario : BaseEntity
{
    public long UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    
    public string Token { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
    public DateTime FechaExpiracion { get; set; }
    public DateTime FechaUltimaActividad { get; set; } = DateTime.UtcNow;
    
    public string? DireccionIp { get; set; }
    public string? UserAgent { get; set; }
    public string? Dispositivo { get; set; }
    public string? Navegador { get; set; }
    public string? SistemaOperativo { get; set; }
    public string? PaisAcceso { get; set; }
    public string? CiudadAcceso { get; set; }
    
    public bool Activo { get; set; } = true;
    public bool CerradaPorUsuario { get; set; } = false;
    public DateTime? FechaCierre { get; set; }
    
    public bool EstaExpirada() => DateTime.UtcNow > FechaExpiracion;
    
    public void Cerrar()
    {
        Activo = false;
        CerradaPorUsuario = true;
        FechaCierre = DateTime.UtcNow;
    }
    
    public void ActualizarActividad() => FechaUltimaActividad = DateTime.UtcNow;
}
