namespace SolarisPlatform.Domain.Enums.Proyectos;

// ──────────────────────────────────────────────────────────────────
// ENUMS MÓDULO PROYECTOS - Solaris Platform
// Namespace: SolarisPlatform.Domain.Enums.Proyectos
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Tipo de proyecto
/// </summary>
public enum TipoProyecto : byte
{
    NuevaObra     = 1,
    Mantenimiento = 2,
    Expansion     = 3,
    Emergencia    = 4
}

/// <summary>
/// Estado del proyecto
/// </summary>
public enum EstadoProyecto : byte
{
    Borrador      = 1,
    Planificacion = 2,
    Activo        = 3,
    Pausado       = 4,
    Cerrado       = 5,
    Cancelado     = 6
}

/// <summary>
/// Prioridad del proyecto
/// </summary>
public enum PrioridadProyecto : byte
{
    Baja    = 1,
    Media   = 2,
    Alta    = 3,
    Critica = 4
}

/// <summary>
/// Estado de un hito
/// </summary>
public enum EstadoHito : byte
{
    Pendiente  = 1,
    Logrado    = 2,
    Retrasado  = 3,
    Cancelado  = 4
}

/// <summary>
/// Estado de una fase
/// </summary>
public enum EstadoFase : byte
{
    Pendiente   = 1,
    EnCurso     = 2,
    Completada  = 3,
    Cancelada   = 4
}

/// <summary>
/// Tipo de documento adjunto
/// </summary>
public enum TipoDocumento : byte
{
    Contrato = 1,
    Plano    = 2,
    Permiso  = 3,
    Foto     = 4,
    Informe  = 5,
    Otro     = 6
}

/// <summary>
/// Tipo de nodo WBS
/// </summary>
public enum TipoNodoWbs : byte
{
    Entregable = 1,
    Paquete    = 2,
    Tarea      = 3
}

/// <summary>
/// Estado de una tarea
/// </summary>
public enum EstadoTarea : byte
{
    Pendiente   = 1,
    EnCurso     = 2,
    Bloqueada   = 3,
    Completada  = 4,
    Cancelada   = 5
}

/// <summary>
/// Prioridad de una tarea
/// </summary>
public enum PrioridadTarea : byte
{
    Baja   = 1,
    Media  = 2,
    Alta   = 3,
    Urgente= 4
}

/// <summary>
/// Tipo de dependencia entre tareas
/// </summary>
public enum TipoDependencia : byte
{
    /// <summary>FinInicio: B no puede empezar hasta que A termine</summary>
    FinInicio       = 1,
    /// <summary>InicioInicio: B no puede empezar hasta que A empiece</summary>
    InicioInicio    = 2,
    /// <summary>FinFin: B no puede terminar hasta que A termine</summary>
    FinFin          = 3,
    /// <summary>InicioFin: B no puede terminar hasta que A empiece</summary>
    InicioFin       = 4
}

/// <summary>
/// Tipo de recurso del proyecto
/// </summary>
public enum TipoRecurso : byte
{
    Humano    = 1,
    Material  = 2,
    Equipo    = 3,
    Servicio  = 4
}

/// <summary>
/// Unidad de medida del recurso
/// </summary>
public enum UnidadRecurso : byte
{
    Hora   = 1,
    Dia    = 2,
    Unidad = 3,
    M2     = 4,
    M3     = 5,
    Ml     = 6,
    Kg     = 7,
    Global = 8
}

/// <summary>
/// Tipo de partida de presupuesto
/// </summary>
public enum TipoPartida : byte
{
    Ingreso    = 1,
    Egreso     = 2,
    Contingencia = 3
}

/// <summary>
/// Origen de un costo real
/// </summary>
public enum OrigenCosto : byte
{
    Manual       = 1,
    OrdenTrabajo = 2,
    Factura      = 3,
    Nomina       = 4
}

/// <summary>
/// Estado de la orden de trabajo
/// </summary>
public enum EstadoOrdenTrabajo : byte
{
    Borrador    = 1,
    Asignada    = 2,
    EnCurso     = 3,
    Pausada     = 4,
    Completada  = 5,
    Rechazada   = 6,
    Cancelada   = 7
}

/// <summary>
/// Tipo de KPI
/// </summary>
public enum TipoKpi : byte
{
    /// <summary>SPI - Schedule Performance Index</summary>
    SPI = 1,
    /// <summary>CPI - Cost Performance Index</summary>
    CPI = 2,
    /// <summary>EAC - Estimate At Completion</summary>
    EAC = 3,
    /// <summary>ETC - Estimate To Complete</summary>
    ETC = 4,
    /// <summary>VAC - Variance At Completion</summary>
    VAC = 5,
    /// <summary>CV  - Cost Variance</summary>
    CV  = 6,
    /// <summary>SV  - Schedule Variance</summary>
    SV  = 7,
    /// <summary>TCPI - To-Complete Performance Index</summary>
    TCPI = 8,
    Personalizado = 99
}

/// <summary>
/// Tipo de alerta del proyecto
/// </summary>
public enum TipoAlerta : byte
{
    RetrasoFecha      = 1,
    SobrecostoPresup  = 2,
    TareaVencida      = 3,
    HitoEnRiesgo      = 4,
    RecursoSinAsignar = 5,
    Otro              = 9
}

/// <summary>
/// Severidad de una alerta
/// </summary>
public enum SeveridadAlerta : byte
{
    Info     = 1,
    Warning  = 2,
    Critica  = 3
}
