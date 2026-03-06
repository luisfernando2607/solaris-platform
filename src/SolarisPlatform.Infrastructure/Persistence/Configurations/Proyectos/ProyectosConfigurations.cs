using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.Proyectos;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations.Proyectos;

public class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> b)
    {
        b.ToTable("proyecto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.TipoProyecto).HasColumnName("tipo_proyecto").IsRequired();
        b.Property(e => e.Estado).HasColumnName("estado").IsRequired();
        b.Property(e => e.Prioridad).HasColumnName("prioridad").IsRequired();
        b.Property(e => e.FechaInicioPlan).HasColumnName("fecha_inicio_plan");
        b.Property(e => e.FechaFinPlan).HasColumnName("fecha_fin_plan");
        b.Property(e => e.FechaInicioReal).HasColumnName("fecha_inicio_real");
        b.Property(e => e.FechaFinReal).HasColumnName("fecha_fin_real");
        b.Property(e => e.MonedaId).HasColumnName("moneda_id");
        b.Property(e => e.PresupuestoTotal).HasColumnName("presupuesto_total").HasColumnType("decimal(18,2)");
        b.Property(e => e.CostoRealTotal).HasColumnName("costo_real_total").HasColumnType("decimal(18,2)");
        b.Property(e => e.PorcentajeAvancePlan).HasColumnName("porcentaje_avance_plan").HasColumnType("decimal(5,2)");
        b.Property(e => e.PorcentajeAvanceReal).HasColumnName("porcentaje_avance_real").HasColumnType("decimal(5,2)");
        b.Property(e => e.ClienteId).HasColumnName("cliente_id");
        b.Property(e => e.GerenteProyectoId).HasColumnName("gerente_proyecto_id");
        b.Property(e => e.ResponsableId).HasColumnName("responsable_id");
        b.Property(e => e.SucursalId).HasColumnName("sucursal_id");
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(500);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
    }
}

