using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 2: WBS Y TAREAS
// Tablas: wbs_nodo, tarea, tarea_dependencia
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Nodo del Work Breakdown Structure (WBS). Árbol jerárquico de entregables.
/// BD: proy.wbs_nodo
/// </summary>
public class WbsNodo : BaseEntity
{
    public long        EmpresaId          { get; set; }
    public long        ProyectoId         { get; set; }
    public long?       FaseId             { get; set; }
    // FIX: padre_id → nodo_padre_id en BD
    public long?       PadreId            { get; set; }
    // FIX: codigo → codigo_wbs en BD
    public string      CodigoWbs          { get; set; } = null!;
    public string      Nombre             { get; set; } = null!;
    public string?     Descripcion        { get; set; }
    public TipoNodoWbs TipoNodo           { get; set; } = TipoNodoWbs.Entregable;
    public int         Nivel              { get; set; } = 1;
    public int         Orden              { get; set; } = 1;
    public decimal     PorcentajeAvance   { get; set; }
    // FIX: porcentaje_peso → peso_relativo en BD
    public decimal     PesoRelativo       { get; set; }
    public bool        EsHoja             { get; set; } = true;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto             Proyecto  { get; set; } = null!;
    public virtual ProyectoFase?        Fase      { get; set; }
    public virtual WbsNodo?             Padre     { get; set; }
    public virtual ICollection<WbsNodo> Hijos     { get; set; } = new List<WbsNodo>();
    public virtual ICollection<Tarea>   Tareas    { get; set; } = new List<Tarea>();
}

/// <summary>
/// Tarea específica dentro del WBS.
/// BD: proy.tarea — NO tiene fase_id, proyecto_id, cuadrilla_id, horas_*, ubicacion
/// Solo tiene: wbs_nodo_id, responsable_id, cuadrilla_id, duracion_dias_plan, duracion_dias_real
/// </summary>
public class Tarea : BaseEntity
{
    public long            EmpresaId          { get; set; }
    public long            ProyectoId         { get; set; }
    public long?           WbsNodoId          { get; set; }
    public long?           CuadrillaId        { get; set; }
    public long?           ResponsableId      { get; set; }
    public string          Nombre             { get; set; } = null!;
    public string?         Descripcion        { get; set; }
    public EstadoTarea     Estado             { get; set; } = EstadoTarea.Pendiente;
    public PrioridadTarea  Prioridad          { get; set; } = PrioridadTarea.Media;

    // Cronograma
    public DateOnly?   FechaInicioPlan        { get; set; }
    public DateOnly?   FechaFinPlan           { get; set; }
    public DateOnly?   FechaInicioReal        { get; set; }
    public DateOnly?   FechaFinReal           { get; set; }

    // FIX: duracion_dias → duracion_dias_plan + duracion_dias_real en BD
    public int         DuracionDiasPlan       { get; set; }
    public int         DuracionDiasReal       { get; set; }

    public decimal     PorcentajeAvance       { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto              Proyecto          { get; set; } = null!;
    public virtual WbsNodo?              WbsNodo           { get; set; }
    public virtual Cuadrilla?            Cuadrilla         { get; set; }

    public virtual ICollection<TareaDependencia> DependenciasOrigen  { get; set; } = new List<TareaDependencia>();
    public virtual ICollection<TareaDependencia> DependenciasDestino { get; set; } = new List<TareaDependencia>();
    public virtual ICollection<GanttProgreso>    GanttProgresos      { get; set; } = new List<GanttProgreso>();
}

/// <summary>
/// Dependencia entre tareas.
/// BD: proy.tarea_dependencia
/// </summary>
public class TareaDependencia : BaseEntity
{
    public long            EmpresaId      { get; set; }
    public long            TareaOrigenId  { get; set; }
    public long            TareaDestinoId { get; set; }
    public TipoDependencia TipoDependencia { get; set; } = TipoDependencia.FinInicio;
    public int             Desfase        { get; set; } = 0;

    public virtual Tarea TareaOrigen      { get; set; } = null!;
    public virtual Tarea TareaDestino     { get; set; } = null!;
}
