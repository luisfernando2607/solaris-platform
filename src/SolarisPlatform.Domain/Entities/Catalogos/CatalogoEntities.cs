using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Entities.Empresas;

namespace SolarisPlatform.Domain.Entities.Catalogos;

public class Pais : CatalogoEntity
{
    public string Codigo { get; set; } = null!;
    public string CodigoIso2 { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? NombreIngles { get; set; }
    public string? CodigoTelefonico { get; set; }
    public string? Bandera { get; set; }
    
    public virtual ICollection<EstadoProvincia> EstadosProvincias { get; set; } = new List<EstadoProvincia>();
    public virtual ICollection<TipoIdentificacion> TiposIdentificacion { get; set; } = new List<TipoIdentificacion>();
    public virtual ICollection<Banco> Bancos { get; set; } = new List<Banco>();
}

public class EstadoProvincia : CatalogoEntity
{
    public long PaisId { get; set; }
    public Pais Pais { get; set; } = null!;
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    
    public virtual ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
}

public class Ciudad : CatalogoEntity
{
    public long EstadoProvinciaId { get; set; }
    public EstadoProvincia EstadoProvincia { get; set; } = null!;
    
    public string? Codigo { get; set; }
    public string Nombre { get; set; } = null!;
}

public class Moneda : CatalogoEntity
{
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Simbolo { get; set; } = null!;
    public short DecimalesPermitidos { get; set; } = 2;
}

public class TipoIdentificacion : CatalogoEntity
{
    public long? PaisId { get; set; }
    public Pais? Pais { get; set; }
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public int? Longitud { get; set; }
    public string? Patron { get; set; }
    public bool AplicaPersona { get; set; } = true;
    public bool AplicaEmpresa { get; set; } = true;
}

public class Impuesto : CatalogoEntity
{
    public long? EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public decimal Porcentaje { get; set; }
    public string TipoImpuesto { get; set; } = null!;
    public bool EsRetencion { get; set; } = false;
}

public class FormaPago : CatalogoEntity
{
    public long? EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public int DiasCredito { get; set; } = 0;
    public bool RequiereBanco { get; set; } = false;
    public bool RequiereReferencia { get; set; } = false;
}

public class Banco : CatalogoEntity
{
    public long? PaisId { get; set; }
    public Pais? Pais { get; set; }
    
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? NombreCorto { get; set; }
}
