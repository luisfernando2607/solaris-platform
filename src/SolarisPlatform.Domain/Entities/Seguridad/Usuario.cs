using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Domain.Entities.Empresas;

namespace SolarisPlatform.Domain.Entities.Seguridad;

public class Usuario : SoftDeletableEntity
{
    public long EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    
    public long? SucursalId { get; set; }
    public Sucursal? Sucursal { get; set; }
    
    public string? Codigo { get; set; }
    public string Email { get; set; } = null!;
    public string? NombreUsuario { get; set; }
    
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string NombreCompleto => $"{Nombres} {Apellidos}";
    
    public long? TipoIdentificacionId { get; set; }
    public string? NumeroIdentificacion { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public char? Genero { get; set; }
    public string? FotoUrl { get; set; }
    
    public string PasswordHash { get; set; } = null!;
    public string? PasswordSalt { get; set; }
    public bool RequiereCambioPassword { get; set; } = false;
    public DateTime? FechaUltimoCambioPassword { get; set; }
    public int IntentosLoginFallidos { get; set; } = 0;
    public DateTime? FechaBloqueo { get; set; }
    
    public bool EmailVerificado { get; set; } = false;
    public string? TokenVerificacionEmail { get; set; }
    public DateTime? FechaExpiracionToken { get; set; }
    
    public string? TokenRecuperacion { get; set; }
    public DateTime? FechaExpiracionRecuperacion { get; set; }
    
    public bool TwoFactorHabilitado { get; set; } = false;
    public string? TwoFactorSecretKey { get; set; }
    
    public EstadoUsuario Estado { get; set; } = EstadoUsuario.Activo;
    public DateTime? UltimoAcceso { get; set; }
    
    public string IdiomaPreferido { get; set; } = "es";
    public string? ZonaHoraria { get; set; }
    public string TemaPreferido { get; set; } = "light";
    
    public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
    public virtual ICollection<UsuarioSucursal> UsuarioSucursales { get; set; } = new List<UsuarioSucursal>();
    public virtual ICollection<SesionUsuario> Sesiones { get; set; } = new List<SesionUsuario>();
    
    public bool EstaActivo() => Estado == EstadoUsuario.Activo && Activo && !Eliminado;
    public bool EstaBloqueado() => Estado == EstadoUsuario.Bloqueado || FechaBloqueo.HasValue;
    
    public void IncrementarIntentosLogin() => IntentosLoginFallidos++;
    
    public void ResetearIntentosLogin()
    {
        IntentosLoginFallidos = 0;
        FechaBloqueo = null;
    }
    
    public void Bloquear(int minutosBloqueo = 30)
    {
        Estado = EstadoUsuario.Bloqueado;
        FechaBloqueo = DateTime.UtcNow.AddMinutes(minutosBloqueo);
    }
    
    public void Desbloquear()
    {
        Estado = EstadoUsuario.Activo;
        FechaBloqueo = null;
        IntentosLoginFallidos = 0;
    }
    
    public void RegistrarAcceso()
    {
        UltimoAcceso = DateTime.UtcNow;
        ResetearIntentosLogin();
    }
}
