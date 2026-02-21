using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums;
using SolarisPlatform.Domain.Entities.Catalogos;

namespace SolarisPlatform.Domain.Entities.Empresas;

public class Empresa : SoftDeletableEntity
{
    public string Codigo { get; set; } = null!;
    public string RazonSocial { get; set; } = null!;
    public string? NombreComercial { get; set; }
    public string TipoIdentificacion { get; set; } = null!;
    public string NumeroIdentificacion { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? DireccionFiscal { get; set; }
    public string? Logo { get; set; }
    public string? PaginaWeb { get; set; }
    
    public long? MonedaPrincipalId { get; set; }
    public Moneda? MonedaPrincipal { get; set; }
    
    public long? PaisId { get; set; }
    public Pais? Pais { get; set; }
    
    public string ZonaHoraria { get; set; } = "America/Guayaquil";
    
    public string PlanContratado { get; set; } = "BASICO";
    public DateTime FechaInicioContrato { get; set; }
    public DateTime? FechaFinContrato { get; set; }
    public int MaxUsuarios { get; set; } = 5;
    
    public EstadoEmpresa Estado { get; set; } = EstadoEmpresa.Activo;
    
    public virtual ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    
    public bool EstaActiva() => Estado == EstadoEmpresa.Activo && Activo && !Eliminado;
    public bool ContratoVigente() => !FechaFinContrato.HasValue || FechaFinContrato > DateTime.UtcNow;
}

public class Sucursal : SoftDeletableEntity
{
    public long EmpresaId { get; set; }
    public Empresa Empresa { get; set; } = null!;
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool EsPrincipal { get; set; } = false;
    
    public long? PaisId { get; set; }
    public Pais? Pais { get; set; }
    
    public long? EstadoProvinciaId { get; set; }
    public EstadoProvincia? EstadoProvincia { get; set; }
    
    public long? CiudadId { get; set; }
    public Ciudad? Ciudad { get; set; }
    
    public string? CodigoPostal { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
}
