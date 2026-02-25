// =====================================================================
// SOLARIS PLATFORM - MÓDULO RRHH
// Entidades del Domain Layer (.NET 10)
// =====================================================================
// Carpeta sugerida: src/SolarisPlatform.Domain/Entities/RRHH/
// Convención: hereda de BaseEntity (igual que el resto del proyecto)
// =====================================================================

using SolarisPlatform.Domain.Common;

namespace SolarisPlatform.Domain.Entities.RRHH;

// ─────────────────────────────────────────────────────────────────────
// ESTRUCTURA ORGANIZACIONAL
// ─────────────────────────────────────────────────────────────────────

public class Departamento : BaseEntity
{
    public long EmpresaId { get; set; }
    public long? DepartamentoPadreId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public long? ResponsableId { get; set; }
    public decimal? PresupuestoAnual { get; set; }
    public int Nivel { get; set; } = 1;

    // Navegación
    public Departamento? DepartamentoPadre { get; set; }
    public ICollection<Departamento> SubDepartamentos { get; set; } = new List<Departamento>();
    public Empleado? Responsable { get; set; }
    public ICollection<Puesto> Puestos { get; set; } = new List<Puesto>();
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}

public class Puesto : BaseEntity
{
    public long EmpresaId { get; set; }
    public long DepartamentoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short NivelJerarquico { get; set; } = 1;        // 1=Operativo, 2=Supervisor, 3=Gerencia, 4=Dirección
    public decimal? BandaSalarialMin { get; set; }
    public decimal? BandaSalarialMax { get; set; }
    public long? MonedaId { get; set; }
    public bool RequiereTitulo { get; set; } = false;

    // Navegación
    public Departamento? Departamento { get; set; }
    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}

// ─────────────────────────────────────────────────────────────────────
// EMPLEADOS
// ─────────────────────────────────────────────────────────────────────

public class Empleado : BaseEntity
{
    public long EmpresaId { get; set; }
    public long? SucursalId { get; set; }
    // Identificación
    public string Codigo { get; set; } = string.Empty;
    public string TipoIdentificacion { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    // Datos personales
    public string PrimerNombre { get; set; } = string.Empty;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public short? Genero { get; set; }              // 1=M, 2=F, 3=Otro
    public short? EstadoCivil { get; set; }
    public long? NacionalidadPaisId { get; set; }
    // Contacto
    public string? EmailPersonal { get; set; }
    public string? EmailCorporativo { get; set; }
    public string? TelefonoCelular { get; set; }
    public string? TelefonoFijo { get; set; }
    // Dirección
    public long? PaisId { get; set; }
    public long? EstadoProvinciaId { get; set; }
    public long? CiudadId { get; set; }
    public string? Direccion { get; set; }
    // Datos laborales
    public long? DepartamentoId { get; set; }
    public long? PuestoId { get; set; }
    public long? JefeDirectoId { get; set; }
    public DateOnly FechaIngreso { get; set; }
    public DateOnly? FechaEgreso { get; set; }
    public short TipoContrato { get; set; } = 1;   // 1=Indefinido, 2=Plazo Fijo, 3=Obra, 4=Pasantía
    public short ModalidadTrabajo { get; set; } = 1;
    public short JornadaLaboral { get; set; } = 1;
    public decimal HorasSemanales { get; set; } = 40;
    // Salario
    public decimal SalarioBase { get; set; }
    public long? MonedaId { get; set; }
    // Seguridad social
    public string? NumeroSeguroSocial { get; set; }
    public string? NumeroAfiliacion { get; set; }
    // Banco
    public long? BancoId { get; set; }
    public short? TipoCuentaBancaria { get; set; }
    public string? NumeroCuentaBancaria { get; set; }
    // Estado
    public short Estado { get; set; } = 1;         // 1=Activo, 2=Licencia, 3=Vacaciones, 4=Egresado
    public short? MotivoEgreso { get; set; }
    public string? DescripcionEgreso { get; set; }
    public string? FotoUrl { get; set; }
    public long? UsuarioId { get; set; }

    // Propiedad calculada (no mapeada a BD - se usa la columna GENERATED)
    public string NombreCompleto =>
        $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}".Trim()
        .Replace("  ", " ");

