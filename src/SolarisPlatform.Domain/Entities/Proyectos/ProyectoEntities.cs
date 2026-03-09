using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 1: PROYECTOS BASE
// Tablas: proyecto, proyecto_hito, proyecto_fase, proyecto_documento
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Entidad raíz del módulo. Representa un proyecto completo
/// de telecomunicaciones/infraestructura.
/// </summary>
public class Proyecto : BaseEntity
{
    public long   EmpresaId              { get; set; }
    public string Codigo                 { get; set; } = null!;
    public string Nombre                 { get; set; } = null!;
    public string? Descripcion           { get; set; }

    // Clasificación
    public TipoProyecto    TipoProyecto  { get; set; } = TipoProyecto.NuevaObra;
    public EstadoProyecto  Estado        { get; set; } = EstadoProyecto.Borrador;
    public PrioridadProyecto Prioridad   { get; set; } = PrioridadProyecto.Media;

    // Fechas planificadas y reales
    public DateOnly? FechaInicioPlan     { get; set; }
    public DateOnly? FechaFinPlan        { get; set; }
    public DateOnly? FechaInicioReal     { get; set; }
    public DateOnly? FechaFinReal        { get; set; }

    // Financiero
    public long?    MonedaId             { get; set; }   // Cross-schema sin FK formal → cat.moneda
    public decimal  PresupuestoTotal     { get; set; }
    public decimal  CostoRealTotal       { get; set; }

    // Avance (0.00 – 100.00)
    public decimal  PorcentajeAvancePlan { get; set; }
    public decimal  PorcentajeAvanceReal { get; set; }

    // Relaciones (cross-schema, sin FK formal en EF)
    public long?  ClienteId             { get; set; }   // futuro crm.cliente
    public long?  GerenteProyectoId     { get; set; }   // rrhh.empleado
    public long?  ResponsableId         { get; set; }   // seg.usuario
    public long?  SucursalId            { get; set; }   // emp.sucursal

    // Geolocalización
    public decimal? Latitud             { get; set; }
    public decimal? Longitud            { get; set; }
    public string?  Direccion           { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual ICollection<ProyectoHito>       Hitos       { get; set; } = new List<ProyectoHito>();
    public virtual ICollection<ProyectoFase>       Fases       { get; set; } = new List<ProyectoFase>();
    public virtual ICollection<ProyectoDocumento>  Documentos  { get; set; } = new List<ProyectoDocumento>();
    public virtual ICollection<WbsNodo>            WbsNodos    { get; set; } = new List<WbsNodo>();
    public virtual ICollection<Cuadrilla>          Cuadrillas  { get; set; } = new List<Cuadrilla>();
    public virtual ICollection<RecursoProyecto>    Recursos    { get; set; } = new List<RecursoProyecto>();
    public virtual ICollection<Presupuesto>        Presupuestos { get; set; } = new List<Presupuesto>();
    public virtual ICollection<GanttLineaBase>     GanttLineas { get; set; } = new List<GanttLineaBase>();
    public virtual ICollection<CentroCosto>        CentrosCosto { get; set; } = new List<CentroCosto>();
    public virtual ICollection<OrdenTrabajo>       OrdenesTrabajo { get; set; } = new List<OrdenTrabajo>();
    public virtual ICollection<ReporteAvance>      Reportes    { get; set; } = new List<ReporteAvance>();
    public virtual ICollection<KpiProyecto>        Kpis        { get; set; } = new List<KpiProyecto>();
    public virtual ICollection<AlertaProyecto>     Alertas     { get; set; } = new List<AlertaProyecto>();
}

/// <summary>
/// Hitos clave del proyecto. Puntos de control con fecha compromiso y fecha real.
/// </summary>
public class ProyectoHito : BaseEntity
{
    public long        EmpresaId         { get; set; }
    public long        ProyectoId        { get; set; }
    public string      Nombre            { get; set; } = null!;
    public string?     Descripcion       { get; set; }
    public DateOnly    FechaCompromiso   { get; set; }
    public DateOnly?   FechaReal         { get; set; }
    public EstadoHito  Estado            { get; set; } = EstadoHito.Pendiente;
    public decimal     PorcentajePeso    { get; set; }   // Peso relativo dentro del proyecto
    public long?       ResponsableId     { get; set; }   // FK seg.usuario
    public int         Orden             { get; set; } = 1;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto Proyecto     { get; set; } = null!;
}

/// <summary>
/// Fases del proyecto (nivel 1 del WBS).
/// </summary>
public class ProyectoFase : BaseEntity
{
    public long        EmpresaId         { get; set; }
    public long        ProyectoId        { get; set; }
    public string      Codigo            { get; set; } = null!;
    public string      Nombre            { get; set; } = null!;
    public string?     Descripcion       { get; set; }
    public int         Orden             { get; set; } = 1;
    public DateOnly?   FechaInicioPlan   { get; set; }
    public DateOnly?   FechaFinPlan      { get; set; }
    public DateOnly?   FechaInicioReal   { get; set; }
    public DateOnly?   FechaFinReal      { get; set; }
    public decimal     PorcentajeAvance  { get; set; }
    public EstadoFase  Estado            { get; set; } = EstadoFase.Pendiente;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto            Proyecto  { get; set; } = null!;
    public virtual ICollection<WbsNodo> WbsNodos { get; set; } = new List<WbsNodo>();
}

/// <summary>
/// Documentos adjuntos al proyecto (contratos, planos, permisos, fotos, informes).
/// </summary>
public class ProyectoDocumento : BaseEntity
{
    public long           EmpresaId              { get; set; }
    public long           ProyectoId             { get; set; }
    public TipoDocumento  TipoDocumento          { get; set; } = TipoDocumento.Otro;
    public string         Nombre                 { get; set; } = null!;
    public string?        Descripcion            { get; set; }
    public string         UrlStorage             { get; set; } = null!;
    public string?        NombreArchivoOriginal  { get; set; }
    public string?        Extension              { get; set; }
    public long?          TamanoBytes            { get; set; }
    public long?          SubidoPorId            { get; set; }   // FK seg.usuario
    public DateTime       FechaSubida            { get; set; } = DateTime.UtcNow;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto Proyecto             { get; set; } = null!;
}
