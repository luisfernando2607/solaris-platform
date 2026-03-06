using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Application.DTOs.Proyectos;

// ═══════════════════════════════════════════════════════════════
// PROYECTO
// ═══════════════════════════════════════════════════════════════

public record ProyectoListDto(
    long Id, string Codigo, string Nombre, TipoProyecto TipoProyecto,
    EstadoProyecto Estado, PrioridadProyecto Prioridad,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    decimal PorcentajeAvancePlan, decimal PorcentajeAvanceReal,
    decimal PresupuestoTotal, decimal CostoRealTotal,
    string? GerenteProyectoNombre, bool Activo);

public record ProyectoDto(
    long Id, long EmpresaId, string Codigo, string Nombre, string? Descripcion,
    TipoProyecto TipoProyecto, EstadoProyecto Estado, PrioridadProyecto Prioridad,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    DateOnly? FechaInicioReal, DateOnly? FechaFinReal,
    long? MonedaId, string? MonedaNombre,
    decimal PresupuestoTotal, decimal CostoRealTotal,
    decimal PorcentajeAvancePlan, decimal PorcentajeAvanceReal,
    long? ClienteId, string? ClienteNombre,
    long? GerenteProyectoId, string? GerenteProyectoNombre,
    long? ResponsableId, string? ResponsableNombre,
    long? SucursalId, string? SucursalNombre,
    decimal? Latitud, decimal? Longitud, string? Direccion,
    bool Activo, DateTime FechaCreacion,
    List<ProyectoFaseDto> Fases);

public record ProyectoDashboardDto(
    long Id, string Codigo, string Nombre, EstadoProyecto Estado,
    decimal PorcentajeAvancePlan, decimal PorcentajeAvanceReal,
    decimal PresupuestoTotal, decimal CostoRealTotal,
    int? DiasRestantes, bool EstaRetrasado,
    List<ProyectoHitoListDto> HitosCercanos,
    List<AlertaProyectoDto> AlertasActivas,
    List<KpiProyectoDto> UltimosKpis)
{
    // Constructor mutable para el servicio
    public int? DiasRestantes { get; set; } = DiasRestantes;
    public bool EstaRetrasado { get; set; } = EstaRetrasado;
    public List<ProyectoHitoListDto> HitosCercanos { get; set; } = HitosCercanos;
    public List<AlertaProyectoDto> AlertasActivas { get; set; } = AlertasActivas;
    public List<KpiProyectoDto> UltimosKpis { get; set; } = UltimosKpis;
}

public record CrearProyectoRequest(
    string Codigo, string Nombre, string? Descripcion,
    TipoProyecto TipoProyecto, PrioridadProyecto Prioridad,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    long? MonedaId, decimal PresupuestoTotal,
    long? ClienteId, long? GerenteProyectoId, long? ResponsableId,
    long? SucursalId, decimal? Latitud, decimal? Longitud, string? Direccion)
{
    public long EmpresaId { get; set; }
}

public record ActualizarProyectoRequest(
    string Nombre, string? Descripcion,
    TipoProyecto TipoProyecto, PrioridadProyecto Prioridad,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    DateOnly? FechaInicioReal, DateOnly? FechaFinReal,
    long? MonedaId, decimal PresupuestoTotal,
    long? ClienteId, long? GerenteProyectoId, long? ResponsableId,
    long? SucursalId, decimal? Latitud, decimal? Longitud, string? Direccion)
{
    public long Id { get; set; }
}

public record CambiarEstadoProyectoRequest(EstadoProyecto Estado)
{
    public long Id { get; set; }
}

public record ActualizarAvanceProyectoRequest(decimal AvanceReal)
{
    public long Id { get; set; }
}