    // Navegación
    public Departamento? Departamento { get; set; }
    public Puesto? Puesto { get; set; }
    public Empleado? JefeDirecto { get; set; }
    public ICollection<EmpleadoHistorial> Historial { get; set; } = new List<EmpleadoHistorial>();
    public ICollection<EmpleadoDocumento> Documentos { get; set; } = new List<EmpleadoDocumento>();
    public ICollection<EmpleadoConcepto> Conceptos { get; set; } = new List<EmpleadoConcepto>();
    public ICollection<SaldoVacaciones> SaldosVacaciones { get; set; } = new List<SaldoVacaciones>();
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}

public class EmpleadoHistorial : BaseEntity
{
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public short TipoCambio { get; set; }           // 1=Ingreso, 2=Cambio Puesto, 3=Cambio Salario, etc.
    public DateOnly FechaEfectiva { get; set; }
    public long? DepartamentoAnteriorId { get; set; }
    public long? PuestoAnteriorId { get; set; }
    public decimal? SalarioAnterior { get; set; }
    public short? TipoContratoAnterior { get; set; }
    public long? DepartamentoNuevoId { get; set; }
    public long? PuestoNuevoId { get; set; }
    public decimal? SalarioNuevo { get; set; }
    public short? TipoContratoNuevo { get; set; }
    public string? Motivo { get; set; }
    public string? Observaciones { get; set; }

    public Empleado? Empleado { get; set; }
}

public class EmpleadoDocumento : BaseEntity
{
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public short TipoDocumento { get; set; }        // 1=Contrato, 2=Título, 3=Certificado, 4=ID, 5=Otro
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ArchivoUrl { get; set; }
    public DateOnly? FechaDocumento { get; set; }
    public DateOnly? FechaVencimiento { get; set; }

