using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarisPlatform.Domain.Entities.Proyectos;

namespace SolarisPlatform.Infrastructure.Persistence.Configurations.Proyectos;

// ─── Sin cambios ────────────────────────────────────────────────────────────
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

// ─── FIX #1: WbsNodo — EsHoja ignorada (columna no existe en BD) ─────────────
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
        b.Property(e => e.PadreId).HasColumnName("nodo_padre_id");
        b.Property(e => e.CodigoWbs).HasColumnName("codigo_wbs").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.TipoNodo).HasColumnName("tipo_nodo").IsRequired();
        b.Property(e => e.Nivel).HasColumnName("nivel");
        b.Property(e => e.Orden).HasColumnName("orden");
        b.Property(e => e.PorcentajeAvance).HasColumnName("porcentaje_avance").HasColumnType("decimal(5,2)");
        b.Property(e => e.PesoRelativo).HasColumnName("peso_relativo").HasColumnType("decimal(5,2)");
        // FIX #1: es_hoja NO existe en BD — se calcula en memoria como !Hijos.Any()
        b.Ignore(e => e.EsHoja);

        b.HasOne(e => e.Proyecto).WithMany(p => p.WbsNodos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Fase).WithMany(f => f.WbsNodos).HasForeignKey(e => e.FaseId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Padre).WithMany(n => n.Hijos).HasForeignKey(e => e.PadreId).OnDelete(DeleteBehavior.Restrict);
    }
}

// ─── FIX: Tarea — columnas corregidas ───────────────────────────────────────
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
        b.Property(e => e.DuracionDiasPlan).HasColumnName("duracion_dias_plan");
        b.Property(e => e.DuracionDiasReal).HasColumnName("duracion_dias_real");
        b.Property(e => e.PorcentajeAvance).HasColumnName("porcentaje_avance").HasColumnType("decimal(5,2)");

        b.HasOne(e => e.Proyecto).WithMany().HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.WbsNodo).WithMany(n => n.Tareas).HasForeignKey(e => e.WbsNodoId).OnDelete(DeleteBehavior.SetNull);
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
        b.HasOne(e => e.TareaOrigen).WithMany(t => t.DependenciasOrigen).HasForeignKey(e => e.TareaOrigenId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.TareaDestino).WithMany(t => t.DependenciasDestino).HasForeignKey(e => e.TareaDestinoId).OnDelete(DeleteBehavior.Restrict);
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
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
        b.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.LiderId).HasColumnName("lider_id");
        b.Property(e => e.CapacidadMaxima).HasColumnName("capacidad_maxima");
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
        b.Property(e => e.FechaIngreso).HasColumnName("fecha_asignacion").IsRequired();
        b.Property(e => e.FechaSalida).HasColumnName("fecha_liberacion");
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
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.Estado).HasColumnName("estado");
        b.Property(e => e.FechaAprobacion).HasColumnName("fecha_aprobacion");
        b.Property(e => e.AprobadoPorId).HasColumnName("aprobado_por_id");
        b.Property(e => e.MontoTotalManoObra).HasColumnName("monto_total_mano_obra").HasColumnType("decimal(18,2)");
        b.Property(e => e.MontoTotalMateriales).HasColumnName("monto_total_materiales").HasColumnType("decimal(18,2)");
        b.Property(e => e.MontoTotalSubcontratos).HasColumnName("monto_total_subcontratos").HasColumnType("decimal(18,2)");
        b.Property(e => e.MontoTotalEquipos).HasColumnName("monto_total_equipos").HasColumnType("decimal(18,2)");
        b.Property(e => e.MontoTotalIndirectos).HasColumnName("monto_total_indirectos").HasColumnType("decimal(18,2)");
        b.Property(e => e.TotalGeneral).HasColumnName("total_general").HasColumnType("decimal(18,2)");
        b.Property(e => e.Observaciones).HasColumnName("observaciones");
        b.HasOne(e => e.Proyecto).WithMany(p => p.Presupuestos).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ─── FIX #2: PresupuestoPartida — mapear propiedades de entidad a columnas BD ─