public record FiltroProyectosRequest(
    string? Busqueda = null,
    EstadoProyecto? Estado = null,
    TipoProyecto? Tipo = null,
    long? GerenteId = null,
    int Pagina = 1,
    int ElementosPorPagina = 20)
{
    public long? EmpresaId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// FASES
// ═══════════════════════════════════════════════════════════════

public record ProyectoFaseDto(
    long Id, long ProyectoId, string Codigo, string Nombre, string? Descripcion,
    int Orden, DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    DateOnly? FechaInicioReal, DateOnly? FechaFinReal,
    decimal PorcentajeAvance, EstadoFase Estado, bool Activo);

public record CrearProyectoFaseRequest(
    string Codigo, string Nombre, string? Descripcion,
    int Orden, DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan)
{
    public long ProyectoId { get; set; }
}

public record ActualizarProyectoFaseRequest(
    string Nombre, string? Descripcion, int Orden,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    DateOnly? FechaInicioReal, DateOnly? FechaFinReal,
    decimal PorcentajeAvance, EstadoFase Estado)
{
    public long Id { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// HITOS
// ═══════════════════════════════════════════════════════════════

public record ProyectoHitoDto(
    long Id, long ProyectoId, string Nombre, string? Descripcion,
    DateOnly FechaCompromiso, DateOnly? FechaReal,
    EstadoHito Estado, decimal PorcentajePeso,
    long? ResponsableId, string? ResponsableNombre,
    int Orden, bool Activo);

public record ProyectoHitoListDto(
    long Id, string Nombre, DateOnly FechaCompromiso, EstadoHito Estado,
    decimal PorcentajePeso, string? ResponsableNombre, int Orden);

public record CrearProyectoHitoRequest(
    string Nombre, string? Descripcion,
    DateOnly FechaCompromiso, decimal PorcentajePeso,
    long? ResponsableId, int Orden)
{
    public long ProyectoId { get; set; }
}

public record ActualizarProyectoHitoRequest(
    string Nombre, string? Descripcion,
    DateOnly FechaCompromiso, decimal PorcentajePeso,
    long? ResponsableId, int Orden, EstadoHito Estado)
{
    public long Id { get; set; }
}

public record MarcarHitoLogradoRequest(DateOnly FechaReal)
{
    public long Id { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// DOCUMENTOS
// ═══════════════════════════════════════════════════════════════

public record ProyectoDocumentoDto(
    long Id, long ProyectoId, TipoDocumento TipoDocumento,
    string Nombre, string? Descripcion, string UrlStorage,
    string? NombreArchivoOriginal, string? Extension, long? TamanoBytes,
    string? SubidoPorNombre, DateTime FechaSubida, bool Activo);

public record CrearProyectoDocumentoRequest(
    TipoDocumento TipoDocumento, string Nombre, string? Descripcion,
    string UrlStorage, string? NombreArchivoOriginal, string? Extension, long? TamanoBytes)
{
    public long ProyectoId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// WBS
// ═══════════════════════════════════════════════════════════════

public record WbsNodoDto(
    long Id, long ProyectoId, long? FaseId, long? PadreId,
    string Codigo, string Nombre, string? Descripcion,
    TipoNodoWbs TipoNodo, int Nivel, int Orden,
    decimal PorcentajeAvance, decimal PorcentajePeso, bool EsHoja, bool Activo,
    List<WbsNodoDto> Hijos, List<TareaListDto> Tareas);

public record CrearWbsNodoRequest(
    string Codigo, string Nombre, string? Descripcion,
    TipoNodoWbs TipoNodo, int Orden, decimal PorcentajePeso,
    long? FaseId, long? PadreId)
{
    public long ProyectoId { get; set; }
}

public record ActualizarWbsNodoRequest(
    string Nombre, string? Descripcion,
    TipoNodoWbs TipoNodo, int Orden, decimal PorcentajePeso)
{
    public long Id { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// TAREAS
// ═══════════════════════════════════════════════════════════════

public record TareaListDto(
    long Id, long ProyectoId, long? WbsNodoId, long? FaseId,
    string Nombre, EstadoTarea Estado, PrioridadTarea Prioridad,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    decimal PorcentajeAvance, long? ResponsableId, string? ResponsableNombre,
    long? CuadrillaId, string? CuadrillaNombre);

public record TareaDto(
    long Id, long ProyectoId, long? WbsNodoId, long? FaseId, long? CuadrillaId,
    long? ResponsableId, string? ResponsableNombre,
    string Nombre, string? Descripcion,
    EstadoTarea Estado, PrioridadTarea Prioridad,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    DateOnly? FechaInicioReal, DateOnly? FechaFinReal,
    int? DuracionDias, decimal? HorasEstimadas, decimal HorasReales,
    decimal PorcentajeAvance,
    decimal? Latitud, decimal? Longitud, string? Ubicacion, bool Activo,
    List<TareaDependenciaDto> DependenciasOrigen,
    List<TareaDependenciaDto> DependenciasDestino);

public record TareaDependenciaDto(
    long Id, long TareaOrigenId, string TareaOrigenNombre,
    long TareaDestinoId, string TareaDestinoNombre,
    TipoDependencia TipoDependencia, int? Desfase);

public record CrearTareaRequest(
    string Nombre, string? Descripcion, PrioridadTarea Prioridad,
    long? WbsNodoId, long? FaseId, long? CuadrillaId, long? ResponsableId,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    int? DuracionDias, decimal? HorasEstimadas,
    decimal? Latitud, decimal? Longitud, string? Ubicacion)
{
    public long ProyectoId { get; set; }
}

public record ActualizarTareaRequest(
    string Nombre, string? Descripcion, PrioridadTarea Prioridad,
    long? WbsNodoId, long? FaseId, long? CuadrillaId, long? ResponsableId,
    DateOnly? FechaInicioPlan, DateOnly? FechaFinPlan,
    DateOnly? FechaInicioReal, DateOnly? FechaFinReal,
    int? DuracionDias, decimal? HorasEstimadas,
    decimal? Latitud, decimal? Longitud, string? Ubicacion)
{
    public long Id { get; set; }
}

public record CambiarEstadoTareaRequest(EstadoTarea Estado)
{
    public long Id { get; set; }
}

public record ActualizarAvanceTareaRequest(decimal Porcentaje, decimal HorasTrabajadas)
{
    public long Id { get; set; }
}

public record CrearDependenciaTareaRequest(
    long TareaDestinoId, TipoDependencia TipoDependencia, int? Desfase)
{
    public long TareaOrigenId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// CUADRILLAS
// ═══════════════════════════════════════════════════════════════

public record CuadrillaDto(
    long Id, long ProyectoId, string Nombre, string? Descripcion,
    long? LiderId, string? LiderNombre, int CapacidadMax, bool Activo,
    List<CuadrillaMiembroDto> Miembros);

public record CuadrillaMiembroDto(
    long Id, long EmpleadoId, string EmpleadoNombre,
    DateOnly FechaIngreso, DateOnly? FechaSalida,
    string? Rol, bool Activo);

public record CrearCuadrillaRequest(
    string Nombre, string? Descripcion, long? LiderId, int CapacidadMax)
{
    public long ProyectoId { get; set; }
}

public record ActualizarCuadrillaRequest(
    string Nombre, string? Descripcion, long? LiderId, int CapacidadMax)
{
    public long Id { get; set; }
}

public record AgregarMiembroCuadrillaRequest(
    long EmpleadoId, DateOnly FechaIngreso, string? Rol)
{
    public long CuadrillaId { get; set; }
}

public record RemoverMiembroCuadrillaRequest(long EmpleadoId, DateOnly FechaSalida)
{
    public long CuadrillaId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// PRESUPUESTO
// ═══════════════════════════════════════════════════════════════

public record PresupuestoDto(
    long Id, long ProyectoId, int Version, string Nombre, string? Descripcion,
    decimal TotalIngresos, decimal TotalEgresos, decimal Contingencia, decimal TotalNeto,
    bool EsActivo, bool EsAprobado,
    long? AprobadoPorId, string? AprobadoPorNombre, DateTime? FechaAprobacion,
    DateTime FechaCreacion, List<PresupuestoPartidaDto> Partidas);

public record PresupuestoPartidaDto(
    long Id, long PresupuestoId, TipoPartida Tipo,
    string Concepto, string? Descripcion, string? CodigoContable,
    decimal Cantidad, string? UnidadMedida, decimal PrecioUnitario,
    decimal Subtotal, decimal Porcentaje, decimal Total, int Orden,
    List<CostoRealDto> CostosReales);

public record CostoRealDto(
    long Id, long PresupuestoId, long PartidaId,
    OrigenCosto Origen, string Concepto, string? Descripcion,
    decimal Monto, DateOnly FechaRegistro, string? NumeroReferencia,
    long? OrdenTrabajoId, string? RegistradoPorNombre);

public record ResumenEjecucionDto(
    long ProyectoId, decimal PresupuestoTotal, decimal TotalEjecutado,
    decimal Saldo, decimal PorcentajeEjecucion,
    List<EjecucionPartidaDto> PorPartida);

public record EjecucionPartidaDto(
    long PartidaId, string Concepto, TipoPartida Tipo,
    decimal Presupuestado, decimal Ejecutado, decimal Saldo, decimal PorcentajeEjecucion);

public record CrearPresupuestoRequest(
    string Nombre, string? Descripcion, decimal Contingencia)
{
    public long ProyectoId { get; set; }
}

public record AgregarPartidaRequest(
    TipoPartida Tipo, string Concepto, string? Descripcion, string? CodigoContable,
    decimal Cantidad, string? UnidadMedida, decimal PrecioUnitario,
    decimal Porcentaje, int Orden)
{
    public long PresupuestoId { get; set; }
}

public record RegistrarCostoRealRequest(
    long PresupuestoId, long PartidaId, OrigenCosto Origen,
    string Concepto, string? Descripcion, decimal Monto,
    DateOnly FechaRegistro, string? NumeroReferencia, long? OrdenTrabajoId);

// ═══════════════════════════════════════════════════════════════
// GANTT
// ═══════════════════════════════════════════════════════════════

public record GanttDto(
    long ProyectoId, string ProyectoNombre,
    DateOnly FechaInicio, DateOnly FechaFin,
    List<GanttFaseDto> Fases);

public record GanttFaseDto(
    long Id, string Nombre,
    DateOnly? Inicio, DateOnly? Fin, decimal Avance,
    List<GanttTareaDto> Tareas);

public record GanttTareaDto(
    long Id, string Nombre,
    DateOnly? InicioPlan, DateOnly? FinPlan,
    DateOnly? InicioReal, DateOnly? FinReal,
    DateOnly? InicioBase, DateOnly? FinBase,
    decimal Avance, EstadoTarea Estado,
    List<long> DependenciasIds);

public record RegistrarProgresoGanttRequest(
    long TareaId, DateOnly FechaProgreso,
    decimal PorcentajeAvance, decimal HorasTrabajadas, string? Observaciones);

// ═══════════════════════════════════════════════════════════════
// CENTROS DE COSTO
// ═══════════════════════════════════════════════════════════════

public record CentroCostoDto(
    long Id, long ProyectoId, string Codigo, string Nombre, string? Descripcion,
    decimal PresupuestoAsignado, decimal GastoActual, bool Activo);

public record CrearCentroCostoRequest(
    string Codigo, string Nombre, string? Descripcion, decimal PresupuestoAsignado)
{
    public long ProyectoId { get; set; }
}

public record ActualizarCentroCostoRequest(
    string Nombre, string? Descripcion, decimal PresupuestoAsignado)
{
    public long Id { get; set; }
}

public record AsignarCostoACentroRequest(
    long? CostoRealId, long? OrdenTrabajoId,
    decimal Porcentaje, decimal Monto, string? Concepto)
{
    public long CentroCostoId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// ÓRDENES DE TRABAJO
// ═══════════════════════════════════════════════════════════════

public record OrdenTrabajoListDto(
    long Id, string Numero, string Titulo, EstadoOrdenTrabajo Estado,
    long ProyectoId, string? ProyectoNombre,
    long? CuadrillaId, string? CuadrillaNombre,
    long? TecnicoResponsableId, string? TecnicoNombre,
    DateTime? FechaInicioPlan, DateTime? FechaFinPlan,
    DateTime? FechaInicioReal, DateTime? FechaFinReal);

public record OrdenTrabajoDto(
    long Id, long ProyectoId, string? ProyectoNombre,
    long? TareaId, string? TareaNombre,
    long? CuadrillaId, string? CuadrillaNombre,
    string Numero, string Titulo, string? Descripcion,
    EstadoOrdenTrabajo Estado,
    DateTime FechaAsignacion,
    DateTime? FechaInicioPlan, DateTime? FechaFinPlan,
    DateTime? FechaInicioReal, DateTime? FechaFinReal,
    long? AsignadoPorId, string? AsignadoPorNombre,
    long? TecnicoResponsableId, string? TecnicoNombre,
    decimal? Latitud, decimal? Longitud, string? DireccionSitio,
    bool RequiereFirma, bool RequiereFotos, int FotosRequeridas,
    string? ObservacionesSupervisor,
    bool TieneFirma, DateTime? FechaFirma, string? FirmadoPorNombre,
    List<OtActividadDto> Actividades,
    List<OtMaterialDto> Materiales);

public record OtActividadDto(
    long Id, string Nombre, string? Descripcion, int Orden,
    bool Completada, DateTime? FechaComplecion,
    string? CompletadaPorNombre, string? Observaciones);

public record OtMaterialDto(
    long Id, string NombreMaterial, string? CodigoMaterial,
    string? UnidadMedida, decimal CantidadPlan, decimal CantidadReal,
    decimal CostoUnitario, decimal CostoTotal);

public record CrearOrdenTrabajoRequest(
    long ProyectoId, long? TareaId, long? CuadrillaId, long? TecnicoResponsableId,
    string Titulo, string? Descripcion,
    DateTime? FechaInicioPlan, DateTime? FechaFinPlan,
    decimal? Latitud, decimal? Longitud, string? DireccionSitio,
    bool RequiereFirma, bool RequiereFotos, int FotosRequeridas,
    List<CrearOtActividadItem> Actividades,
    List<CrearOtMaterialItem> Materiales);

public record CrearOtActividadItem(string Nombre, string? Descripcion);
public record CrearOtMaterialItem(
    string NombreMaterial, string? CodigoMaterial, string? UnidadMedida,
    decimal CantidadPlan, decimal CostoUnitario);

public record ActualizarOrdenTrabajoRequest(
    string Titulo, string? Descripcion,
    long? CuadrillaId, long? TecnicoResponsableId,
    DateTime? FechaInicioPlan, DateTime? FechaFinPlan,
    decimal? Latitud, decimal? Longitud, string? DireccionSitio,
    bool RequiereFirma, bool RequiereFotos, int FotosRequeridas,
    string? ObservacionesSupervisor)
{
    public long Id { get; set; }
}

public record CambiarEstadoOtRequest(EstadoOrdenTrabajo Estado)
{
    public long Id { get; set; }
}

public record CompletarActividadRequest(long ActividadId, string? Observaciones)
{
    public long OtId { get; set; }
}

public record RegistrarFirmaOtRequest(string FirmaBase64, string FirmantNombre)
{
    public long Id { get; set; }
}

public record FiltroOrdenesTrabajoRequest(
    string? Busqueda = null,
    EstadoOrdenTrabajo? Estado = null,
    long? ProyectoId = null,
    long? CuadrillaId = null,
    int Pagina = 1,
    int ElementosPorPagina = 20)
{
    public long? EmpresaId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// REPORTES DE AVANCE
// ═══════════════════════════════════════════════════════════════

public record ReporteAvanceDto(
    long Id, long ProyectoId, long? OrdenTrabajoId,
    DateOnly FechaReporte, string Titulo, string? Descripcion,
    decimal PorcentajeAvancePlan, decimal PorcentajeAvanceReal,
    decimal? Bac, decimal? Ev, decimal? Pv, decimal? Ac,
    decimal? Cpi, decimal? Spi,
    string? ReportadoPorNombre, string? Observaciones,
    decimal? Latitud, decimal? Longitud,
    List<ReporteAvanceFotoDto> Fotos);

public record ReporteAvanceFotoDto(
    long Id, string UrlStorage, string? NombreArchivo,
    string? Descripcion, decimal? Latitud, decimal? Longitud,
    DateTime FechaFoto, int Orden);

public record CrearReporteAvanceRequest(
    DateOnly FechaReporte, string Titulo, string? Descripcion,
    decimal PorcentajeAvancePlan, decimal PorcentajeAvanceReal,
    string? Observaciones, decimal? Latitud, decimal? Longitud,
    long? OrdenTrabajoId)
{
    public long ProyectoId { get; set; }
}

public record AgregarFotoReporteRequest(
    string UrlStorage, string? NombreArchivo, string? Descripcion,
    decimal? Latitud, decimal? Longitud, int Orden)
{
    public long ReporteId { get; set; }
}

// ═══════════════════════════════════════════════════════════════
// KPIs Y ALERTAS
// ═══════════════════════════════════════════════════════════════

public record KpiProyectoDto(
    long Id, long ProyectoId, TipoKpi TipoKpi, string Nombre,
    decimal Valor, decimal? ValorMeta, decimal? ValorMinimo,
    DateTime FechaCalculo, string? Formula,
    string Semaforo);  // "verde" | "amarillo" | "rojo"

public record AlertaProyectoDto(
    long Id, long ProyectoId, TipoAlerta TipoAlerta,
    SeveridadAlerta Severidad, string Titulo, string Mensaje,
    DateTime FechaAlerta, bool Leida, bool Resuelta);