namespace SolarisPlatform.Domain.Enums;

public enum EstadoUsuario
{
    Inactivo = 0,
    Activo = 1,
    Bloqueado = 2,
    Pendiente = 3
}

public enum EstadoEmpresa
{
    Inactivo = 0,
    Activo = 1,
    Suspendido = 2,
    Prueba = 3
}

public enum TipoEventoAcceso
{
    Login,
    Logout,
    LoginFallido,
    SesionExpirada,
    CambioPassword,
    RecuperacionPassword,
    BloqueoUsuario,
    DesbloqueoUsuario
}

public enum AccionAuditoria
{
    Insert,
    Update,
    Delete
}

public enum NivelError
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

public enum PrioridadNotificacion
{
    Baja = 0,
    Normal = 1,
    Alta = 2,
    Urgente = 3
}

public enum EstadoColaNotificacion
{
    Pendiente,
    Procesando,
    Enviado,
    Error
}

public enum CanalNotificacion
{
    Email,
    Push,
    Sms,
    Interna
}

public enum TipoPermiso
{
    Accion,
    Menu,
    Dato
}

public enum Genero
{
    Masculino,
    Femenino,
    Otro
}

public enum TipoImpuesto
{
    IVA,
    Retencion,
    RetencionIVA
}

public enum TipoFormaPago
{
    Efectivo,
    Tarjeta,
    Transferencia,
    Cheque,
    Credito
}

public enum TipoDato
{
    String,
    Int,
    Decimal,
    Bool,
    Json,
    Date
}
