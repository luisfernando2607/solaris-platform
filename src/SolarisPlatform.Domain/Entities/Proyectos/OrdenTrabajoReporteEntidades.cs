using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 7: ÓRDENES DE TRABAJO (FIELD SERVICE)
// Tablas: orden_trabajo, ot_actividad, ot_material
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Orden de Trabajo (OT). Instrucción formal para ejecutar actividades en campo.
/// Soporta firma digital, geolocalización y requerimiento de fotos.
/// </summary>
public class OrdenTrabajo : BaseEntity
{
    public long                EmpresaId          { get; set; }
    public long                ProyectoId         { get; set; }
    public long?               TareaId            { get; set; }
    public long?               CuadrillaId        { get; set; }
    public string              Numero             { get; set; } = null!;
    public string              Titulo             { get; set; } = null!;
    public string?             Descripcion        { get; set; }
    public EstadoOrdenTrabajo  Estado             { get; set; } = EstadoOrdenTrabajo.Borrador;

    // Fechas y tiempos
    public DateTime?   FechaAsignacion        { get; set; }
    public DateTime?   FechaInicioPlan        { get; set; }
    public DateTime?   FechaFinPlan           { get; set; }
    public DateTime?   FechaInicioReal        { get; set; }
    public DateTime?   FechaFinReal           { get; set; }

    // Responsables (cross-schema)
    public long?  AsignadoPorId              { get; set; }   // FK seg.usuario
    public long?  TecnicoResponsableId       { get; set; }   // FK rrhh.empleado

    // Geolocalización del sitio
    public decimal?  Latitud                 { get; set; }
    public decimal?  Longitud                { get; set; }
    public string?   DireccionSitio          { get; set; }

    // Firma digital (campo)
    public bool      RequiereFirma           { get; set; } = false;
    public string?   FirmaBase64             { get; set; }
    public DateTime? FechaFirma              { get; set; }
    public string?   FirmadoPorNombre        { get; set; }

    // Control de fotos
    public bool  RequiereFotos              { get; set; } = false;
    public int   FotosRequeridas            { get; set; } = 0;

    // Observaciones
    public string? ObservacionesTecnico     { get; set; }
    public string? ObservacionesSupervisor  { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto               Proyecto    { get; set; } = null!;
    public virtual Tarea?                 Tarea       { get; set; }
    public virtual Cuadrilla?             Cuadrilla   { get; set; }
    public virtual ICollection<OtActividad>  Actividades { get; set; } = new List<OtActividad>();
    public virtual ICollection<OtMaterial>   Materiales  { get; set; } = new List<OtMaterial>();
    public virtual ICollection<ReporteAvance> Reportes   { get; set; } = new List<ReporteAvance>();
    public virtual ICollection<CostoReal>    CostosReales { get; set; } = new List<CostoReal>();
    public virtual ICollection<AsignacionCentroCosto> Asignaciones { get; set; } = new List<AsignacionCentroCosto>();
}

/// <summary>
/// Actividad dentro de una Orden de Trabajo.
/// Checklist de acciones a realizar en campo.
/// </summary>
public class OtActividad : BaseEntity
{
    public long    EmpresaId       { get; set; }
    public long    OrdenTrabajoId  { get; set; }
    public string  Nombre          { get; set; } = null!;
    public string? Descripcion     { get; set; }
    public int     Orden           { get; set; } = 1;
    public bool    Completada      { get; set; } = false;
    public DateTime? FechaComplecion { get; set; }
    public long?   CompletadaPorId { get; set; }   // FK seg.usuario
    public string? Observaciones   { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual OrdenTrabajo OrdenTrabajo { get; set; } = null!;
}

/// <summary>
/// Material utilizado en una Orden de Trabajo.
/// Permite rastrear el consumo real de materiales por OT.
/// </summary>
public class OtMaterial : BaseEntity
{
    public long    EmpresaId       { get; set; }
    public long    OrdenTrabajoId  { get; set; }
    public string  NombreMaterial  { get; set; } = null!;
    public string? CodigoMaterial  { get; set; }
    public string? UnidadMedida    { get; set; }
    public decimal CantidadPlan    { get; set; }
    public decimal CantidadReal    { get; set; }
    public decimal CostoUnitario   { get; set; }
    public decimal CostoTotal      { get; set; }
    public long?   ProductoId      { get; set; }   // futuro: inventario

    // ── Navegación ──────────────────────────────────────────────────
    public virtual OrdenTrabajo OrdenTrabajo { get; set; } = null!;
}

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 8: REPORTES DE AVANCE
// Tablas: reporte_avance, reporte_avance_foto
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Reporte de avance del proyecto. Registro periódico (diario/semanal/mensual)
/// del estado del proyecto con métricas EVM.
/// </summary>
public class ReporteAvance : BaseEntity
{
    public long      EmpresaId             { get; set; }
    public long      ProyectoId            { get; set; }
    public long?     OrdenTrabajoId        { get; set; }
    public DateTime  FechaReporte          { get; set; }
    public string    Titulo                { get; set; } = null!;
    public string?   Descripcion           { get; set; }

    // Avance físico
    public decimal   PorcentajeAvancePlan  { get; set; }
    public decimal   PorcentajeAvanceReal  { get; set; }

    // Métricas EVM (Earned Value Management)
    public decimal?  Bac                   { get; set; }   // Budget At Completion
    public decimal?  Ev                    { get; set; }   // Earned Value
    public decimal?  Pv                    { get; set; }   // Planned Value
    public decimal?  Ac                    { get; set; }   // Actual Cost
    public decimal?  Cpi                   { get; set; }   // Cost Performance Index
    public decimal?  Spi                   { get; set; }   // Schedule Performance Index

    // Responsable
    public long?     ReportadoPorId        { get; set; }   // FK seg.usuario
    public string?   Observaciones         { get; set; }

    // Geolocalización (si es reporte de campo)
    public decimal?  Latitud               { get; set; }
    public decimal?  Longitud              { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto       Proyecto     { get; set; } = null!;
    public virtual OrdenTrabajo?  OrdenTrabajo { get; set; }
    public virtual ICollection<ReporteAvanceFoto> Fotos { get; set; } = new List<ReporteAvanceFoto>();
}

/// <summary>
/// Foto adjunta a un reporte de avance.
/// Las fotos son evidencia del trabajo ejecutado en campo.
/// </summary>
public class ReporteAvanceFoto : BaseEntity
{
    public long    EmpresaId       { get; set; }
    public long    ReporteId       { get; set; }
    public string  UrlStorage      { get; set; } = null!;
    public string? NombreArchivo   { get; set; }
    public string? Descripcion     { get; set; }
    public decimal? Latitud        { get; set; }
    public decimal? Longitud       { get; set; }
    public DateTime FechaFoto      { get; set; } = DateTime.UtcNow;
    public int     Orden           { get; set; } = 1;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual ReporteAvance Reporte { get; set; } = null!;
}
