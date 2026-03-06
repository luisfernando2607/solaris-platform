using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 2: WBS Y TAREAS
// Tablas: wbs_nodo, tarea, tarea_dependencia
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Nodo del Work Breakdown Structure (WBS). Árbol jerárquico de entregables.
/// Puede representar un entregable, paquete de trabajo o tarea.
/// </summary>
public class WbsNodo : BaseEntity
{
    public long        EmpresaId          { get; set; }
    public long        ProyectoId         { get; set; }
    public long?       FaseId             { get; set; }
    public long?       PadreId            { get; set; }   // Auto-referencia para árbol
    public string      Codigo             { get; set; } = null!;  // Ej: "1.2.3"
    public string      Nombre             { get; set; } = null!;
    public string?     Descripcion        { get; set; }
    public TipoNodoWbs TipoNodo           { get; set; } = TipoNodoWbs.Entregable;
    public int         Nivel              { get; set; } = 1;       // Profundidad en el árbol
    public int         Orden              { get; set; } = 1;
    public decimal     PorcentajeAvance   { get; set; }
    public decimal     PorcentajePeso     { get; set; }   // Peso relativo en el proyecto
    public bool        EsHoja             { get; set; } = true;    // Sin hijos

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto             Proyecto  { get; set; } = null!;
    public virtual ProyectoFase?        Fase      { get; set; }
    public virtual WbsNodo?             Padre     { get; set; }
    public virtual ICollection<WbsNodo> Hijos     { get; set; } = new List<WbsNodo>();
    public virtual ICollection<Tarea>   Tareas    { get; set; } = new List<Tarea>();
}

/// <summary>
/// Tarea específica dentro del WBS. Unidad mínima de trabajo rastreable.
/// </summary>
public class Tarea : BaseEntity
{
    public long            EmpresaId          { get; set; }
    public long            ProyectoId         { get; set; }
    public long?           WbsNodoId          { get; set; }
    public long?           FaseId             { get; set; }
    public long?           CuadrillaId        { get; set; }
    public long?           ResponsableId      { get; set; }   // FK seg.usuario
    public string          Nombre             { get; set; } = null!;
    public string?         Descripcion        { get; set; }
    public EstadoTarea     Estado             { get; set; } = EstadoTarea.Pendiente;
    public PrioridadTarea  Prioridad          { get; set; } = PrioridadTarea.Media;

    // Cronograma
    public DateOnly?   FechaInicioPlan        { get; set; }
    public DateOnly?   FechaFinPlan           { get; set; }
    public DateOnly?   FechaInicioReal        { get; set; }
    public DateOnly?   FechaFinReal           { get; set; }

    // Esfuerzo
    public decimal     DuracionDias           { get; set; }       // Duración planificada en días
    public decimal     HorasEstimadas         { get; set; }
    public decimal     HorasReales            { get; set; }
    public decimal     PorcentajeAvance       { get; set; }

    // Geolocalización (para trabajo de campo)
    public decimal?    Latitud                { get; set; }
    public decimal?    Longitud               { get; set; }
    public string?     Ubicacion              { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto              Proyecto          { get; set; } = null!;
    public virtual WbsNodo?              WbsNodo           { get; set; }
    public virtual ProyectoFase?         Fase              { get; set; }
    public virtual Cuadrilla?            Cuadrilla         { get; set; }

    public virtual ICollection<TareaDependencia> DependenciasOrigen  { get; set; } = new List<TareaDependencia>();
    public virtual ICollection<TareaDependencia> DependenciasDestino { get; set; } = new List<TareaDependencia>();
    public virtual ICollection<GanttProgreso>    GanttProgresos      { get; set; } = new List<GanttProgreso>();
}

/// <summary>
/// Dependencia entre tareas. Implementa los 4 tipos estándar de Project Management.
/// </summary>
public class TareaDependencia : BaseEntity
{
    public long            EmpresaId      { get; set; }
    public long            TareaOrigenId  { get; set; }
    public long            TareaDestinoId { get; set; }
    public TipoDependencia TipoDependencia { get; set; } = TipoDependencia.FinInicio;

    /// <summary>Desfase en días (positivo = retraso, negativo = adelanto)</summary>
    public int             Desfase        { get; set; } = 0;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Tarea TareaOrigen      { get; set; } = null!;
    public virtual Tarea TareaDestino     { get; set; } = null!;
}
