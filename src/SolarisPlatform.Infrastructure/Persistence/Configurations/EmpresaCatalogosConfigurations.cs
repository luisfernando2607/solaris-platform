using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.Empresas;
using SolarisPlatform.Domain.Entities.Catalogos;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Empresa
/// </summary>
public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("empresa", "emp");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(e => e.RazonSocial).HasColumnName("razon_social").HasMaxLength(200).IsRequired();
        builder.Property(e => e.NombreComercial).HasColumnName("nombre_comercial").HasMaxLength(200);
        builder.Property(e => e.TipoIdentificacion).HasColumnName("tipo_identificacion").HasMaxLength(20).IsRequired();
        builder.Property(e => e.NumeroIdentificacion).HasColumnName("numero_identificacion").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
        builder.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
        builder.Property(e => e.DireccionFiscal).HasColumnName("direccion_fiscal").HasMaxLength(500);
        builder.Property(e => e.Logo).HasColumnName("logo").HasMaxLength(500);
        builder.Property(e => e.PaginaWeb).HasColumnName("pagina_web").HasMaxLength(256);
        builder.Property(e => e.MonedaPrincipalId).HasColumnName("moneda_principal_id");
        builder.Property(e => e.PaisId).HasColumnName("pais_id");
        builder.Property(e => e.ZonaHoraria).HasColumnName("zona_horaria").HasMaxLength(50);
        builder.Property(e => e.PlanContratado).HasColumnName("plan_contratado").HasMaxLength(50);
        builder.Property(e => e.FechaInicioContrato).HasColumnName("fecha_inicio_contrato");
        builder.Property(e => e.FechaFinContrato).HasColumnName("fecha_fin_contrato");
        builder.Property(e => e.MaxUsuarios).HasColumnName("max_usuarios");
        builder.Property(e => e.Estado).HasColumnName("estado").HasConversion<int>();

        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Eliminado).HasColumnName("eliminado");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
        builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
        builder.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        builder.Property(e => e.FechaEliminacion).HasColumnName("fecha_eliminacion");
        builder.Property(e => e.UsuarioEliminacion).HasColumnName("usuario_eliminacion");

        builder.HasOne(e => e.MonedaPrincipal)
            .WithMany()
            .HasForeignKey(e => e.MonedaPrincipalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Pais)
            .WithMany()
            .HasForeignKey(e => e.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.HasIndex(e => e.NumeroIdentificacion).IsUnique();

        builder.HasQueryFilter(e => !e.Eliminado);
    }
}

/// <summary>
/// Configuración de Sucursal
/// </summary>
public class SucursalConfiguration : IEntityTypeConfiguration<Sucursal>
{
    public void Configure(EntityTypeBuilder<Sucursal> builder)
    {
        builder.ToTable("sucursal", "emp");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(500);
        builder.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(256);
        builder.Property(e => e.EsPrincipal).HasColumnName("es_principal");
        builder.Property(e => e.PaisId).HasColumnName("pais_id");
        builder.Property(e => e.EstadoProvinciaId).HasColumnName("estado_provincia_id");
        builder.Property(e => e.CiudadId).HasColumnName("ciudad_id");
        builder.Property(e => e.CodigoPostal).HasColumnName("codigo_postal").HasMaxLength(10);
        builder.Property(e => e.Latitud).HasColumnName("latitud").HasPrecision(10, 7);
        builder.Property(e => e.Longitud).HasColumnName("longitud").HasPrecision(10, 7);

        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Eliminado).HasColumnName("eliminado");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
        builder.Property(e => e.FechaModificacion).HasColumnName("fecha_modificacion");
        builder.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        builder.Property(e => e.FechaEliminacion).HasColumnName("fecha_eliminacion");
        builder.Property(e => e.UsuarioEliminacion).HasColumnName("usuario_eliminacion");

        builder.HasOne(e => e.Empresa)
            .WithMany(emp => emp.Sucursales)
            .HasForeignKey(e => e.EmpresaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Pais)
            .WithMany()
            .HasForeignKey(e => e.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EstadoProvincia)
            .WithMany()
            .HasForeignKey(e => e.EstadoProvinciaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Ciudad)
            .WithMany()
            .HasForeignKey(e => e.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();

        builder.HasQueryFilter(e => !e.Eliminado);
    }
}

