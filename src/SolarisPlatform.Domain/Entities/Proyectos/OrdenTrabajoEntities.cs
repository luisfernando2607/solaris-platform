using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 7: ÓRDENES DE TRABAJO
// BD real: proy.orden_trabajo
// Columnas reales: id, empresa_id, proyecto_id, tarea_id,
//   presupuesto_partida_id, codigo, descripcion, tipo_ot, estado, prioridad,
//   tecnico_asignado_id, cuadrilla_id, fecha_programada,
//   fecha_inicio_ejecucion, fecha_fin_ejecucion, tiempo_ejecucion_min,
//   latitud, longitud, direccion, primer_intento,
//   observaciones_tecnico, observaciones_cierre, url_firma_digital
//
// FIX: La entidad original tenía Numero, Titulo, FechaAsignacion,
//      FechaInicioPlan/FinPlan/InicioReal/FinReal, AsignadoPorId,
//      TecnicoResponsableId, DireccionSitio, RequiereFirma, FirmaBase64,
//      FechaFirma, FirmadoPorNombre, RequiereFotos, FotosRequeridas,
//      ObservacionesSupervisor — muchos NO existen en la BD.
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Orden de Trabajo (OT).
/// </summary>
public class OrdenTrabajo : BaseEntity
{
    public long                EmpresaId              { get; set; }
    public long                ProyectoId             { get; set; }
    public long?               TareaId                { get; set; }
    // FIX: BD tiene presupuesto_partida_id (no en entidad original)
    public long?               PresupuestoPartidaId   { get; set; }
    public long?               CuadrillaId            { get; set; }

    // FIX: Numero/Titulo → Codigo/Descripcion (BD: codigo, descripcion)
    public string              Codigo                 { get; set; } = null!;
    public string?             Descripcion            { get; set; }

    // FIX: BD tiene tipo_ot y prioridad (no en entidad original)
    public short               TipoOt                 { get; set; } = 1;
    public EstadoOrdenTrabajo  Estado                 { get; set; } = EstadoOrdenTrabajo.Borrador;
    public short               Prioridad              { get; set; } = 2;

    // FIX: TecnicoResponsableId → TecnicoAsignadoId (BD: tecnico_asignado_id)
    public long?               TecnicoAsignadoId      { get; set; }

    // FIX: Fechas reales en BD (no plan/real separados)
    public DateTime?           FechaProgramada        { get; set; }
    public DateTime?           FechaInicioEjecucion   { get; set; }
    public DateTime?           FechaFinEjecucion      { get; set; }
    // FIX: BD tiene tiempo_ejecucion_min (minutos de ejecución)
    public int?                TiempoEjecucionMin     { get; set; }

    // Geolocalización
    public decimal?            Latitud                { get; set; }
    public decimal?            Longitud               { get; set; }
    // FIX: DireccionSitio → Direccion (BD: direccion)
    public string?             Direccion              { get; set; }

    // FIX: BD tiene primer_intento (bool), no RequiereFirma/Fotos
    public bool?                PrimerIntento          { get; set; } = true;
    // FIX: url_firma_digital reemplaza FirmaBase64
    public string?             UrlFirmaDigital        { get; set; }

    public string?             ObservacionesTecnico   { get; set; }
    // FIX: ObservacionesSupervisor → ObservacionesCierre (BD: observaciones_cierre)
    public string?             ObservacionesCierre    { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto               Proyecto     { get; set; } = null!;
    public virtual Tarea?                 Tarea        { get; set; }
    public virtual Cuadrilla?             Cuadrilla    { get; set; }
    public virtual ICollection<OtActividad>           Actividades  { get; set; } = new List<OtActividad>();
    public virtual ICollection<OtMaterial>            Materiales   { get; set; } = new List<OtMaterial>();
    public virtual ICollection<ReporteAvance>         Reportes     { get; set; } = new List<ReporteAvance>();
    public virtual ICollection<CostoReal>             CostosReales { get; set; } = new List<CostoReal>();
    public virtual ICollection<AsignacionCentroCosto> Asignaciones { get; set; } = new List<AsignacionCentroCosto>();
}

public class OtActividad : BaseEntity
{
    public long    EmpresaId       { get; set; }
    public long    OrdenTrabajoId  { get; set; }
    // FIX F7: BD tiene solo "descripcion" (no "nombre" separado)
    public string  Descripcion     { get; set; } = null!;
    public int     Orden           { get; set; } = 1;
    // FIX F7: BD: completado (bool), fecha_completado, observacion_tecnico
    //         NO existe: nombre, completada_por_id
    public bool    Completado      { get; set; } = false;
    public DateTime? FechaCompletado { get; set; }
    public string? ObservacionTecnico { get; set; }
    // BD tiene: tipo_actividad, requiere_foto, url_foto
    public short?  TipoActividad   { get; set; }
    public bool    RequiereFoto    { get; set; } = false;
    public string? UrlFoto         { get; set; }

    public virtual OrdenTrabajo OrdenTrabajo { get; set; } = null!;
}

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
    public long?   ProductoId      { get; set; }

    public virtual OrdenTrabajo OrdenTrabajo { get; set; } = null!;
}

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 8: REPORTES DE AVANCE
// BD real: proy.reporte_avance
// Columnas: id, empresa_id, proyecto_id, titulo, fecha_reporte,
//   creado_por_id, avance_general, avance_costo, observaciones,
//   proximas_actividades, riesgos_identificados, conclusiones,
//   enviado_a_cliente, fecha_envio
//
// FIX: La entidad original tenía OrdenTrabajoId, PorcentajeAvancePlan/Real,
//      Bac/Ev/Pv/Ac/Cpi/Spi, ReportadoPorId, Latitud/Longitud —
//      NINGUNA de esas columnas EVM existe en BD.
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Reporte de avance del proyecto.
/// </summary>
public class ReporteAvance : BaseEntity
{
    public long      EmpresaId              { get; set; }
    public long      ProyectoId             { get; set; }
    public string    Titulo                 { get; set; } = null!;
    public DateOnly  FechaReporte           { get; set; }
    // FIX: ReportadoPorId → CreadoPorId (BD: creado_por_id)
    public long?     CreadoPorId            { get; set; }

    // FIX: PorcentajeAvancePlan/Real → AvanceGeneral/AvanceCosto (BD)
    public decimal   AvanceGeneral          { get; set; }
    public decimal   AvanceCosto            { get; set; }

    public string?   Observaciones          { get; set; }
    // FIX: Nuevos campos BD que no estaban en entidad
    public string?   ProximasActividades    { get; set; }
    public string?   RiesgosIdentificados   { get; set; }
    public string?   Conclusiones           { get; set; }
    public bool      EnviadoACliente        { get; set; } = false;
    public DateTime? FechaEnvio             { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto Proyecto { get; set; } = null!;
    public virtual ICollection<ReporteAvanceFoto> Fotos { get; set; } = new List<ReporteAvanceFoto>();
}

public class ReporteAvanceFoto : BaseEntity
{
    public long    EmpresaId     { get; set; }
    public long    ReporteId     { get; set; }
    public string  UrlStorage    { get; set; } = null!;
    public string? NombreArchivo { get; set; }
    public string? Descripcion   { get; set; }
    public decimal? Latitud      { get; set; }
    public decimal? Longitud     { get; set; }
    public DateTime FechaFoto    { get; set; } = DateTime.UtcNow;
    public int     Orden         { get; set; } = 1;

    public virtual ReporteAvance Reporte { get; set; } = null!;
}
