using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.RRHH;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations.RRHH;

// ─────────────────────────────────────────────────────────────────────
// ESTRUCTURA ORGANIZACIONAL
// ─────────────────────────────────────────────────────────────────────

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> b)
    {
        b.ToTable("departamento", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.DepartamentoPadreId).HasColumnName("departamento_padre_id");
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.ResponsableId).HasColumnName("responsable_id");
        b.Property(e => e.PresupuestoAnual).HasColumnName("presupuesto_anual").HasColumnType("numeric(18,2)");
        b.Property(e => e.Nivel).HasColumnName("nivel").HasDefaultValue(1);

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
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.DepartamentoId).HasColumnName("departamento_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(1000);
        b.Property(e => e.NivelJerarquico).HasColumnName("nivel_jerarquico").HasDefaultValue((short)1);
        b.Property(e => e.BandaSalarialMin).HasColumnName("banda_salarial_min").HasColumnType("numeric(18,2)");
        b.Property(e => e.BandaSalarialMax).HasColumnName("banda_salarial_max").HasColumnType("numeric(18,2)");
        b.Property(e => e.MonedaId).HasColumnName("moneda_id");
        b.Property(e => e.RequiereTitulo).HasColumnName("requiere_titulo");

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
        b.HasIndex(e => e.DepartamentoId);

        b.HasOne(e => e.Departamento)
            .WithMany(d => d.Puestos)
            .HasForeignKey(e => e.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// ─────────────────────────────────────────────────────────────────────
// EMPLEADOS
// ─────────────────────────────────────────────────────────────────────

public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
{
    public void Configure(EntityTypeBuilder<Empleado> b)
    {
        b.ToTable("empleado", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.SucursalId).HasColumnName("sucursal_id");
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.TipoIdentificacion).HasColumnName("tipo_identificacion").HasMaxLength(10).IsRequired();
        b.Property(e => e.NumeroIdentificacion).HasColumnName("numero_identificacion").HasMaxLength(20).IsRequired();
        b.Property(e => e.PrimerNombre).HasColumnName("primer_nombre").HasMaxLength(100).IsRequired();
        b.Property(e => e.SegundoNombre).HasColumnName("segundo_nombre").HasMaxLength(100);
        b.Property(e => e.PrimerApellido).HasColumnName("primer_apellido").HasMaxLength(100).IsRequired();
        b.Property(e => e.SegundoApellido).HasColumnName("segundo_apellido").HasMaxLength(100);
        b.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
        b.Property(e => e.Genero).HasColumnName("genero");
        b.Property(e => e.EstadoCivil).HasColumnName("estado_civil");
        b.Property(e => e.NacionalidadPaisId).HasColumnName("nacionalidad_pais_id");
        b.Property(e => e.EmailPersonal).HasColumnName("email_personal").HasMaxLength(200);
        b.Property(e => e.EmailCorporativo).HasColumnName("email_corporativo").HasMaxLength(200);
        b.Property(e => e.TelefonoCelular).HasColumnName("telefono_celular").HasMaxLength(20);
        b.Property(e => e.TelefonoFijo).HasColumnName("telefono_fijo").HasMaxLength(20);
        b.Property(e => e.PaisId).HasColumnName("pais_id");
        b.Property(e => e.EstadoProvinciaId).HasColumnName("estado_provincia_id");
        b.Property(e => e.CiudadId).HasColumnName("ciudad_id");
        b.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(500);
        b.Property(e => e.DepartamentoId).HasColumnName("departamento_id");
        b.Property(e => e.PuestoId).HasColumnName("puesto_id");
        b.Property(e => e.JefeDirectoId).HasColumnName("jefe_directo_id");
        b.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso").IsRequired();
        b.Property(e => e.FechaEgreso).HasColumnName("fecha_egreso");
        b.Property(e => e.TipoContrato).HasColumnName("tipo_contrato").HasDefaultValue((short)1);
        b.Property(e => e.ModalidadTrabajo).HasColumnName("modalidad_trabajo").HasDefaultValue((short)1);
        b.Property(e => e.JornadaLaboral).HasColumnName("jornada_laboral").HasDefaultValue((short)1);
        b.Property(e => e.HorasSemanales).HasColumnName("horas_semanales").HasColumnType("numeric(5,2)").HasDefaultValue(40m);
        b.Property(e => e.SalarioBase).HasColumnName("salario_base").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(e => e.MonedaId).HasColumnName("moneda_id");
        b.Property(e => e.NumeroSeguroSocial).HasColumnName("numero_seguro_social").HasMaxLength(20);
        b.Property(e => e.NumeroAfiliacion).HasColumnName("numero_afiliacion").HasMaxLength(30);
        b.Property(e => e.BancoId).HasColumnName("banco_id");
        b.Property(e => e.TipoCuentaBancaria).HasColumnName("tipo_cuenta_bancaria");
        b.Property(e => e.NumeroCuentaBancaria).HasColumnName("numero_cuenta_bancaria").HasMaxLength(30);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.MotivoEgreso).HasColumnName("motivo_egreso");
        b.Property(e => e.DescripcionEgreso).HasColumnName("descripcion_egreso").HasMaxLength(500);
        b.Property(e => e.FotoUrl).HasColumnName("foto_url").HasMaxLength(500);
        b.Property(e => e.UsuarioId).HasColumnName("usuario_id");

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

public class EmpleadoHistorialConfiguration : IEntityTypeConfiguration<EmpleadoHistorial>
{
    public void Configure(EntityTypeBuilder<EmpleadoHistorial> b)
    {
        b.ToTable("empleado_historial", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.TipoCambio).HasColumnName("tipo_cambio").IsRequired();
        b.Property(e => e.FechaEfectiva).HasColumnName("fecha_efectiva").IsRequired();
        b.Property(e => e.DepartamentoAnteriorId).HasColumnName("departamento_anterior_id");
        b.Property(e => e.PuestoAnteriorId).HasColumnName("puesto_anterior_id");
        b.Property(e => e.SalarioAnterior).HasColumnName("salario_anterior").HasColumnType("numeric(18,2)");
        b.Property(e => e.TipoContratoAnterior).HasColumnName("tipo_contrato_anterior");
        b.Property(e => e.DepartamentoNuevoId).HasColumnName("departamento_nuevo_id");
        b.Property(e => e.PuestoNuevoId).HasColumnName("puesto_nuevo_id");
        b.Property(e => e.SalarioNuevo).HasColumnName("salario_nuevo").HasColumnType("numeric(18,2)");
        b.Property(e => e.TipoContratoNuevo).HasColumnName("tipo_contrato_nuevo");
        b.Property(e => e.Motivo).HasColumnName("motivo").HasMaxLength(500);
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(1000);

        b.HasOne(e => e.Empleado)
            .WithMany(e => e.Historial)
            .HasForeignKey(e => e.EmpleadoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmpleadoDocumentoConfiguration : IEntityTypeConfiguration<EmpleadoDocumento>
{
    public void Configure(EntityTypeBuilder<EmpleadoDocumento> b)
    {
        b.ToTable("empleado_documento", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.TipoDocumento).HasColumnName("tipo_documento").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.ArchivoUrl).HasColumnName("archivo_url").HasMaxLength(500);
        b.Property(e => e.FechaDocumento).HasColumnName("fecha_documento");
        b.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");

        b.HasOne(e => e.Empleado)
            .WithMany(e => e.Documentos)
            .HasForeignKey(e => e.EmpleadoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmpleadoConceptoConfiguration : IEntityTypeConfiguration<EmpleadoConcepto>
{
    public void Configure(EntityTypeBuilder<EmpleadoConcepto> b)
    {
        b.ToTable("empleado_concepto", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.ConceptoId).HasColumnName("concepto_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Valor).HasColumnName("valor").HasColumnType("numeric(18,2)");
        b.Property(e => e.Porcentaje).HasColumnName("porcentaje").HasColumnType("numeric(8,4)");
        b.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        b.Property(e => e.FechaFin).HasColumnName("fecha_fin");
        b.Property(e => e.CuotasTotal).HasColumnName("cuotas_total");
        b.Property(e => e.CuotasPagadas).HasColumnName("cuotas_pagadas").HasDefaultValue(0);
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(500);
    }
}

// ─────────────────────────────────────────────────────────────────────
// RECLUTAMIENTO
// ─────────────────────────────────────────────────────────────────────

public class RequisicionPersonalConfiguration : IEntityTypeConfiguration<RequisicionPersonal>
{
    public void Configure(EntityTypeBuilder<RequisicionPersonal> b)
    {
        b.ToTable("requisicion_personal", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Numero).HasColumnName("numero").HasMaxLength(20).IsRequired();
        b.Property(e => e.DepartamentoId).HasColumnName("departamento_id").IsRequired();
        b.Property(e => e.PuestoId).HasColumnName("puesto_id").IsRequired();
        b.Property(e => e.SolicitanteId).HasColumnName("solicitante_id").IsRequired();
        b.Property(e => e.Motivo).HasColumnName("motivo").IsRequired();
        b.Property(e => e.CantidadPlazas).HasColumnName("cantidad_plazas").HasDefaultValue(1);
        b.Property(e => e.FechaSolicitud).HasColumnName("fecha_solicitud").IsRequired();
        b.Property(e => e.FechaRequerida).HasColumnName("fecha_requerida");
        b.Property(e => e.SalarioOfrecidoMin).HasColumnName("salario_ofrecido_min").HasColumnType("numeric(18,2)");
        b.Property(e => e.SalarioOfrecidoMax).HasColumnName("salario_ofrecido_max").HasColumnType("numeric(18,2)");
        b.Property(e => e.DescripcionPerfil).HasColumnName("descripcion_perfil");
        b.Property(e => e.Requisitos).HasColumnName("requisitos");
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.AprobadoPorId).HasColumnName("aprobado_por_id");
        b.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(1000);
    }
}

public class CandidatoConfiguration : IEntityTypeConfiguration<Candidato>
{
    public void Configure(EntityTypeBuilder<Candidato> b)
    {
        b.ToTable("candidato", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PrimerNombre).HasColumnName("primer_nombre").HasMaxLength(100).IsRequired();
        b.Property(e => e.PrimerApellido).HasColumnName("primer_apellido").HasMaxLength(100).IsRequired();
        b.Property(e => e.TipoIdentificacion).HasColumnName("tipo_identificacion").HasMaxLength(10);
        b.Property(e => e.NumeroIdentificacion).HasColumnName("numero_identificacion").HasMaxLength(20);
        b.Property(e => e.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
        b.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
        b.Property(e => e.PaisId).HasColumnName("pais_id");
        b.Property(e => e.CiudadId).HasColumnName("ciudad_id");
        b.Property(e => e.CvUrl).HasColumnName("cv_url").HasMaxLength(500);
        b.Property(e => e.LinkedinUrl).HasColumnName("linkedin_url").HasMaxLength(200);
        b.Property(e => e.NivelEducacion).HasColumnName("nivel_educacion");
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
    }
}

public class ProcesoSeleccionConfiguration : IEntityTypeConfiguration<ProcesoSeleccion>
{
    public void Configure(EntityTypeBuilder<ProcesoSeleccion> b)
    {
        b.ToTable("proceso_seleccion", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.RequisicionId).HasColumnName("requisicion_id").IsRequired();
        b.Property(e => e.ResponsableId).HasColumnName("responsable_id").IsRequired();
        b.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        b.Property(e => e.FechaCierre).HasColumnName("fecha_cierre");
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(1000);
    }
}

public class PostulacionConfiguration : IEntityTypeConfiguration<Postulacion>
{
    public void Configure(EntityTypeBuilder<Postulacion> b)
    {
        b.ToTable("postulacion", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.ProcesoId).HasColumnName("proceso_id").IsRequired();
        b.Property(e => e.CandidatoId).HasColumnName("candidato_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Fuente).HasColumnName("fuente");
        b.Property(e => e.EtapaActual).HasColumnName("etapa_actual").HasDefaultValue((short)1);
        b.Property(e => e.Calificacion).HasColumnName("calificacion").HasColumnType("numeric(5,2)");
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(1000);
        b.Property(e => e.FechaPostulacion).HasColumnName("fecha_postulacion");
    }
}

public class EntrevistaConfiguration : IEntityTypeConfiguration<Entrevista>
{
    public void Configure(EntityTypeBuilder<Entrevista> b)
    {
        b.ToTable("entrevista", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.PostulacionId).HasColumnName("postulacion_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo").HasDefaultValue((short)1);
        b.Property(e => e.EntrevistadorId).HasColumnName("entrevistador_id").IsRequired();
        b.Property(e => e.FechaProgramada).HasColumnName("fecha_programada").IsRequired();
        b.Property(e => e.FechaRealizada).HasColumnName("fecha_realizada");
        b.Property(e => e.DuracionMinutos).HasColumnName("duracion_minutos");
        b.Property(e => e.EnlaceVideo).HasColumnName("enlace_video").HasMaxLength(500);
        b.Property(e => e.Calificacion).HasColumnName("calificacion").HasColumnType("numeric(5,2)");
        b.Property(e => e.Resultado).HasColumnName("resultado");
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(1000);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
    }
}

// ─────────────────────────────────────────────────────────────────────
// TIEMPO Y ASISTENCIA
// ─────────────────────────────────────────────────────────────────────

public class HorarioConfiguration : IEntityTypeConfiguration<Horario>
{
    public void Configure(EntityTypeBuilder<Horario> b)
    {
        b.ToTable("horario", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.Tipo).HasColumnName("tipo").HasDefaultValue((short)1);
        b.Property(e => e.HoraEntrada).HasColumnName("hora_entrada");
        b.Property(e => e.HoraSalida).HasColumnName("hora_salida");
        b.Property(e => e.HorasDiarias).HasColumnName("horas_diarias").HasColumnType("numeric(5,2)").HasDefaultValue(8m);
        b.Property(e => e.DiasLaborables).HasColumnName("dias_laborables").HasColumnType("jsonb");
        b.Property(e => e.ToleranciEntradaMin).HasColumnName("tolerancia_entrada_min").HasDefaultValue(0);
        b.Property(e => e.ToleranciaSalidaMin).HasColumnName("tolerancia_salida_min").HasDefaultValue(0);
    }
}

public class AsistenciaConfiguration : IEntityTypeConfiguration<Asistencia>
{
    public void Configure(EntityTypeBuilder<Asistencia> b)
    {
        b.ToTable("asistencia", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
        b.Property(e => e.HoraEntrada).HasColumnName("hora_entrada");
        b.Property(e => e.HoraSalida).HasColumnName("hora_salida");
        b.Property(e => e.HoraEntradaAlmuerzo).HasColumnName("hora_entrada_almuerzo");
        b.Property(e => e.HoraSalidaAlmuerzo).HasColumnName("hora_salida_almuerzo");
        b.Property(e => e.HorasTrabajadas).HasColumnName("horas_trabajadas").HasColumnType("numeric(5,2)");
        b.Property(e => e.HorasExtras).HasColumnName("horas_extras").HasColumnType("numeric(5,2)").HasDefaultValue(0m);
        b.Property(e => e.MinutosTardanza).HasColumnName("minutos_tardanza").HasDefaultValue(0);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.TipoAusencia).HasColumnName("tipo_ausencia");
        b.Property(e => e.Justificacion).HasColumnName("justificacion").HasMaxLength(500);
        b.Property(e => e.FuenteRegistro).HasColumnName("fuente_registro").HasDefaultValue((short)1);

        b.HasIndex(e => new { e.EmpleadoId, e.Fecha }).IsUnique();
        b.HasIndex(e => e.EmpresaId);

        b.HasOne(e => e.Empleado)
            .WithMany()
            .HasForeignKey(e => e.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SolicitudAusenciaConfiguration : IEntityTypeConfiguration<SolicitudAusencia>
{
    public void Configure(EntityTypeBuilder<SolicitudAusencia> b)
    {
        b.ToTable("solicitud_ausencia", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo").IsRequired();
        b.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        b.Property(e => e.FechaFin).HasColumnName("fecha_fin").IsRequired();
        b.Property(e => e.DiasHabiles).HasColumnName("dias_habiles").HasColumnType("numeric(5,2)");
        b.Property(e => e.Motivo).HasColumnName("motivo").HasMaxLength(500);
        b.Property(e => e.DocumentoUrl).HasColumnName("documento_url").HasMaxLength(500);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.AprobadorId).HasColumnName("aprobador_id");
        b.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");
        b.Property(e => e.ObservacionAprobador).HasColumnName("observacion_aprobador").HasMaxLength(500);

        b.HasOne(e => e.Empleado).WithMany().HasForeignKey(e => e.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.Aprobador).WithMany().HasForeignKey(e => e.AprobadorId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SaldoVacacionesConfiguration : IEntityTypeConfiguration<SaldoVacaciones>
{
    public void Configure(EntityTypeBuilder<SaldoVacaciones> b)
    {
        b.ToTable("saldo_vacaciones", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Anno).HasColumnName("anno").IsRequired();
        b.Property(e => e.DiasGanados).HasColumnName("dias_ganados").HasColumnType("numeric(5,2)").HasDefaultValue(0m);
        b.Property(e => e.DiasTomados).HasColumnName("dias_tomados").HasColumnType("numeric(5,2)").HasDefaultValue(0m);
        b.Property(e => e.DiasVencidos).HasColumnName("dias_vencidos").HasColumnType("numeric(5,2)").HasDefaultValue(0m);
        b.Ignore(e => e.DiasPendientes);

        b.HasIndex(e => new { e.EmpleadoId, e.Anno }).IsUnique();
        b.HasOne(e => e.Empleado).WithMany(e => e.SaldosVacaciones).HasForeignKey(e => e.EmpleadoId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ─────────────────────────────────────────────────────────────────────
// NÓMINA
// ─────────────────────────────────────────────────────────────────────

public class ConceptoNominaConfiguration : IEntityTypeConfiguration<ConceptoNomina>
{
    public void Configure(EntityTypeBuilder<ConceptoNomina> b)
    {
        b.ToTable("concepto_nomina", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.Tipo).HasColumnName("tipo").IsRequired();
        b.Property(e => e.Subtipo).HasColumnName("subtipo");
        b.Property(e => e.FormaCalculo).HasColumnName("forma_calculo").HasDefaultValue((short)1);
        b.Property(e => e.Porcentaje).HasColumnName("porcentaje").HasColumnType("numeric(8,4)");
        b.Property(e => e.ConceptoBaseId).HasColumnName("concepto_base_id");
        b.Property(e => e.Formula).HasColumnName("formula").HasMaxLength(1000);
        b.Property(e => e.ValorFijo).HasColumnName("valor_fijo").HasColumnType("numeric(18,2)");
        b.Property(e => e.GravableRenta).HasColumnName("gravable_renta").HasDefaultValue(true);
        b.Property(e => e.GravableSeguridadSocial).HasColumnName("gravable_seguridad_social").HasDefaultValue(true);
        b.Property(e => e.AplicaTodosEmpleados).HasColumnName("aplica_todos_empleados").HasDefaultValue(false);
        b.Property(e => e.EsObligatorio).HasColumnName("es_obligatorio").HasDefaultValue(false);
        b.Property(e => e.EsSistema).HasColumnName("es_sistema").HasDefaultValue(false);
        b.Property(e => e.OrdenCalculo).HasColumnName("orden_calculo").HasDefaultValue(0);
        b.Property(e => e.PaisCodigo).HasColumnName("pais_codigo").HasMaxLength(5);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
        b.HasOne(e => e.ConceptoBase).WithMany().HasForeignKey(e => e.ConceptoBaseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PeriodoNominaConfiguration : IEntityTypeConfiguration<PeriodoNomina>
{
    public void Configure(EntityTypeBuilder<PeriodoNomina> b)
    {
        b.ToTable("periodo_nomina", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Anno).HasColumnName("anno").IsRequired();
        b.Property(e => e.NumeroPeriodo).HasColumnName("numero_periodo").IsRequired();
        b.Property(e => e.TipoPeriodo).HasColumnName("tipo_periodo").HasDefaultValue((short)1);
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(100).IsRequired();
        b.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        b.Property(e => e.FechaFin).HasColumnName("fecha_fin").IsRequired();
        b.Property(e => e.FechaPago).HasColumnName("fecha_pago");
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
    }
}

public class RolPagoConfiguration : IEntityTypeConfiguration<RolPago>
{
    public void Configure(EntityTypeBuilder<RolPago> b)
    {
        b.ToTable("rol_pago", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PeriodoId).HasColumnName("periodo_id").IsRequired();
        b.Property(e => e.Numero).HasColumnName("numero").HasMaxLength(20).IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo").HasDefaultValue((short)1);
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(300);
        b.Property(e => e.TotalIngresos).HasColumnName("total_ingresos").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalDescuentos).HasColumnName("total_descuentos").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalAportesPatronales).HasColumnName("total_aportes_patronales").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalNeto).HasColumnName("total_neto").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalEmpleados).HasColumnName("total_empleados").HasDefaultValue(0);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.AprobadoPorId).HasColumnName("aprobado_por_id");
        b.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(500);

        b.HasIndex(e => new { e.EmpresaId, e.Numero }).IsUnique();
        b.HasIndex(e => e.PeriodoId);

        b.HasOne(e => e.Periodo).WithMany(p => p.RolesPago).HasForeignKey(e => e.PeriodoId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.Empleados).WithOne(e => e.RolPago).HasForeignKey(e => e.RolPagoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RolPagoEmpleadoConfiguration : IEntityTypeConfiguration<RolPagoEmpleado>
{
    public void Configure(EntityTypeBuilder<RolPagoEmpleado> b)
    {
        b.ToTable("rol_pago_empleado", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.RolPagoId).HasColumnName("rol_pago_id").IsRequired();
        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PuestoNombre).HasColumnName("puesto_nombre").HasMaxLength(150);
        b.Property(e => e.DepartamentoNombre).HasColumnName("departamento_nombre").HasMaxLength(150);
        b.Property(e => e.SalarioBase).HasColumnName("salario_base").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(e => e.DiasTrabajados).HasColumnName("dias_trabajados").HasColumnType("numeric(5,2)").HasDefaultValue(30m);
        b.Property(e => e.HorasExtras).HasColumnName("horas_extras").HasColumnType("numeric(5,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalIngresos).HasColumnName("total_ingresos").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalDescuentos).HasColumnName("total_descuentos").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.TotalAportesPatronales).HasColumnName("total_aportes_patronales").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.NetoAPagar).HasColumnName("neto_a_pagar").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.EstadoPago).HasColumnName("estado_pago").HasDefaultValue((short)1);
        b.Property(e => e.FechaPago).HasColumnName("fecha_pago");
        b.Property(e => e.ReferenciaPago).HasColumnName("referencia_pago").HasMaxLength(100);

        b.HasOne(e => e.Empleado).WithMany().HasForeignKey(e => e.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.Detalles).WithOne(d => d.RolPagoEmpleado).HasForeignKey(d => d.RolPagoEmpleadoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RolPagoDetalleConfiguration : IEntityTypeConfiguration<RolPagoDetalle>
{
    public void Configure(EntityTypeBuilder<RolPagoDetalle> b)
    {
        b.ToTable("rol_pago_detalle", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.RolPagoEmpleadoId).HasColumnName("rol_pago_empleado_id").IsRequired();
        b.Property(e => e.ConceptoId).HasColumnName("concepto_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.BaseCalculo).HasColumnName("base_calculo").HasColumnType("numeric(18,2)");
        b.Property(e => e.PorcentajeAplicado).HasColumnName("porcentaje_aplicado").HasColumnType("numeric(8,4)");
        b.Property(e => e.Cantidad).HasColumnName("cantidad").HasColumnType("numeric(10,4)").HasDefaultValue(1m);
        b.Property(e => e.Valor).HasColumnName("valor").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo").IsRequired();

        b.HasOne(e => e.Concepto).WithMany().HasForeignKey(e => e.ConceptoId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ─────────────────────────────────────────────────────────────────────
// PRÉSTAMOS
// ─────────────────────────────────────────────────────────────────────

public class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
{
    public void Configure(EntityTypeBuilder<Prestamo> b)
    {
        b.ToTable("prestamo", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.Numero).HasColumnName("numero").HasMaxLength(20).IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo").HasDefaultValue((short)1);
        b.Property(e => e.MontoSolicitado).HasColumnName("monto_solicitado").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(e => e.MontoAprobado).HasColumnName("monto_aprobado").HasColumnType("numeric(18,2)");
        b.Property(e => e.NumeroCuotas).HasColumnName("numero_cuotas").HasDefaultValue(1);
        b.Property(e => e.CuotaMensual).HasColumnName("cuota_mensual").HasColumnType("numeric(18,2)");
        b.Property(e => e.FechaSolicitud).HasColumnName("fecha_solicitud").IsRequired();
        b.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");
        b.Property(e => e.FechaPrimerDescuento).HasColumnName("fecha_primer_descuento");
        b.Property(e => e.Motivo).HasColumnName("motivo").HasMaxLength(500);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.AprobadoPorId).HasColumnName("aprobado_por_id");
        b.Property(e => e.SaldoPendiente).HasColumnName("saldo_pendiente").HasColumnType("numeric(18,2)").HasDefaultValue(0m);

        b.HasOne(e => e.Empleado).WithMany(e => e.Prestamos).HasForeignKey(e => e.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.Cuotas).WithOne(c => c.Prestamo).HasForeignKey(c => c.PrestamoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PrestamoCuotaConfiguration : IEntityTypeConfiguration<PrestamoCuota>
{
    public void Configure(EntityTypeBuilder<PrestamoCuota> b)
    {
        b.ToTable("prestamo_cuota", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.PrestamoId).HasColumnName("prestamo_id").IsRequired();
        b.Property(e => e.NumeroCuota).HasColumnName("numero_cuota").IsRequired();
        b.Property(e => e.FechaDescuento).HasColumnName("fecha_descuento").IsRequired();
        b.Property(e => e.ValorCuota).HasColumnName("valor_cuota").HasColumnType("numeric(18,2)").IsRequired();
        b.Property(e => e.ValorPagado).HasColumnName("valor_pagado").HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        b.Property(e => e.RolPagoDetalleId).HasColumnName("rol_pago_detalle_id");
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
    }
}

// ─────────────────────────────────────────────────────────────────────
// EVALUACIÓN
// ─────────────────────────────────────────────────────────────────────

public class PlantillaEvaluacionConfiguration : IEntityTypeConfiguration<PlantillaEvaluacion>
{
    public void Configure(EntityTypeBuilder<PlantillaEvaluacion> b)
    {
        b.ToTable("plantilla_evaluacion", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.Tipo).HasColumnName("tipo").HasDefaultValue((short)1);
        b.Property(e => e.EscalaMin).HasColumnName("escala_min").HasColumnType("numeric(5,2)").HasDefaultValue(1m);
        b.Property(e => e.EscalaMax).HasColumnName("escala_max").HasColumnType("numeric(5,2)").HasDefaultValue(5m);
        b.Property(e => e.Instrucciones).HasColumnName("instrucciones");
    }
}

public class PlantillaCriterioConfiguration : IEntityTypeConfiguration<PlantillaCriterio>
{
    public void Configure(EntityTypeBuilder<PlantillaCriterio> b)
    {
        b.ToTable("plantilla_criterio", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.PlantillaId).HasColumnName("plantilla_id").IsRequired();
        b.Property(e => e.CriterioPadreId).HasColumnName("criterio_padre_id");
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.Peso).HasColumnName("peso").HasColumnType("numeric(5,2)").HasDefaultValue(100m);
        b.Property(e => e.Orden).HasColumnName("orden").HasDefaultValue(0);

        b.HasOne(e => e.CriterioPadre).WithMany(e => e.SubCriterios).HasForeignKey(e => e.CriterioPadreId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EvaluacionProcesoConfiguration : IEntityTypeConfiguration<EvaluacionProceso>
{
    public void Configure(EntityTypeBuilder<EvaluacionProceso> b)
    {
        b.ToTable("evaluacion_proceso", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PlantillaId).HasColumnName("plantilla_id").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.Anno).HasColumnName("anno").IsRequired();
        b.Property(e => e.Periodo).HasColumnName("periodo").HasMaxLength(50);
        b.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        b.Property(e => e.FechaFin).HasColumnName("fecha_fin").IsRequired();
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
    }
}

public class EvaluacionConfiguration : IEntityTypeConfiguration<Evaluacion>
{
    public void Configure(EntityTypeBuilder<Evaluacion> b)
    {
        b.ToTable("evaluacion", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.ProcesoId).HasColumnName("proceso_id").IsRequired();
        b.Property(e => e.EvaluadoId).HasColumnName("evaluado_id").IsRequired();
        b.Property(e => e.EvaluadorId).HasColumnName("evaluador_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.TipoEvaluador).HasColumnName("tipo_evaluador").HasDefaultValue((short)1);
        b.Property(e => e.PuntuacionTotal).HasColumnName("puntuacion_total").HasColumnType("numeric(5,2)");
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.FechaCompletado).HasColumnName("fecha_completado");
        b.Property(e => e.ComentarioGeneral).HasColumnName("comentario_general").HasMaxLength(2000);

        b.HasOne(e => e.Evaluado).WithMany().HasForeignKey(e => e.EvaluadoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.Evaluador).WithMany().HasForeignKey(e => e.EvaluadorId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(e => e.Respuestas).WithOne(r => r.Evaluacion).HasForeignKey(r => r.EvaluacionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EvaluacionRespuestaConfiguration : IEntityTypeConfiguration<EvaluacionRespuesta>
{
    public void Configure(EntityTypeBuilder<EvaluacionRespuesta> b)
    {
        b.ToTable("evaluacion_respuesta", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EvaluacionId).HasColumnName("evaluacion_id").IsRequired();
        b.Property(e => e.CriterioId).HasColumnName("criterio_id").IsRequired();
        b.Property(e => e.Puntuacion).HasColumnName("puntuacion").HasColumnType("numeric(5,2)");
        b.Property(e => e.Comentario).HasColumnName("comentario").HasMaxLength(1000);
    }
}

// ─────────────────────────────────────────────────────────────────────
// CAPACITACIÓN
// ─────────────────────────────────────────────────────────────────────

public class CapacitacionConfiguration : IEntityTypeConfiguration<Capacitacion>
{
    public void Configure(EntityTypeBuilder<Capacitacion> b)
    {
        b.ToTable("capacitacion", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(1000);
        b.Property(e => e.Tipo).HasColumnName("tipo").HasDefaultValue((short)1);
        b.Property(e => e.Modalidad).HasColumnName("modalidad").HasDefaultValue((short)1);
        b.Property(e => e.Instructor).HasColumnName("instructor").HasMaxLength(200);
        b.Property(e => e.Institucion).HasColumnName("institucion").HasMaxLength(200);
        b.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
        b.Property(e => e.FechaFin).HasColumnName("fecha_fin").IsRequired();
        b.Property(e => e.HorasDuracion).HasColumnName("horas_duracion").HasColumnType("numeric(6,2)");
        b.Property(e => e.Costo).HasColumnName("costo").HasColumnType("numeric(18,2)");
        b.Property(e => e.MonedaId).HasColumnName("moneda_id");
        b.Property(e => e.Cupos).HasColumnName("cupos");
        b.Property(e => e.DescripcionCertificado).HasColumnName("descripcion_certificado").HasMaxLength(500);
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);

        b.HasMany(e => e.Participantes).WithOne(p => p.Capacitacion).HasForeignKey(p => p.CapacitacionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CapacitacionParticipanteConfiguration : IEntityTypeConfiguration<CapacitacionParticipante>
{
    public void Configure(EntityTypeBuilder<CapacitacionParticipante> b)
    {
        b.ToTable("capacitacion_participante", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.CapacitacionId).HasColumnName("capacitacion_id").IsRequired();
        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue((short)1);
        b.Property(e => e.AsistenciaPorcentaje).HasColumnName("asistencia_porcentaje").HasColumnType("numeric(5,2)");
        b.Property(e => e.Calificacion).HasColumnName("calificacion").HasColumnType("numeric(5,2)");
        b.Property(e => e.CertificadoUrl).HasColumnName("certificado_url").HasMaxLength(500);
        b.Property(e => e.FechaCertificado).HasColumnName("fecha_certificado");
        b.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(500);

        b.HasOne(e => e.Empleado).WithMany().HasForeignKey(e => e.EmpleadoId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ─────────────────────────────────────────────────────────────────────
// PARÁMETROS NÓMINA
// ─────────────────────────────────────────────────────────────────────

public class ParametroNominaConfiguration : IEntityTypeConfiguration<ParametroNomina>
{
    public void Configure(EntityTypeBuilder<ParametroNomina> b)
    {
        b.ToTable("parametro_nomina", "rrhh");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();

        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PaisCodigo).HasColumnName("pais_codigo").HasMaxLength(5).IsRequired();
        b.Property(e => e.Anno).HasColumnName("anno").IsRequired();
        b.Property(e => e.Clave).HasColumnName("clave").HasMaxLength(50).IsRequired();
        b.Property(e => e.Valor).HasColumnName("valor").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(300);
        b.Property(e => e.TipoDato).HasColumnName("tipo_dato").HasMaxLength(10).HasDefaultValue("DECIMAL");

        b.HasIndex(e => new { e.EmpresaId, e.PaisCodigo, e.Anno, e.Clave }).IsUnique();
    }
}