    public Empleado? Empleado { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// RECLUTAMIENTO
// ─────────────────────────────────────────────────────────────────────

public class RequisicionPersonal : BaseEntity
{
    public long EmpresaId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public long DepartamentoId { get; set; }
    public long PuestoId { get; set; }
    public long SolicitanteId { get; set; }
    public short Motivo { get; set; }               // 1=Vacante, 2=Nueva Plaza, 3=Reemplazo
    public int CantidadPlazas { get; set; } = 1;
    public DateOnly FechaSolicitud { get; set; }
    public DateOnly? FechaRequerida { get; set; }
    public decimal? SalarioOfrecidoMin { get; set; }
    public decimal? SalarioOfrecidoMax { get; set; }
    public string? DescripcionPerfil { get; set; }
    public string? Requisitos { get; set; }
    public short Estado { get; set; } = 1;          // 1=Borrador, 2=Aprobada, 3=En Proceso, 4=Cerrada, 5=Cancelada
    public long? AprobadoPorId { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? Observaciones { get; set; }

    public Departamento? Departamento { get; set; }
    public Puesto? Puesto { get; set; }
    public Empleado? Solicitante { get; set; }
    public ICollection<ProcesoSeleccion> Procesos { get; set; } = new List<ProcesoSeleccion>();
}

public class Candidato : BaseEntity
{
    public long EmpresaId { get; set; }
    public string PrimerNombre { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? TipoIdentificacion { get; set; }
    public string? NumeroIdentificacion { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public long? PaisId { get; set; }
    public long? CiudadId { get; set; }
    public string? CvUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public short? NivelEducacion { get; set; }
    public short Estado { get; set; } = 1;

    public ICollection<Postulacion> Postulaciones { get; set; } = new List<Postulacion>();
}

public class ProcesoSeleccion : BaseEntity
{
    public long EmpresaId { get; set; }
    public long RequisicionId { get; set; }
    public long ResponsableId { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaCierre { get; set; }
    public short Estado { get; set; } = 1;
    public string? Observaciones { get; set; }

    public RequisicionPersonal? Requisicion { get; set; }
    public Empleado? Responsable { get; set; }
    public ICollection<Postulacion> Postulaciones { get; set; } = new List<Postulacion>();
}

public class Postulacion : BaseEntity
{
    public long ProcesoId { get; set; }
    public long CandidatoId { get; set; }
    public long EmpresaId { get; set; }
    public short? Fuente { get; set; }
    public short EtapaActual { get; set; } = 1;    // 1=Recibida...7=Descartado
    public decimal? Calificacion { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaPostulacion { get; set; } = DateTime.UtcNow;

    public ProcesoSeleccion? Proceso { get; set; }
    public Candidato? Candidato { get; set; }
    public ICollection<Entrevista> Entrevistas { get; set; } = new List<Entrevista>();
}

public class Entrevista : BaseEntity
{
    public long PostulacionId { get; set; }
    public long EmpresaId { get; set; }
    public short Tipo { get; set; } = 1;
    public long EntrevistadorId { get; set; }
    public DateTime FechaProgramada { get; set; }
    public DateTime? FechaRealizada { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? EnlaceVideo { get; set; }
    public decimal? Calificacion { get; set; }
    public short? Resultado { get; set; }
    public string? Observaciones { get; set; }
    public short Estado { get; set; } = 1;

    public Postulacion? Postulacion { get; set; }
    public Empleado? Entrevistador { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// TIEMPO Y ASISTENCIA
// ─────────────────────────────────────────────────────────────────────

public class Horario : BaseEntity
{
    public long EmpresaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short Tipo { get; set; } = 1;            // 1=Fijo, 2=Rotativo, 3=Flexible
    public TimeOnly? HoraEntrada { get; set; }
    public TimeOnly? HoraSalida { get; set; }
    public decimal HorasDiarias { get; set; } = 8;
    public string? DiasLaborables { get; set; }     // JSON: {"lun":true,...}
    public int ToleranciEntradaMin { get; set; } = 0;
    public int ToleranciaSalidaMin { get; set; } = 0;
}

public class Asistencia : BaseEntity
{
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public DateOnly Fecha { get; set; }
    public DateTime? HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public DateTime? HoraEntradaAlmuerzo { get; set; }
    public DateTime? HoraSalidaAlmuerzo { get; set; }
    public decimal? HorasTrabajadas { get; set; }
    public decimal HorasExtras { get; set; } = 0;
    public int MinutosTardanza { get; set; } = 0;
    public short Estado { get; set; } = 1;
    public short? TipoAusencia { get; set; }
    public string? Justificacion { get; set; }
    public short FuenteRegistro { get; set; } = 1;

    public Empleado? Empleado { get; set; }
}

public class SolicitudAusencia : BaseEntity
{
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public short Tipo { get; set; }                 // 1=Vacaciones, 2=Permiso, 3=Licencia Médica, etc.
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public decimal? DiasHabiles { get; set; }
    public string? Motivo { get; set; }
    public string? DocumentoUrl { get; set; }
    public short Estado { get; set; } = 1;
    public long? AprobadorId { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? ObservacionAprobador { get; set; }

    public Empleado? Empleado { get; set; }
    public Empleado? Aprobador { get; set; }
}

public class SaldoVacaciones : BaseEntity
{
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public int Anno { get; set; }
    public decimal DiasGanados { get; set; } = 0;
    public decimal DiasTomados { get; set; } = 0;
    public decimal DiasVencidos { get; set; } = 0;
    public decimal DiasPendientes => DiasGanados - DiasTomados - DiasVencidos;

    public Empleado? Empleado { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// NÓMINA / PLANILLA
// ─────────────────────────────────────────────────────────────────────

public class ConceptoNomina : BaseEntity
{
    public long EmpresaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short Tipo { get; set; }                 // 1=Ingreso, 2=Descuento, 3=Aporte Patronal
    public short? Subtipo { get; set; }
    public short FormaCalculo { get; set; } = 1;    // 1=Fijo, 2=% Salario, 3=% Otro, 4=Fórmula, 5=Manual
    public decimal? Porcentaje { get; set; }
    public long? ConceptoBaseId { get; set; }
    public string? Formula { get; set; }
    public decimal? ValorFijo { get; set; }
    public bool GravableRenta { get; set; } = true;
    public bool GravableSeguridadSocial { get; set; } = true;
    public bool AplicaTodosEmpleados { get; set; } = false;
    public bool EsObligatorio { get; set; } = false;
    public bool EsSistema { get; set; } = false;
    public int OrdenCalculo { get; set; } = 0;
    public string? PaisCodigo { get; set; }

    public ConceptoNomina? ConceptoBase { get; set; }
}

public class PeriodoNomina : BaseEntity
{
    public long EmpresaId { get; set; }
    public int Anno { get; set; }
    public int NumeroPeriodo { get; set; }
    public short TipoPeriodo { get; set; } = 1;     // 1=Mensual, 2=Quincenal, 3=Semanal
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public DateOnly? FechaPago { get; set; }
    public short Estado { get; set; } = 1;          // 1=Abierto, 2=En Proceso, 3=Cerrado, 4=Pagado

    public ICollection<RolPago> RolesPago { get; set; } = new List<RolPago>();
}

public class RolPago : BaseEntity
{
    public long EmpresaId { get; set; }
    public long PeriodoId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public short Tipo { get; set; } = 1;            // 1=Normal, 2=Liquidación, 3=Décimo13, etc.
    public string? Descripcion { get; set; }
    public decimal TotalIngresos { get; set; } = 0;
    public decimal TotalDescuentos { get; set; } = 0;
    public decimal TotalAportesPatronales { get; set; } = 0;
    public decimal TotalNeto { get; set; } = 0;
    public int TotalEmpleados { get; set; } = 0;
    public short Estado { get; set; } = 1;          // 1=Borrador, 2=Calculado, 3=Aprobado, 4=Pagado, 5=Anulado
    public long? AprobadoPorId { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? Observaciones { get; set; }

    public PeriodoNomina? Periodo { get; set; }
    public ICollection<RolPagoEmpleado> Empleados { get; set; } = new List<RolPagoEmpleado>();
}

public class RolPagoEmpleado : BaseEntity
{
    public long RolPagoId { get; set; }
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public string? PuestoNombre { get; set; }
    public string? DepartamentoNombre { get; set; }
    public decimal SalarioBase { get; set; }
    public decimal DiasTrabajados { get; set; } = 30;
    public decimal HorasExtras { get; set; } = 0;
    public decimal TotalIngresos { get; set; } = 0;
    public decimal TotalDescuentos { get; set; } = 0;
    public decimal TotalAportesPatronales { get; set; } = 0;
    public decimal NetoAPagar { get; set; } = 0;
    public short EstadoPago { get; set; } = 1;
    public DateOnly? FechaPago { get; set; }
    public string? ReferenciaPago { get; set; }

    public RolPago? RolPago { get; set; }
    public Empleado? Empleado { get; set; }
    public ICollection<RolPagoDetalle> Detalles { get; set; } = new List<RolPagoDetalle>();
}

public class RolPagoDetalle : BaseEntity
{
    public long RolPagoEmpleadoId { get; set; }
    public long ConceptoId { get; set; }
    public long EmpresaId { get; set; }
    public decimal? BaseCalculo { get; set; }
    public decimal? PorcentajeAplicado { get; set; }
    public decimal Cantidad { get; set; } = 1;
    public decimal Valor { get; set; }
    public short Tipo { get; set; }                 // 1=Ingreso, 2=Descuento, 3=Aporte Patronal

    public RolPagoEmpleado? RolPagoEmpleado { get; set; }
    public ConceptoNomina? Concepto { get; set; }
}

public class EmpleadoConcepto : BaseEntity
{
    public long EmpleadoId { get; set; }
    public long ConceptoId { get; set; }
    public long EmpresaId { get; set; }
    public decimal? Valor { get; set; }
    public decimal? Porcentaje { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public int? CuotasTotal { get; set; }
    public int CuotasPagadas { get; set; } = 0;
    public string? Observaciones { get; set; }

    public Empleado? Empleado { get; set; }
    public ConceptoNomina? Concepto { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// PRÉSTAMOS
// ─────────────────────────────────────────────────────────────────────

public class Prestamo : BaseEntity
{
    public long EmpresaId { get; set; }
    public long EmpleadoId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public short Tipo { get; set; } = 1;            // 1=Préstamo, 2=Anticipo, 3=Fondo Reserva
    public decimal MontoSolicitado { get; set; }
    public decimal? MontoAprobado { get; set; }
    public int NumeroCuotas { get; set; } = 1;
    public decimal? CuotaMensual { get; set; }
    public DateOnly FechaSolicitud { get; set; }
    public DateOnly? FechaAprobacion { get; set; }
    public DateOnly? FechaPrimerDescuento { get; set; }
    public string? Motivo { get; set; }
    public short Estado { get; set; } = 1;
    public long? AprobadoPorId { get; set; }
    public decimal SaldoPendiente { get; set; } = 0;

    public Empleado? Empleado { get; set; }
    public ICollection<PrestamoCuota> Cuotas { get; set; } = new List<PrestamoCuota>();
}

public class PrestamoCuota : BaseEntity
{
    public long PrestamoId { get; set; }
    public int NumeroCuota { get; set; }
    public DateOnly FechaDescuento { get; set; }
    public decimal ValorCuota { get; set; }
    public decimal ValorPagado { get; set; } = 0;
    public long? RolPagoDetalleId { get; set; }
    public short Estado { get; set; } = 1;          // 1=Pendiente, 2=Pagada, 3=Parcial

    public Prestamo? Prestamo { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// EVALUACIÓN DE DESEMPEÑO
// ─────────────────────────────────────────────────────────────────────

public class PlantillaEvaluacion : BaseEntity
{
    public long EmpresaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short Tipo { get; set; } = 1;            // 1=Autoevaluación, 2=Jefe, 3=360°, 4=KPIs
    public decimal EscalaMin { get; set; } = 1;
    public decimal EscalaMax { get; set; } = 5;
    public string? Instrucciones { get; set; }

    public ICollection<PlantillaCriterio> Criterios { get; set; } = new List<PlantillaCriterio>();
}

public class PlantillaCriterio : BaseEntity
{
    public long PlantillaId { get; set; }
    public long? CriterioPadreId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Peso { get; set; } = 100;
    public int Orden { get; set; } = 0;

    public PlantillaEvaluacion? Plantilla { get; set; }
    public PlantillaCriterio? CriterioPadre { get; set; }
    public ICollection<PlantillaCriterio> SubCriterios { get; set; } = new List<PlantillaCriterio>();
}

public class EvaluacionProceso : BaseEntity
{
    public long EmpresaId { get; set; }
    public long PlantillaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Anno { get; set; }
    public string? Periodo { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public short Estado { get; set; } = 1;

    public PlantillaEvaluacion? Plantilla { get; set; }
    public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
}

public class Evaluacion : BaseEntity
{
    public long ProcesoId { get; set; }
    public long EvaluadoId { get; set; }
    public long EvaluadorId { get; set; }
    public long EmpresaId { get; set; }
    public short TipoEvaluador { get; set; } = 1;   // 1=Auto, 2=Jefe, 3=Par, 4=Subordinado
    public decimal? PuntuacionTotal { get; set; }
    public short Estado { get; set; } = 1;
    public DateTime? FechaCompletado { get; set; }
    public string? ComentarioGeneral { get; set; }

    public EvaluacionProceso? Proceso { get; set; }
    public Empleado? Evaluado { get; set; }
    public Empleado? Evaluador { get; set; }
    public ICollection<EvaluacionRespuesta> Respuestas { get; set; } = new List<EvaluacionRespuesta>();
}

public class EvaluacionRespuesta : BaseEntity
{
    public long EvaluacionId { get; set; }
    public long CriterioId { get; set; }
    public decimal? Puntuacion { get; set; }
    public string? Comentario { get; set; }

    public Evaluacion? Evaluacion { get; set; }
    public PlantillaCriterio? Criterio { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// CAPACITACIÓN
// ─────────────────────────────────────────────────────────────────────

public class Capacitacion : BaseEntity
{
    public long EmpresaId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short Tipo { get; set; } = 1;            // 1=Interna, 2=Externa, 3=Online, 4=Taller
    public short Modalidad { get; set; } = 1;
    public string? Instructor { get; set; }
    public string? Institucion { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public decimal? HorasDuracion { get; set; }
    public decimal? Costo { get; set; }
    public long? MonedaId { get; set; }
    public int? Cupos { get; set; }
    public string? DescripcionCertificado { get; set; }
    public short Estado { get; set; } = 1;

    public ICollection<CapacitacionParticipante> Participantes { get; set; } = new List<CapacitacionParticipante>();
}

public class CapacitacionParticipante : BaseEntity
{
    public long CapacitacionId { get; set; }
    public long EmpleadoId { get; set; }
    public long EmpresaId { get; set; }
    public short Estado { get; set; } = 1;
    public decimal? AsistenciaPorcentaje { get; set; }
    public decimal? Calificacion { get; set; }
    public string? CertificadoUrl { get; set; }
    public DateOnly? FechaCertificado { get; set; }
    public string? Observaciones { get; set; }

    public Capacitacion? Capacitacion { get; set; }
    public Empleado? Empleado { get; set; }
}

// ─────────────────────────────────────────────────────────────────────
// PARÁMETROS DE NÓMINA
// ─────────────────────────────────────────────────────────────────────

public class ParametroNomina : BaseEntity
{
    public long EmpresaId { get; set; }
    public string PaisCodigo { get; set; } = string.Empty;
    public int Anno { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string TipoDato { get; set; } = "DECIMAL";   // DECIMAL, INT, BOOL, TEXT
}
