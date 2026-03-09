// =====================================================
// INFRASTRUCTURE LAYER - EF Core Configurations
// Archivo: Infrastructure/Persistence/Configurations/Catalogos/
// Convención: PostgreSQL snake_case, esquema "cat"
// =====================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.Catalogos;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations.Catalogos;

// ══════════════════════════════════════════════════════
// PAÍS - sin relaciones (se definen en el lado hijo)
// ══════════════════════════════════════════════════════
public class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("pais", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(3).IsRequired();
        builder.Property(x => x.CodigoIso2).HasColumnName("codigo_iso2").HasMaxLength(2).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NombreIngles).HasColumnName("nombre_ingles").HasMaxLength(100);
        builder.Property(x => x.CodigoTelefonico).HasColumnName("codigo_telefonico").HasMaxLength(10);
        builder.Property(x => x.Bandera).HasColumnName("bandera").HasMaxLength(10);
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("uq_pais_codigo");
        builder.HasIndex(x => x.CodigoIso2).IsUnique().HasDatabaseName("uq_pais_codigo_iso2");
    }
}

// ══════════════════════════════════════════════════════
// ESTADO / PROVINCIA
// ══════════════════════════════════════════════════════
public class EstadoProvinciaConfiguration : IEntityTypeConfiguration<EstadoProvincia>
{
    public void Configure(EntityTypeBuilder<EstadoProvincia> builder)
    {
        builder.ToTable("estado_provincia", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.PaisId).HasColumnName("pais_id").IsRequired();
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => new { x.PaisId, x.Codigo }).IsUnique().HasDatabaseName("uq_estado_pais_codigo");
        builder.HasIndex(x => x.PaisId).HasDatabaseName("ix_estado_pais_id");

        builder.HasOne(x => x.Pais)
            .WithMany(x => x.EstadosProvincias)
            .HasForeignKey(x => x.PaisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// ══════════════════════════════════════════════════════
// CIUDAD
// ══════════════════════════════════════════════════════
public class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
{
    public void Configure(EntityTypeBuilder<Ciudad> builder)
    {
        builder.ToTable("ciudad", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.EstadoProvinciaId).HasColumnName("estado_provincia_id").IsRequired();
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(10);
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.EstadoProvinciaId).HasDatabaseName("ix_ciudad_estado_id");

        builder.HasOne(x => x.EstadoProvincia)
            .WithMany(x => x.Ciudades)
            .HasForeignKey(x => x.EstadoProvinciaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// ══════════════════════════════════════════════════════
// MONEDA
// ══════════════════════════════════════════════════════
public class MonedaConfiguration : IEntityTypeConfiguration<Moneda>
{
    public void Configure(EntityTypeBuilder<Moneda> builder)
    {
        builder.ToTable("moneda", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(3).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Simbolo).HasColumnName("simbolo").HasMaxLength(10).IsRequired();
        builder.Property(x => x.DecimalesPermitidos).HasColumnName("decimales_permitidos").HasDefaultValue((short)2);
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("uq_moneda_codigo");
    }
}

// ══════════════════════════════════════════════════════
// TIPO DE IDENTIFICACIÓN
// ══════════════════════════════════════════════════════
public class TipoIdentificacionConfiguration : IEntityTypeConfiguration<TipoIdentificacion>
{
    public void Configure(EntityTypeBuilder<TipoIdentificacion> builder)
    {
        builder.ToTable("tipo_identificacion", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.PaisId).HasColumnName("pais_id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Longitud).HasColumnName("longitud");
        builder.Property(x => x.Patron).HasColumnName("patron").HasMaxLength(100);
        builder.Property(x => x.AplicaPersona).HasColumnName("aplica_persona").HasDefaultValue(true);
        builder.Property(x => x.AplicaEmpresa).HasColumnName("aplica_empresa").HasDefaultValue(true);
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("uq_tipo_identificacion_codigo");
        builder.HasIndex(x => x.PaisId).HasDatabaseName("ix_tipo_identificacion_pais_id");

        builder.HasOne(x => x.Pais)
            .WithMany(x => x.TiposIdentificacion)
            .HasForeignKey(x => x.PaisId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// ══════════════════════════════════════════════════════
// IMPUESTO
// ══════════════════════════════════════════════════════
public class ImpuestoConfiguration : IEntityTypeConfiguration<Impuesto>
{
    public void Configure(EntityTypeBuilder<Impuesto> builder)
    {
        builder.ToTable("impuesto", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.EmpresaId).HasColumnName("empresa_id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Porcentaje).HasColumnName("porcentaje").HasPrecision(5, 2).IsRequired();
        builder.Property(x => x.TipoImpuesto).HasColumnName("tipo_impuesto").HasMaxLength(20).IsRequired();
        builder.Property(x => x.EsRetencion).HasColumnName("es_retencion").HasDefaultValue(false);
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.EmpresaId).HasDatabaseName("ix_impuesto_empresa_id");
        builder.ToTable(t => t.HasCheckConstraint("ck_impuesto_porcentaje", "porcentaje >= 0 AND porcentaje <= 100"));
    }
}

// ══════════════════════════════════════════════════════
// FORMA DE PAGO
// ══════════════════════════════════════════════════════
public class FormaPagoConfiguration : IEntityTypeConfiguration<FormaPago>
{
    public void Configure(EntityTypeBuilder<FormaPago> builder)
    {
        builder.ToTable("forma_pago", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.EmpresaId).HasColumnName("empresa_id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DiasCredito).HasColumnName("dias_credito").HasDefaultValue(0);
        builder.Property(x => x.RequiereBanco).HasColumnName("requiere_banco").HasDefaultValue(false);
        builder.Property(x => x.RequiereReferencia).HasColumnName("requiere_referencia").HasDefaultValue(false);
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.EmpresaId).HasDatabaseName("ix_forma_pago_empresa_id");
    }
}

// ══════════════════════════════════════════════════════
// BANCO
// ══════════════════════════════════════════════════════
public class BancoConfiguration : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.ToTable("banco", "cat");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(x => x.PaisId).HasColumnName("pais_id");
        builder.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NombreCorto).HasColumnName("nombre_corto").HasMaxLength(50);
        builder.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        builder.Property(x => x.Orden).HasColumnName("orden").HasDefaultValue(0);
        builder.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("NOW()");
        builder.HasIndex(x => x.PaisId).HasDatabaseName("ix_banco_pais_id");

        builder.HasOne(x => x.Pais)
            .WithMany(x => x.Bancos)
            .HasForeignKey(x => x.PaisId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}