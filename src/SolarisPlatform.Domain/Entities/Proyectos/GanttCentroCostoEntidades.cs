using SolarisPlatform.Domain.Common;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 5: GANTT Y CENTRO DE COSTOS
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Centro de Costo del proyecto.
/// BD real: proy.centro_costo
/// Columnas: id, empresa_id, proyecto_id, codigo, nombre, descripcion,
///           tipo, responsable_id, presupuesto_anual
///
/// FIX: GastoActual no existe en BD. PresupuestoAsignado → presupuesto_anual.
///      BD tiene: tipo (smallint), responsable_id — que faltaban en la entidad original.
/// </summary>
public class CentroCosto : BaseEntity
{
    public long    EmpresaId            { get; set; }
    public long    ProyectoId           { get; set; }
    public string  Codigo               { get; set; } = null!;
    public string  Nombre               { get; set; } = null!;
    public string? Descripcion          { get; set; }
    // FIX: BD tiene columna "tipo" (smallint)
    public short   Tipo                 { get; set; } = 1;
    // FIX: BD tiene responsable_id
    public long?   ResponsableId        { get; set; }
    // FIX: PresupuestoAsignado → PresupuestoAnual (BD: presupuesto_anual)
    //      GastoActual eliminado — no existe en BD
    public decimal PresupuestoAnual     { get; set; }

    public virtual Proyecto                        Proyecto     { get; set; } = null!;
    public virtual ICollection<AsignacionCentroCosto> Asignaciones { get; set; } = new List<AsignacionCentroCosto>();
}

/// <summary>
/// Asignación de costo a un centro de costo.
/// BD: proy.asignacion_centro_costo
/// Columnas reales: id, empresa_id, centro_costo_id, tipo_origen (NOT NULL),
///   origen_id (NOT NULL), monto (NOT NULL), fecha (date NOT NULL),
///   descripcion varchar(500), registrado_por_id
/// FIX: Eliminados CostoRealId, OrdenTrabajoId, Porcentaje, Concepto, FechaAsignacion
///      Agregados: TipoOrigen, OrigenId, Fecha, RegistradoPorId, EmpresaId
/// </summary>
public class AsignacionCentroCosto : BaseEntity
{
    public long      EmpresaId        { get; set; }
    public long      CentroCostoId    { get; set; }
    // tipo_origen NOT NULL: 1=CostoReal, 2=OrdenTrabajo, 3=Manual
    public short     TipoOrigen       { get; set; } = 1;
    // origen_id NOT NULL: ID del registro origen
    public long      OrigenId         { get; set; } = 1;
    public decimal   Monto            { get; set; }
    // fecha date NOT NULL
    public DateOnly  Fecha            { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    // descripcion reemplaza Concepto
    public string?   Descripcion      { get; set; }
    public long?     RegistradoPorId  { get; set; }

    public virtual CentroCosto CentroCosto { get; set; } = null!;
}

/// <summary>
/// Línea base del Gantt.
/// BD: proy.gantt_linea_base — sin cambios relevantes.
/// </summary>
public class GanttLineaBase : BaseEntity
{
    public long      EmpresaId        { get; set; }
    public long      ProyectoId       { get; set; }
    public long?     TareaId          { get; set; }
    public long?     FaseId           { get; set; }
    public DateTime? FechaCaptura     { get; set; }
    public DateOnly  FechaInicioBase  { get; set; }
    public DateOnly  FechaFinBase     { get; set; }
    public decimal   DuracionDias     { get; set; }
    public decimal   CostoBase        { get; set; }
    public string?   Descripcion      { get; set; }

    public virtual Proyecto      Proyecto { get; set; } = null!;
    public virtual Tarea?        Tarea    { get; set; }
    public virtual ProyectoFase? Fase     { get; set; }
}

/// <summary>
/// Progreso registrado en el Gantt.
/// BD: proy.gantt_progreso — sin cambios relevantes.
/// </summary>
public class GanttProgreso : BaseEntity
{
    public long      EmpresaId        { get; set; }
    public long      TareaId          { get; set; }
    public DateOnly  FechaProgreso    { get; set; }
    public decimal   PorcentajeAvance { get; set; }
    public decimal   HorasTrabajadas  { get; set; }
    public string?   Observaciones    { get; set; }
    public long?     ReportadoPorId   { get; set; }

    public virtual Tarea Tarea { get; set; } = null!;
}
