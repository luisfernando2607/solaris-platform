using SolarisPlatform.Domain.Common;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 5: GANTT
// Tablas: gantt_linea_base, gantt_progreso
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Línea base del Gantt. Snapshot de planificación en un momento dado.
/// Permite comparar plan original vs. realidad a lo largo del tiempo.
/// </summary>
public class GanttLineaBase : BaseEntity
{
    public long      EmpresaId        { get; set; }
    public long      ProyectoId       { get; set; }
    public long?     TareaId          { get; set; }
    public long?     FaseId           { get; set; }
    public DateTime  FechaCaptura     { get; set; } = DateTime.UtcNow;
    public DateOnly  FechaInicioBase  { get; set; }
    public DateOnly  FechaFinBase     { get; set; }
    public decimal   DuracionDias     { get; set; }
    public decimal   CostoBase        { get; set; }
    public string?   Descripcion      { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto      Proyecto { get; set; } = null!;
    public virtual Tarea?        Tarea    { get; set; }
    public virtual ProyectoFase? Fase     { get; set; }
}

/// <summary>
/// Progreso del Gantt por fecha. Registro diario/semanal del avance real de una tarea.
/// Permite construir la curva S y calcular métricas EVM.
/// </summary>
public class GanttProgreso : BaseEntity
{
    public long    EmpresaId           { get; set; }
    public long    TareaId             { get; set; }
    public DateOnly FechaProgreso      { get; set; }
    public decimal PorcentajeAvance    { get; set; }
    public decimal HorasTrabajadas     { get; set; }
    public string? Observaciones       { get; set; }
    public long?   ReportadoPorId      { get; set; }   // FK seg.usuario

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Tarea Tarea         { get; set; } = null!;
}

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 6: CENTROS DE COSTO
// Tablas: centro_costo, asignacion_centro_costo
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Centro de costo del proyecto. Agrupa gastos e ingresos por área/categoría.
/// </summary>
public class CentroCosto : BaseEntity
{
    public long   EmpresaId      { get; set; }
    public long   ProyectoId     { get; set; }
    public string Codigo         { get; set; } = null!;
    public string Nombre         { get; set; } = null!;
    public string? Descripcion   { get; set; }
    public decimal PresupuestoAsignado { get; set; }
    public decimal GastoActual   { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto                          Proyecto     { get; set; } = null!;
    public virtual ICollection<AsignacionCentroCosto> Asignaciones { get; set; } = new List<AsignacionCentroCosto>();
}

/// <summary>
/// Asignación de un costo real a un centro de costo.
/// Un costo puede repartirse entre varios centros (por porcentaje).
/// </summary>
public class AsignacionCentroCosto : BaseEntity
{
    public long    EmpresaId      { get; set; }
    public long    CentroCostoId  { get; set; }
    public long?   CostoRealId    { get; set; }
    public long?   OrdenTrabajoId { get; set; }
    public decimal Porcentaje     { get; set; } = 100;
    public decimal Monto          { get; set; }
    public string? Concepto       { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual CentroCosto   CentroCosto   { get; set; } = null!;
    public virtual CostoReal?    CostoReal     { get; set; }
    public virtual OrdenTrabajo? OrdenTrabajo  { get; set; }
}