/// <summary>
/// Configuración de País
/// </summary>
public class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("pais", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
        builder.Property(e => e.CodigoIso2).HasColumnName("codigo_iso2").HasMaxLength(2).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.NombreIngles).HasColumnName("nombre_ingles").HasMaxLength(100);
        builder.Property(e => e.CodigoTelefonico).HasColumnName("codigo_telefonico").HasMaxLength(10);
        builder.Property(e => e.Bandera).HasColumnName("bandera").HasMaxLength(10);
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.HasIndex(e => e.CodigoIso2).IsUnique();
    }
}

/// <summary>
/// Configuración de EstadoProvincia
/// </summary>
public class EstadoProvinciaConfiguration : IEntityTypeConfiguration<EstadoProvincia>
{
    public void Configure(EntityTypeBuilder<EstadoProvincia> builder)
    {
        builder.ToTable("estado_provincia", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.PaisId).HasColumnName("pais_id").IsRequired();
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.Pais)
            .WithMany(p => p.EstadosProvincias)
            .HasForeignKey(e => e.PaisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.PaisId, e.Codigo }).IsUnique();
    }
}

/// <summary>
/// Configuración de Ciudad
/// </summary>
public class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
{
    public void Configure(EntityTypeBuilder<Ciudad> builder)
    {
        builder.ToTable("ciudad", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EstadoProvinciaId).HasColumnName("estado_provincia_id").IsRequired();
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(10);
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.EstadoProvincia)
            .WithMany(ep => ep.Ciudades)
            .HasForeignKey(e => e.EstadoProvinciaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Configuración de Moneda
/// </summary>
public class MonedaConfiguration : IEntityTypeConfiguration<Moneda>
{
    public void Configure(EntityTypeBuilder<Moneda> builder)
    {
        builder.ToTable("moneda", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Simbolo).HasColumnName("simbolo").HasMaxLength(5).IsRequired();
        builder.Property(e => e.DecimalesPermitidos).HasColumnName("decimales_permitidos");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasIndex(e => e.Codigo).IsUnique();
    }
}

/// <summary>
/// Configuración de TipoIdentificacion
/// </summary>
public class TipoIdentificacionConfiguration : IEntityTypeConfiguration<TipoIdentificacion>
{
    public void Configure(EntityTypeBuilder<TipoIdentificacion> builder)
    {
        builder.ToTable("tipo_identificacion", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.PaisId).HasColumnName("pais_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Longitud).HasColumnName("longitud");
        builder.Property(e => e.Patron).HasColumnName("patron").HasMaxLength(100);
        builder.Property(e => e.AplicaPersona).HasColumnName("aplica_persona");
        builder.Property(e => e.AplicaEmpresa).HasColumnName("aplica_empresa");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.Pais)
            .WithMany()
            .HasForeignKey(e => e.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Codigo).IsUnique();
    }
}

/// <summary>
/// Configuración de Impuesto
/// </summary>
public class ImpuestoConfiguration : IEntityTypeConfiguration<Impuesto>
{
    public void Configure(EntityTypeBuilder<Impuesto> builder)
    {
        builder.ToTable("impuesto", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Porcentaje).HasColumnName("porcentaje").HasPrecision(5, 2);
        builder.Property(e => e.TipoImpuesto).HasColumnName("tipo_impuesto").HasMaxLength(20);
        builder.Property(e => e.EsRetencion).HasColumnName("es_retencion");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.Empresa)
            .WithMany()
            .HasForeignKey(e => e.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Configuración de FormaPago
/// </summary>
public class FormaPagoConfiguration : IEntityTypeConfiguration<FormaPago>
{
    public void Configure(EntityTypeBuilder<FormaPago> builder)
    {
        builder.ToTable("forma_pago", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(20);
        builder.Property(e => e.DiasCredito).HasColumnName("dias_credito");
        builder.Property(e => e.RequiereBanco).HasColumnName("requiere_banco");
        builder.Property(e => e.RequiereReferencia).HasColumnName("requiere_referencia");
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.Empresa)
            .WithMany()
            .HasForeignKey(e => e.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Configuración de Banco
/// </summary>
public class BancoConfiguration : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.ToTable("banco", "cat");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.PaisId).HasColumnName("pais_id");
        builder.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(e => e.NombreCorto).HasColumnName("nombre_corto").HasMaxLength(50);
        builder.Property(e => e.Activo).HasColumnName("activo");
        builder.Property(e => e.Orden).HasColumnName("orden");
        builder.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

        builder.HasOne(e => e.Pais)
            .WithMany()
            .HasForeignKey(e => e.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.Codigo).IsUnique();
    }
}