// Entidad (nombres C#) → BD (columnas reales)
// Tipo           → tipo_partida
// Concepto       → descripcion  (campo más cercano semánticamente)
// CodigoContable → IGNORAR (no existe en BD)
// UnidadMedida   → unidad
// Cantidad       → cantidad_plan
// PrecioUnitario → precio_unitario
// Subtotal       → monto_plan
// Porcentaje     → porcentaje_variacion
// Total          → monto_real
public class PresupuestoPartidaConfiguration : IEntityTypeConfiguration<PresupuestoPartida>
{
    public void Configure(EntityTypeBuilder<PresupuestoPartida> b)
    {
        b.ToTable("presupuesto_partida", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        b.Property(e => e.PresupuestoId).HasColumnName("presupuesto_id").IsRequired();
        b.Property(e => e.Tipo).HasColumnName("tipo_partida").IsRequired();
        b.Property(e => e.Concepto).HasColumnName("descripcion").HasMaxLength(500).IsRequired();
        // FIX: Descripcion existe en entidad pero la BD no tiene esa columna
        // "descripcion" ya está ocupada por Concepto — ignorar para que EF no genere "p.Descripcion"
        b.Ignore(e => e.Descripcion);
        b.Ignore(e => e.CodigoContable);
        b.Property(e => e.UnidadMedida).HasColumnName("unidad").HasMaxLength(50);
        b.Property(e => e.Cantidad).HasColumnName("cantidad_plan").HasColumnType("decimal(18,4)");
        b.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("decimal(18,4)");
        b.Property(e => e.Subtotal).HasColumnName("monto_plan").HasColumnType("decimal(18,4)");
        b.Property(e => e.Porcentaje).HasColumnName("porcentaje_variacion").HasColumnType("decimal(7,2)");
        b.Property(e => e.Total).HasColumnName("monto_real").HasColumnType("decimal(18,4)");
        b.Property(e => e.Orden).HasColumnName("orden");
        // Descripcion en entidad no existe — BD tiene descripcion mapeada a Concepto arriba
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
        b.Property(e => e.OrigenId).HasColumnName("origen_id");
        b.Property(e => e.Concepto).HasColumnName("concepto").HasMaxLength(300).IsRequired();
        b.Property(e => e.Monto).HasColumnName("monto").HasColumnType("decimal(18,2)");
        b.Property(e => e.Fecha).HasColumnName("fecha");
        b.Property(e => e.RegistradoPorId).HasColumnName("registrado_por_id");
        b.Property(e => e.Observaciones).HasColumnName("observaciones");
        b.HasOne(e => e.Presupuesto).WithMany().HasForeignKey(e => e.PresupuestoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(e => e.Partida).WithMany(p => p.CostosReales).HasForeignKey(e => e.PartidaId).OnDelete(DeleteBehavior.Restrict);
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
        b.Property(e => e.Tipo).HasColumnName("tipo");
        b.Property(e => e.ResponsableId).HasColumnName("responsable_id");
        b.Property(e => e.PresupuestoAnual).HasColumnName("presupuesto_anual").HasColumnType("decimal(18,2)");
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
        b.Property(e => e.TipoOrigen).HasColumnName("tipo_origen").IsRequired();
        b.Property(e => e.OrigenId).HasColumnName("origen_id").IsRequired();
        b.Property(e => e.Monto).HasColumnName("monto").HasColumnType("decimal(18,2)").IsRequired();
        b.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.RegistradoPorId).HasColumnName("registrado_por_id");
        b.HasOne(e => e.CentroCosto).WithMany(c => c.Asignaciones).HasForeignKey(e => e.CentroCostoId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ─── FIX: OrdenTrabajo — estructura completamente corregida ─────────────────
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
        b.Property(e => e.PresupuestoPartidaId).HasColumnName("presupuesto_partida_id");
        b.Property(e => e.CuadrillaId).HasColumnName("cuadrilla_id");
        b.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(30).IsRequired();
        b.Property(e => e.Descripcion).HasColumnName("descripcion");
        b.Property(e => e.TipoOt).HasColumnName("tipo_ot");
        b.Property(e => e.Estado).HasColumnName("estado").IsRequired();
        b.Property(e => e.Prioridad).HasColumnName("prioridad");
        b.Property(e => e.TecnicoAsignadoId).HasColumnName("tecnico_asignado_id");
        b.Property(e => e.FechaProgramada).HasColumnName("fecha_programada");
        b.Property(e => e.FechaInicioEjecucion).HasColumnName("fecha_inicio_ejecucion");
        b.Property(e => e.FechaFinEjecucion).HasColumnName("fecha_fin_ejecucion");
        b.Property(e => e.TiempoEjecucionMin).HasColumnName("tiempo_ejecucion_min");
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(500);
        // FIX: primer_intento es nullable en BD (NULL = Pendiente)
        b.Property(e => e.PrimerIntento).HasColumnName("primer_intento");
        b.Property(e => e.UrlFirmaDigital).HasColumnName("url_firma_digital").HasMaxLength(1000);
        b.Property(e => e.ObservacionesTecnico).HasColumnName("observaciones_tecnico");
        b.Property(e => e.ObservacionesCierre).HasColumnName("observaciones_cierre");

        b.HasOne(e => e.Proyecto).WithMany(p => p.OrdenesTrabajo).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Tarea).WithMany().HasForeignKey(e => e.TareaId).OnDelete(DeleteBehavior.SetNull);
        b.HasOne(e => e.Cuadrilla).WithMany().HasForeignKey(e => e.CuadrillaId).OnDelete(DeleteBehavior.SetNull);
        // ReporteAvance pertenece a Proyecto, NO a OrdenTrabajo — ignorar esta colección
        b.Ignore(e => e.Reportes);

        b.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
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
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500).IsRequired();
        b.Property(e => e.Orden).HasColumnName("orden");
        b.Property(e => e.Completado).HasColumnName("completado");
        b.Property(e => e.FechaCompletado).HasColumnName("fecha_completado");
        b.Property(e => e.ObservacionTecnico).HasColumnName("observacion_tecnico");
        b.Property(e => e.TipoActividad).HasColumnName("tipo_actividad");
        b.Property(e => e.RequiereFoto).HasColumnName("requiere_foto");
        b.Property(e => e.UrlFoto).HasColumnName("url_foto").HasMaxLength(1000);
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id").IsRequired();
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
        b.Property(e => e.NombreMaterial).HasColumnName("nombre_material").HasMaxLength(300).IsRequired();
        b.Property(e => e.CodigoMaterial).HasColumnName("codigo_material").HasMaxLength(50);
        b.Property(e => e.UnidadMedida).HasColumnName("unidad_medida").HasMaxLength(50);
        b.Property(e => e.CantidadPlan).HasColumnName("cantidad_plan").HasColumnType("decimal(12,2)");
        b.Property(e => e.CantidadReal).HasColumnName("cantidad_real").HasColumnType("decimal(12,2)");
        b.Property(e => e.CostoUnitario).HasColumnName("costo_unitario").HasColumnType("decimal(18,2)");
        b.Property(e => e.CostoTotal).HasColumnName("costo_total").HasColumnType("decimal(18,2)");
        b.Property(e => e.ProductoId).HasColumnName("producto_id");
        b.Property(e => e.OrdenTrabajoId).HasColumnName("orden_trabajo_id").IsRequired();
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
        b.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(300).IsRequired();
        b.Property(e => e.FechaReporte).HasColumnName("fecha_reporte").IsRequired();
        b.Property(e => e.CreadoPorId).HasColumnName("creado_por_id");
        b.Property(e => e.AvanceGeneral).HasColumnName("avance_general").HasColumnType("decimal(5,2)");
        b.Property(e => e.AvanceCosto).HasColumnName("avance_costo").HasColumnType("decimal(5,2)");
        b.Property(e => e.Observaciones).HasColumnName("observaciones");
        b.Property(e => e.ProximasActividades).HasColumnName("proximas_actividades");
        b.Property(e => e.RiesgosIdentificados).HasColumnName("riesgos_identificados");
        b.Property(e => e.Conclusiones).HasColumnName("conclusiones");
        b.Property(e => e.EnviadoACliente).HasColumnName("enviado_a_cliente");
        b.Property(e => e.FechaEnvio).HasColumnName("fecha_envio");
        b.HasOne(e => e.Proyecto).WithMany(p => p.Reportes).HasForeignKey(e => e.ProyectoId).OnDelete(DeleteBehavior.Cascade);
    }
}

