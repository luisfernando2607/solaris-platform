namespace SolarisPlatform.Domain.Enums.RRHH;

public enum EstadoEmpleado : short
{
    Activo      = 1,
    Licencia    = 2,
    Vacaciones  = 3,
    Egresado    = 4
}

public enum TipoContrato : short
{
    Indefinido  = 1,
    PlazoFijo   = 2,
    Obra        = 3,
    Pasantia    = 4
}

public enum ModalidadTrabajo : short
{
    Presencial  = 1,
    Remoto      = 2,
    Hibrido     = 3
}

public enum JornadaLaboral : short
{
    Completa    = 1,
    Parcial     = 2
}

public enum TipoCambioHistorial : short
{
    Ingreso         = 1,
    CambioPuesto    = 2,
    CambioSalario   = 3,
    CambioDepartamento = 4,
    Licencia        = 5,
    Egreso          = 6
}

public enum MotivoEgreso : short
{
    Renuncia        = 1,
    Despido         = 2,
    Jubilacion      = 3,
    Fallecimiento   = 4,
    VencimientoContrato = 5
}

public enum EstadoRequisicion : short
{
    Borrador    = 1,
    Aprobada    = 2,
    EnProceso   = 3,
    Cerrada     = 4,
    Cancelada   = 5
}

public enum EtapaPostulacion : short
{
    Recibida        = 1,
    RevisionCV      = 2,
    Entrevista      = 3,
    Pruebas         = 4,
    Oferta          = 5,
    Contratado      = 6,
    Descartado      = 7
}

public enum TipoEntrevista : short
{
    Telefonica  = 1,
    Video       = 2,
    Presencial  = 3,
    Tecnica     = 4
}

public enum EstadoAsistencia : short
{
    Presente    = 1,
    Ausente     = 2,
    Tardanza    = 3,
    MedioDia    = 4,
    Feriado     = 5
}

public enum TipoAusencia : short
{
    Vacacion            = 1,
    PermisoPersonal     = 2,
    LicenciaMedica      = 3,
    Maternidad          = 4,
    Calamidad           = 5
}

public enum EstadoSolicitudAusencia : short
{
    Pendiente   = 1,
    Aprobada    = 2,
    Rechazada   = 3,
    Cancelada   = 4
}

public enum TipoConceptoNomina : short
{
    Ingreso             = 1,
    Descuento           = 2,
    AportePatronal      = 3
}

public enum FormaCalculoConcepto : short
{
    ValorFijo           = 1,
    PorcentajeSalario   = 2,
    PorcentajeOtro      = 3,
    Formula             = 4,
    Manual              = 5
}

public enum TipoPeriodoNomina : short
{
    Mensual     = 1,
    Quincenal   = 2,
    Semanal     = 3
}

public enum EstadoPeriodoNomina : short
{
    Abierto     = 1,
    EnProceso   = 2,
    Cerrado     = 3,
    Pagado      = 4
}

public enum TipoRolPago : short
{
    Normal      = 1,
    Liquidacion = 2,
    Decimo13    = 3,
    Decimo14    = 4,
    Utilidades  = 5,
    Especial    = 6
}

public enum EstadoRolPago : short
{
    Borrador    = 1,
    Calculado   = 2,
    Aprobado    = 3,
    Pagado      = 4,
    Anulado     = 5
}

public enum EstadoPagoempleado : short
{
    Pendiente   = 1,
    Pagado      = 2,
    Rechazado   = 3
}

public enum TipoEvaluador : short
{
    Autoevaluacion  = 1,
    JefeDirecto     = 2,
    Par             = 3,
    Subordinado     = 4,
    Cliente         = 5
}

public enum EstadoEvaluacion : short
{
    Pendiente   = 1,
    EnProceso   = 2,
    Completada  = 3,
    Revisada    = 4
}

public enum TipoCapacitacion : short
{
    Interna     = 1,
    Externa     = 2,
    Online      = 3,
    Taller      = 4
}

public enum EstadoCapacitacion : short
{
    Planificada = 1,
    EnCurso     = 2,
    Finalizada  = 3,
    Cancelada   = 4
}

public enum TipoPrestamo : short
{
    Prestamo        = 1,
    Anticipo        = 2,
    FondoReserva    = 3
}

public enum NivelJerarquico : short
{
    Operativo   = 1,
    Supervisor  = 2,
    Gerencia    = 3,
    Direccion   = 4
}