public class ProyectoHitoConfiguration : IEntityTypeConfiguration<ProyectoHito>
{
    public void Configure(EntityTypeBuilder<ProyectoHito> b)
    {
        b.ToTable("proyecto_hito", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.FechaCompromiso).HasColumnName("fecha_compromiso").IsRequired();
        b.Property(e => e.FechaReal).HasColumnName("fecha_real");
        b.Property(e => e.Estado).HasColumnName("estado").IsRequired();
        b.Property(e => e.PorcentajePeso).HasColumnName("porcentaje_peso").HasColumnType("decimal(5,2)");
        b.Property(e => e.ResponsableId).HasColumnName("responsable_id");
        b.Property(e => e.Orden).HasColumnName("orden");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Hitos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProyectoFaseConfiguration : IEntityTypeConfiguration<ProyectoFase>
{
    public void Configure(EntityTypeBuilder<ProyectoFase> b)
    {
        b.ToTable("proyecto_fase", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Orden).HasColumnName("orden");
        b.Property(e => e.FechaInicioPlan).HasColumnName("fecha_inicio_plan");
        b.Property(e => e.FechaFinPlan).HasColumnName("fecha_fin_plan");
        b.Property(e => e.FechaInicioReal).HasColumnName("fecha_inicio_real");
        b.Property(e => e.FechaFinReal).HasColumnName("fecha_fin_real");
        b.Property(e => e.PorcentajeAvance).HasColumnName("porcentaje_avance").HasColumnType("decimal(5,2)");
        b.Property(e => e.Estado).HasColumnName("estado").IsRequired();

        b.HasOne(e => e.Proyecto).WithMany(p => p.Fases).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProyectoDocumentoConfiguration : IEntityTypeConfiguration<ProyectoDocumento>
{
    public void Configure(EntityTypeBuilder<ProyectoDocumento> b)
    {
        b.ToTable("proyecto_documento", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.TipoDocumento).HasColumnName("tipo_documento").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.UrlStorage).HasColumnName("url_storage").HasMaxLength(1000).IsRequired();
        b.Property(e => e.NombreArchivoOriginal).HasColumnName("nombre_archivo_original").HasMaxLength(300);
        b.Property(e => e.Extension).HasColumnName("extension").HasMaxLength(10);
        b.Property(e => e.TamanoBytes).HasColumnName("tamano_bytes");
        b.Property(e => e.SubidoPorId).HasColumnName("subido_por_id");
        b.Property(e => e.FechaSubida).HasColumnName("fecha_subida");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Documentos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class WbsNodoConfiguration : IEntityTypeConfiguration<WbsNodo>
{
    public void Configure(EntityTypeBuilder<WbsNodo> b)
    {
        b.ToTable("wbs_nodo", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.FaseId).HasColumnName("fase_id");
        b.Property(e => e.PadreId).HasColumnName("padre_id");
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.TipoNodo).HasColumnName("tipo_nodo").IsRequired();
        b.Property(e => e.Nivel).HasColumnName("nivel");
        b.Property(e => e.Orden).HasColumnName("orden");
        b.Property(e => e.PorcentajeAvance).HasColumnName("porcentaje_avance").HasColumnType("decimal(5,2)");
        b.Property(e => e.PorcentajePeso).HasColumnName("porcentaje_peso").HasColumnType("decimal(5,2)");
        b.Property(e => e.EsHoja).HasColumnName("es_hoja");

        b.HasOne(e => e.Proyecto).WithMany(p => p.WbsNodos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Fase).WithMany(f => f.WbsNodos).HasForeignKey(e => e.FaseId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Padre).WithMany(n => n.Hijos).HasForeignKey(e => e.PadreId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
{
    public void Configure(EntityTypeBuilder<Tarea> b)
    {
        b.ToTable("tarea", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.WbsNodoId).HasColumnName("wbs_nodo_id");
        b.Property(e => e.FaseId).HasColumnName("fase_id");
        b.Property(e => e.CuadrillaId).HasColumnName("cuadrilla_id");
        b.Property(e => e.ResponsableId).HasColumnName("responsable_id");
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Estado).HasColumnName("estado").IsRequired();
        b.Property(e => e.Prioridad).HasColumnName("prioridad").IsRequired();
        b.Property(e => e.FechaInicioPlan).HasColumnName("fecha_inicio_plan");
        b.Property(e => e.FechaFinPlan).HasColumnName("fecha_fin_plan");
        b.Property(e => e.FechaInicioReal).HasColumnName("fecha_inicio_real");
        b.Property(e => e.FechaFinReal).HasColumnName("fecha_fin_real");
        b.Property(e => e.DuracionDias).HasColumnName("duracion_dias").HasColumnType("decimal(8,2)");
        b.Property(e => e.HorasEstimadas).HasColumnName("horas_estimadas").HasColumnType("decimal(8,2)");
        b.Property(e => e.HorasReales).HasColumnName("horas_reales").HasColumnType("decimal(8,2)");
        b.Property(e => e.PorcentajeAvance).HasColumnName("porcentaje_avance").HasColumnType("decimal(5,2)");
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Ubicacion).HasColumnName("ubicacion").HasMaxLength(500);

        b.HasOne(e => e.Proyecto).WithMany().HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.WbsNodo).WithMany(n => n.Tareas).HasForeignKey(e => e.WbsNodoId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Fase).WithMany().HasForeignKey(e => e.FaseId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Cuadrilla).WithMany(c => c.Tareas).HasForeignKey(e => e.CuadrillaId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class TareaDependenciaConfiguration : IEntityTypeConfiguration<TareaDependencia>
{
    public void Configure(EntityTypeBuilder<TareaDependencia> b)
    {
        b.ToTable("tarea_dependencia", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.TareaOrigenId).HasColumnName("tarea_origen_id").IsRequired();
        b.Property(e => e.TareaDestinoId).HasColumnName("tarea_destino_id").IsRequired();
        b.Property(e => e.TipoDependencia).HasColumnName("tipo_dependencia").IsRequired();
        b.Property(e => e.Desfase).HasColumnName("desfase");

        // Configuración explícita de las dos FK a la misma tabla
        b.HasOne(e => e.TareaOrigen).WithMany(t => t.DependenciasOrigen)
            .HasForeignKey(e => e.TareaOrigenId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.TareaDestino).WithMany(t => t.DependenciasDestino)
            .HasForeignKey(e => e.TareaDestinoId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CuadrillaConfiguration : IEntityTypeConfiguration<Cuadrilla>
{
    public void Configure(EntityTypeBuilder<Cuadrilla> b)
    {
        b.ToTable("cuadrilla", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.LiderId).HasColumnName("lider_id");
        b.Property(e => e.CapacidadMax).HasColumnName("capacidad_max");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Cuadrillas).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CuadrillaMiembroConfiguration : IEntityTypeConfiguration<CuadrillaMiembro>
{
    public void Configure(EntityTypeBuilder<CuadrillaMiembro> b)
    {
        b.ToTable("cuadrilla_miembro", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.CuadrillaId).HasColumnName("cuadrilla_id").IsRequired();
        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id").IsRequired();
        b.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso").IsRequired();
        b.Property(e => e.FechaSalida).HasColumnName("fecha_salida");
        b.Property(e => e.Rol).HasColumnName("rol").HasMaxLength(100);

        b.HasOne(e => e.Cuadrilla).WithMany(c => c.Miembros).HasForeignKey(e => e.CuadrillaId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RecursoProyectoConfiguration : IEntityTypeConfiguration<RecursoProyecto>
{
    public void Configure(EntityTypeBuilder<RecursoProyecto> b)
    {
        b.ToTable("recurso_proyecto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.TareaId).HasColumnName("tarea_id");
        b.Property(e => e.TipoRecurso).HasColumnName("tipo_recurso").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(50);
        b.Property(e => e.UnidadMedida).HasColumnName("unidad_medida").IsRequired();
        b.Property(e => e.CantidadPlan).HasColumnName("cantidad_plan").HasColumnType("decimal(12,2)");
        b.Property(e => e.CantidadReal).HasColumnName("cantidad_real").HasColumnType("decimal(12,2)");
        b.Property(e => e.CostoUnitario).HasColumnName("costo_unitario").HasColumnType("decimal(18,2)");
        b.Property(e => e.CostoTotalPlan).HasColumnName("costo_total_plan").HasColumnType("decimal(18,2)");
        b.Property(e => e.CostoTotalReal).HasColumnName("costo_total_real").HasColumnType("decimal(18,2)");
        b.Property(e => e.EmpleadoId).HasColumnName("empleado_id");
        b.Property(e => e.ProveedorId).HasColumnName("proveedor_id");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Recursos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Tarea).WithMany().HasForeignKey(e => e.TareaId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class PresupuestoConfiguration : IEntityTypeConfiguration<Presupuesto>
{
    public void Configure(EntityTypeBuilder<Presupuesto> b)
    {
        b.ToTable("presupuesto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.Version).HasColumnName("version");
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.TotalIngresos).HasColumnName("total_ingresos").HasColumnType("decimal(18,2)");
        b.Property(e => e.TotalEgresos).HasColumnName("total_egresos").HasColumnType("decimal(18,2)");
        b.Property(e => e.Contingencia).HasColumnName("contingencia").HasColumnType("decimal(18,2)");
        b.Property(e => e.TotalNeto).HasColumnName("total_neto").HasColumnType("decimal(18,2)");
        b.Property(e => e.EsActivo).HasColumnName("es_activo");
        b.Property(e => e.EsAprobado).HasColumnName("es_aprobado");
        b.Property(e => e.AprobadoPorId).HasColumnName("aprobado_por_id");
        b.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Presupuestos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PresupuestoPartidaConfiguration : IEntityTypeConfiguration<PresupuestoPartida>
{
    public void Configure(EntityTypeBuilder<PresupuestoPartida> b)
    {
        b.ToTable("presupuesto_partida", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PresupuestoId).HasColumnName("presupuesto_id").IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo").IsRequired();
        b.Property(e => e.Concepto).HasColumnName("concepto").HasMaxLength(300).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.CodigoContable).HasColumnName("codigo_contable").HasMaxLength(50);
        b.Property(e => e.Cantidad).HasColumnName("cantidad").HasColumnType("decimal(12,2)");
        b.Property(e => e.UnidadMedida).HasColumnName("unidad_medida").HasMaxLength(50);
        b.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("decimal(18,2)");
        b.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(18,2)");
        b.Property(e => e.Porcentaje).HasColumnName("porcentaje").HasColumnType("decimal(5,2)");
        b.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(18,2)");
        b.Property(e => e.Orden).HasColumnName("orden");

        b.HasOne(e => e.Presupuesto).WithMany(p => p.Partidas).HasForeignKey(e => e.PresupuestoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CostoRealConfiguration : IEntityTypeConfiguration<CostoReal>
{
    public void Configure(EntityTypeBuilder<CostoReal> b)
    {
        b.ToTable("costo_real", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PresupuestoId).HasColumnName("presupuesto_id").IsRequired();
        b.Property(e => e.PartidaId).HasColumnName("partida_id").IsRequired();
        b.Property(e => e.Origen).HasColumnName("origen").IsRequired();
        b.Property(e => e.Concepto).HasColumnName("concepto").HasMaxLength(300).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Monto).HasColumnName("monto").HasColumnType("decimal(18,2)");
        b.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");
        b.Property(e => e.NumeroReferencia).HasColumnName("numero_referencia").HasMaxLength(100);
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id");
        b.Property(e => e.RegistradoPorId).HasColumnName("registrado_por_id");

        b.HasOne(e => e.Presupuesto).WithMany().HasForeignKey(e => e.PresupuestoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.Partida).WithMany(p => p.CostosReales).HasForeignKey(e => e.PartidaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.OrdenTrabajo).WithMany(o => o.CostosReales).HasForeignKey(e => e.OrdenTrabajoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class GanttLineaBaseConfiguration : IEntityTypeConfiguration<GanttLineaBase>
{
    public void Configure(EntityTypeBuilder<GanttLineaBase> b)
    {
        b.ToTable("gantt_linea_base", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.TareaId).HasColumnName("tarea_id");
        b.Property(e => e.FaseId).HasColumnName("fase_id");
        b.Property(e => e.FechaCaptura).HasColumnName("fecha_captura");
        b.Property(e => e.FechaInicioBase).HasColumnName("fecha_inicio_base").IsRequired();
        b.Property(e => e.FechaFinBase).HasColumnName("fecha_fin_base").IsRequired();
        b.Property(e => e.DuracionDias).HasColumnName("duracion_dias").HasColumnType("decimal(8,2)");
        b.Property(e => e.CostoBase).HasColumnName("costo_base").HasColumnType("decimal(18,2)");
        b.Property(e => e.Descripcion).HasColumnName("descripcion");

        b.HasOne(e => e.Proyecto).WithMany(p => p.GanttLineas).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Tarea).WithMany().HasForeignKey(e => e.TareaId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Fase).WithMany().HasForeignKey(e => e.FaseId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class GanttProgresoConfiguration : IEntityTypeConfiguration<GanttProgreso>
{
    public void Configure(EntityTypeBuilder<GanttProgreso> b)
    {
        b.ToTable("gantt_progreso", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.TareaId).HasColumnName("tarea_id").IsRequired();
        b.Property(e => e.FechaProgreso).HasColumnName("fecha_progreso").IsRequired();
        b.Property(e => e.PorcentajeAvance).HasColumnName("porcentaje_avance").HasColumnType("decimal(5,2)");
        b.Property(e => e.HorasTrabajadas).HasColumnName("horas_trabajadas").HasColumnType("decimal(8,2)");
        b.Property(e => e.Observaciones).HasColumnName("observaciones");
        b.Property(e => e.ReportadoPorId).HasColumnName("reportado_por_id");

        b.HasOne(e => e.Tarea).WithMany(t => t.GanttProgresos).HasForeignKey(e => e.TareaId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CentroCostoConfiguration : IEntityTypeConfiguration<CentroCosto>
{
    public void Configure(EntityTypeBuilder<CentroCosto> b)
    {
        b.ToTable("centro_costo", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.PresupuestoAsignado).HasColumnName("presupuesto_asignado").HasColumnType("decimal(18,2)");
        b.Property(e => e.GastoActual).HasColumnName("gasto_actual").HasColumnType("decimal(18,2)");

        b.HasOne(e => e.Proyecto).WithMany(p => p.CentrosCosto).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AsignacionCentroCostoConfiguration : IEntityTypeConfiguration<AsignacionCentroCosto>
{
    public void Configure(EntityTypeBuilder<AsignacionCentroCosto> b)
    {
        b.ToTable("asignacion_centro_costo", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.CentroCostoId).HasColumnName("centro_costo_id").IsRequired();
        b.Property(e => e.CostoRealId).HasColumnName("costo_real_id");
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id");
        b.Property(e => e.Porcentaje).HasColumnName("porcentaje").HasColumnType("decimal(5,2)");
        b.Property(e => e.Monto).HasColumnName("monto").HasColumnType("decimal(18,2)");
        b.Property(e => e.Concepto).HasColumnName("concepto").HasMaxLength(300);
        b.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");

        b.HasOne(e => e.CentroCosto).WithMany(c => c.Asignaciones).HasForeignKey(e => e.CentroCostoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.CostoReal).WithMany().HasForeignKey(e => e.CostoRealId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.OrdenTrabajo).WithMany(o => o.Asignaciones).HasForeignKey(e => e.OrdenTrabajoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class OrdenTrabajoConfiguration : IEntityTypeConfiguration<OrdenTrabajo>
{
    public void Configure(EntityTypeBuilder<OrdenTrabajo> b)
    {
        b.ToTable("orden_trabajo", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.TareaId).HasColumnName("tarea_id");
        b.Property(e => e.CuadrillaId).HasColumnName("cuadrilla_id");
        b.Property(e => e.Numero).HasColumnName("numero").HasMaxLength(50).IsRequired();
        b.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(300).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Estado).HasColumnName("estado").IsRequired();
        b.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");
        b.Property(e => e.FechaInicioPlan).HasColumnName("fecha_inicio_plan");
        b.Property(e => e.FechaFinPlan).HasColumnName("fecha_fin_plan");
        b.Property(e => e.FechaInicioReal).HasColumnName("fecha_inicio_real");
        b.Property(e => e.FechaFinReal).HasColumnName("fecha_fin_real");
        b.Property(e => e.AsignadoPorId).HasColumnName("asignado_por_id");
        b.Property(e => e.TecnicoResponsableId).HasColumnName("tecnico_responsable_id");
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.DireccionSitio).HasColumnName("direccion_sitio").HasMaxLength(500);
        b.Property(e => e.RequiereFirma).HasColumnName("requiere_firma");
        b.Property(e => e.FirmaBase64).HasColumnName("firma_base64");
        b.Property(e => e.FechaFirma).HasColumnName("fecha_firma");
        b.Property(e => e.FirmadoPorNombre).HasColumnName("firmado_por_nombre").HasMaxLength(200);
        b.Property(e => e.RequiereFotos).HasColumnName("requiere_fotos");
        b.Property(e => e.FotosRequeridas).HasColumnName("fotos_requeridas");
        b.Property(e => e.ObservacionesTecnico).HasColumnName("observaciones_tecnico");
        b.Property(e => e.ObservacionesSupervisor).HasColumnName("observaciones_supervisor");

        b.HasOne(e => e.Proyecto).WithMany(p => p.OrdenesTrabajo).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Tarea).WithMany().HasForeignKey(e => e.TareaId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Cuadrilla).WithMany().HasForeignKey(e => e.CuadrillaId).OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(e => new { e.EmpresaId, e.Numero }).IsUnique();
    }
}

public class OtActividadConfiguration : IEntityTypeConfiguration<OtActividad>
{
    public void Configure(EntityTypeBuilder<OtActividad> b)
    {
        b.ToTable("ot_actividad", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(300).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Orden).HasColumnName("orden");
        b.Property(e => e.Completada).HasColumnName("completada");
        b.Property(e => e.FechaComplecion).HasColumnName("fecha_complecion");
        b.Property(e => e.CompletadaPorId).HasColumnName("completada_por_id");
        b.Property(e => e.Observaciones).HasColumnName("observaciones");

        b.HasOne(e => e.OrdenTrabajo).WithMany(o => o.Actividades).HasForeignKey(e => e.OrdenTrabajoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class OtMaterialConfiguration : IEntityTypeConfiguration<OtMaterial>
{
    public void Configure(EntityTypeBuilder<OtMaterial> b)
    {
        b.ToTable("ot_material", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id").IsRequired();
        b.Property(e => e.NombreMaterial).HasColumnName("nombre_material").HasMaxLength(300).IsRequired();
        b.Property(e => e.CodigoMaterial).HasColumnName("codigo_material").HasMaxLength(50);
        b.Property(e => e.UnidadMedida).HasColumnName("unidad_medida").HasMaxLength(50);
        b.Property(e => e.CantidadPlan).HasColumnName("cantidad_plan").HasColumnType("decimal(12,2)");
        b.Property(e => e.CantidadReal).HasColumnName("cantidad_real").HasColumnType("decimal(12,2)");
        b.Property(e => e.CostoUnitario).HasColumnName("costo_unitario").HasColumnType("decimal(18,2)");
        b.Property(e => e.CostoTotal).HasColumnName("costo_total").HasColumnType("decimal(18,2)");
        b.Property(e => e.ProductoId).HasColumnName("producto_id");

        b.HasOne(e => e.OrdenTrabajo).WithMany(o => o.Materiales).HasForeignKey(e => e.OrdenTrabajoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReporteAvanceConfiguration : IEntityTypeConfiguration<ReporteAvance>
{
    public void Configure(EntityTypeBuilder<ReporteAvance> b)
    {
        b.ToTable("reporte_avance", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id");
        b.Property(e => e.FechaReporte).HasColumnName("fecha_reporte").IsRequired();
        b.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(300).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.PorcentajeAvancePlan).HasColumnName("porcentaje_avance_plan").HasColumnType("decimal(5,2)");
        b.Property(e => e.PorcentajeAvanceReal).HasColumnName("porcentaje_avance_real").HasColumnType("decimal(5,2)");
        b.Property(e => e.Bac).HasColumnName("bac").HasColumnType("decimal(18,2)");
        b.Property(e => e.Ev).HasColumnName("ev").HasColumnType("decimal(18,2)");
        b.Property(e => e.Pv).HasColumnName("pv").HasColumnType("decimal(18,2)");
        b.Property(e => e.Ac).HasColumnName("ac").HasColumnType("decimal(18,2)");
        b.Property(e => e.Cpi).HasColumnName("cpi").HasColumnType("decimal(8,4)");
        b.Property(e => e.Spi).HasColumnName("spi").HasColumnType("decimal(8,4)");
        b.Property(e => e.ReportadoPorId).HasColumnName("reportado_por_id");
        b.Property(e => e.Observaciones).HasColumnName("observaciones");
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Reportes).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.OrdenTrabajo).WithMany(o => o.Reportes).HasForeignKey(e => e.OrdenTrabajoId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class ReporteAvanceFotoConfiguration : IEntityTypeConfiguration<ReporteAvanceFoto>
{
    public void Configure(EntityTypeBuilder<ReporteAvanceFoto> b)
    {
        b.ToTable("reporte_avance_foto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ReporteId).HasColumnName("reporte_id").IsRequired();
        b.Property(e => e.UrlStorage).HasColumnName("url_storage").HasMaxLength(1000).IsRequired();
        b.Property(e => e.NombreArchivo).HasColumnName("nombre_archivo").HasMaxLength(300);
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.FechaFoto).HasColumnName("fecha_foto");
        b.Property(e => e.Orden).HasColumnName("orden");

        b.HasOne(e => e.Reporte).WithMany(r => r.Fotos).HasForeignKey(e => e.ReporteId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class KpiProyectoConfiguration : IEntityTypeConfiguration<KpiProyecto>
{
    public void Configure(EntityTypeBuilder<KpiProyecto> b)
    {
        b.ToTable("kpi_proyecto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.TipoKpi).HasColumnName("tipo_kpi").IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,4)");
        b.Property(e => e.ValorMeta).HasColumnName("valor_meta").HasColumnType("decimal(18,4)");
        b.Property(e => e.ValorMinimo).HasColumnName("valor_minimo").HasColumnType("decimal(18,4)");
        b.Property(e => e.Unidad).HasColumnName("unidad").HasMaxLength(20);
        b.Property(e => e.FechaCalculo).HasColumnName("fecha_calculo");
        b.Property(e => e.Formula).HasColumnName("formula").HasMaxLength(500);
        b.Property(e => e.Observaciones).HasColumnName("observaciones");

        b.HasOne(e => e.Proyecto).WithMany(p => p.Kpis).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AlertaProyectoConfiguration : IEntityTypeConfiguration<AlertaProyecto>
{
    public void Configure(EntityTypeBuilder<AlertaProyecto> b)
    {
        b.ToTable("alerta_proyecto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.ProyectoId).HasColumnName("proyecto_id").IsRequired();
        b.Property(e => e.TipoAlerta).HasColumnName("tipo_alerta").IsRequired();
        b.Property(e => e.Severidad).HasColumnName("severidad").IsRequired();
        b.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(300).IsRequired();
        b.Property(e => e.Mensaje).HasColumnName("mensaje").IsRequired();
        b.Property(e => e.Leida).HasColumnName("leida");
        b.Property(e => e.Resuelta).HasColumnName("resuelta");
        b.Property(e => e.FechaAlerta).HasColumnName("fecha_alerta");
        b.Property(e => e.FechaLeida).HasColumnName("fecha_leida");
        b.Property(e => e.FechaResuelta).HasColumnName("fecha_resuelta");
        b.Property(e => e.UsuarioId).HasColumnName("usuario_id");
        b.Property(e => e.UrlReferencia).HasColumnName("url_referencia").HasMaxLength(500);

        b.HasOne(e => e.Proyecto).WithMany(p => p.Alertas).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}