// ─── FIX #4: ReporteAvanceFoto — usar propiedades reales de entidad ───────────
// Entidad tiene: ReporteId, UrlStorage, NombreArchivo, Descripcion,
//               Latitud, Longitud, FechaFoto, Orden
// BD tiene:      reporte_avance_id, tarea_id, url_foto, descripcion,
//               latitud, longitud, fecha_captura, subido_por_id, orden
public class ReporteAvanceFotoConfiguration : IEntityTypeConfiguration<ReporteAvanceFoto>
{
    public void Configure(EntityTypeBuilder<ReporteAvanceFoto> b)
    {
        b.ToTable("reporte_avance_foto", "proy");
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
        b.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        // ReporteId → reporte_avance_id
        b.Property(e => e.ReporteId).HasColumnName("reporte_avance_id").IsRequired();
        // UrlStorage → url_foto
        b.Property(e => e.UrlStorage).HasColumnName("url_foto").HasMaxLength(1000).IsRequired();
        // NombreArchivo → nombre_archivo (no existe en BD → ignorar)
        b.Ignore(e => e.NombreArchivo);
        b.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
        b.Property(e => e.Latitud).HasColumnName("latitud").HasColumnType("decimal(10,7)");
        b.Property(e => e.Longitud).HasColumnName("longitud").HasColumnType("decimal(10,7)");
        // FechaFoto → fecha_captura
        b.Property(e => e.FechaFoto).HasColumnName("fecha_captura");
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