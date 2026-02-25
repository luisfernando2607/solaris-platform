using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.RRHH;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations.RRHH;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> b)
    {
        b.ToTable("departamento", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).IsRequired();
        b.Property(e => e.Codigo).HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasMaxLength(500);
        b.Property(e => e.PresupuestoAnual).HasColumnType("numeric(18,2)");
        b.Property(e => e.Nivel).HasDefaultValue(1);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
        b.HasIndex(e => e.EmpresaId);

        b.HasOne(e => e.DepartamentoPadre)
            .WithMany(e => e.SubDepartamentos)
            .HasForeignKey(e => e.DepartamentoPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Responsable)
            .WithMany()
            .HasForeignKey(e => e.ResponsableId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PuestoConfiguration : IEntityTypeConfiguration<Puesto>
{
    public void Configure(EntityTypeBuilder<Puesto> b)
    {
        b.ToTable("puesto", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).IsRequired();
        b.Property(e => e.Codigo).HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasMaxLength(1000);
        b.Property(e => e.BandaSalarialMin).HasColumnType("numeric(18,2)");
        b.Property(e => e.BandaSalarialMax).HasColumnType("numeric(18,2)");
        b.Property(e => e.NivelJerarquico).HasDefaultValue(1);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
        b.HasIndex(e => e.DepartamentoId);

        b.HasOne(e => e.Departamento)
            .WithMany(d => d.Puestos)
            .HasForeignKey(e => e.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> b)
    {
        b.ToTable("empleado", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).IsRequired();
        b.Property(e => e.Codigo).HasMaxLength(20).IsRequired();
        b.Property(e => e.TipoIdentificacion).HasMaxLength(10).IsRequired();
        b.Property(e => e.NumeroIdentificacion).HasMaxLength(20).IsRequired();
        b.Property(e => e.PrimerNombre).HasMaxLength(100).IsRequired();
        b.Property(e => e.SegundoNombre).HasMaxLength(100);
        b.Property(e => e.PrimerApellido).HasMaxLength(100).IsRequired();
        b.Property(e => e.SegundoApellido).HasMaxLength(100);
        b.Property(e => e.EmailPersonal).HasMaxLength(200);
        b.Property(e => e.EmailCorporativo).HasMaxLength(200);
        b.Property(e => e.TelefonoCelular).HasMaxLength(20);
        b.Property(e => e.TelefonoFijo).HasMaxLength(20);
        b.Property(e => e.Direccion).HasMaxLength(500);
        b.Property(e => e.SalarioBase).HasColumnType("numeric(18,2)").IsRequired();
        b.Property(e => e.HorasSemanales).HasColumnType("numeric(5,2)").HasDefaultValue(40m);
        b.Property(e => e.NumeroSeguroSocial).HasMaxLength(20);
        b.Property(e => e.NumeroAfiliacion).HasMaxLength(30);
        b.Property(e => e.NumeroCuentaBancaria).HasMaxLength(30);
        b.Property(e => e.FotoUrl).HasMaxLength(500);
        b.Property(e => e.DescripcionEgreso).HasMaxLength(500);
        b.Property(e => e.Estado).HasDefaultValue((short)1);
        b.Property(e => e.TipoContrato).HasDefaultValue((short)1);
        b.Property(e => e.ModalidadTrabajo).HasDefaultValue((short)1);
        b.Property(e => e.JornadaLaboral).HasDefaultValue((short)1);

        b.Ignore(e => e.NombreCompleto);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
        b.HasIndex(e => new { e.EmpresaId, e.TipoIdentificacion, e.NumeroIdentificacion }).IsUnique();
        b.HasIndex(e => e.EmpresaId);
        b.HasIndex(e => e.DepartamentoId);

        b.HasOne(e => e.Departamento)
            .WithMany(d => d.Empleados)
            .HasForeignKey(e => e.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.Puesto)
            .WithMany(p => p.Empleados)
            .HasForeignKey(e => e.PuestoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(e => e.JefeDirecto)
            .WithMany()
            .HasForeignKey(e => e.JefeDirectoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ConceptoNominaConfiguration : IEntityTypeConfiguration<ConceptoNomina>
{
    public void Configure(EntityTypeBuilder<ConceptoNomina> b)
    {
        b.ToTable("concepto_nomina", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).IsRequired();
        b.Property(e => e.Codigo).HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasMaxLength(500);
        b.Property(e => e.Porcentaje).HasColumnType("numeric(8,4)");
        b.Property(e => e.ValorFijo).HasColumnType("numeric(18,2)");
        b.Property(e => e.Formula).HasMaxLength(1000);
        b.Property(e => e.PaisCodigo).HasMaxLength(5);
        b.Property(e => e.Tipo).IsRequired();
        b.Property(e => e.FormaCalculo).HasDefaultValue((short)1);
        b.Property(e => e.OrdenCalculo).HasDefaultValue(0);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
        b.HasIndex(e => e.EmpresaId);

        b.HasOne(e => e.ConceptoBase)
            .WithMany()
            .HasForeignKey(e => e.ConceptoBaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RolPagoConfiguration : IEntityTypeConfiguration<RolPago>
{
    public void Configure(EntityTypeBuilder<RolPago> b)
    {
        b.ToTable("rol_pago", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).IsRequired();
        b.Property(e => e.Numero).HasMaxLength(20).IsRequired();
        b.Property(e => e.Descripcion).HasMaxLength(300);
        b.Property(e => e.TotalIngresos).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalDescuentos).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalAportesPatronales).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalNeto).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.Observaciones).HasMaxLength(500);
        b.Property(e => e.Estado).HasDefaultValue((short)1);
        b.Property(e => e.Tipo).HasDefaultValue((short)1);

        b.HasIndex(e => new { e.EmpresaId, e.Numero }).IsUnique();
        b.HasIndex(e => e.PeriodoId);

        b.HasOne(e => e.Periodo)
            .WithMany(p => p.RolesPago)
            .HasForeignKey(e => e.PeriodoId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(e => e.Empleados)
            .WithOne(e => e.RolPago)
            .HasForeignKey(e => e.RolPagoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AsistenciaConfiguration : IEntityTypeConfiguration<Asistencia>
{
    public void Configure(EntityTypeBuilder<Asistencia> b)
    {
        b.ToTable("asistencia", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).IsRequired();
        b.Property(e => e.EmpresaId).IsRequired();
        b.Property(e => e.Fecha).IsRequired();
        b.Property(e => e.HorasTrabajadas).HasColumnType("numeric(5,2)");
        b.Property(e => e.HorasExtras).HasColumnType("numeric(5,2)").HasDefaultValue(0m);
        b.Property(e => e.Justificacion).HasMaxLength(500);
        b.Property(e => e.Estado).HasDefaultValue((short)1);
        b.Property(e => e.FuenteRegistro).HasDefaultValue((short)1);

        b.HasIndex(e => new { e.EmpleadoId, e.Fecha }).IsUnique();
        b.HasIndex(e => e.EmpresaId);

        b.HasOne(e => e.Empleado)
            .WithMany()
            .HasForeignKey(e => e.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
