using SolarisPlatform.Domain.Entities.RRHH;

namespace SolarisPlatform.Domain.Interfaces.RRHH;

// ─────────────────────────────────────────────────────
// ESTRUCTURA ORGANIZACIONAL
// ─────────────────────────────────────────────────────

public interface IDepartamentoRepository
{
    Task<IEnumerable<Departamento>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivos = true);
    Task<Departamento?> ObtenerPorIdAsync(long id);
    Task<Departamento?> ObtenerConHijosAsync(long id);
    Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null);
    Task<Departamento> CrearAsync(Departamento departamento);
    Task<Departamento> ActualizarAsync(Departamento departamento);
    Task EliminarAsync(long id);
}

public interface IPuestoRepository
{
    Task<IEnumerable<Puesto>> ObtenerPorEmpresaAsync(long empresaId, long? departamentoId = null, bool soloActivos = true);
    Task<Puesto?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null);
    Task<Puesto> CrearAsync(Puesto puesto);
    Task<Puesto> ActualizarAsync(Puesto puesto);
    Task EliminarAsync(long id);
}

// ─────────────────────────────────────────────────────
// EMPLEADOS
// ─────────────────────────────────────────────────────

public interface IEmpleadoRepository
{
    Task<(IEnumerable<Empleado> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        string? busqueda = null, long? departamentoId = null,
        long? puestoId = null, short? estado = null);
    Task<Empleado?> ObtenerPorIdAsync(long id);
    Task<Empleado?> ObtenerFichaCompletaAsync(long id);
    Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null);
    Task<bool> ExisteIdentificacionAsync(long empresaId, string tipo, string numero, long? excludeId = null);
    Task<IEnumerable<EmpleadoHistorial>> ObtenerHistorialAsync(long empleadoId);
    Task<IEnumerable<EmpleadoDocumento>> ObtenerDocumentosAsync(long empleadoId);
    Task<Empleado> CrearAsync(Empleado empleado);
    Task<Empleado> ActualizarAsync(Empleado empleado);
    Task<EmpleadoHistorial> RegistrarHistorialAsync(EmpleadoHistorial historial);
    Task<EmpleadoDocumento> AgregarDocumentoAsync(EmpleadoDocumento documento);
    Task EliminarDocumentoAsync(long documentoId);
}

// ─────────────────────────────────────────────────────
// RECLUTAMIENTO
// ─────────────────────────────────────────────────────

public interface IRequisicionPersonalRepository
{
    Task<(IEnumerable<RequisicionPersonal> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, short? estado = null);
    Task<RequisicionPersonal?> ObtenerPorIdAsync(long id);
    Task<string> GenerarNumeroAsync(long empresaId);
    Task<RequisicionPersonal> CrearAsync(RequisicionPersonal requisicion);
    Task<RequisicionPersonal> ActualizarAsync(RequisicionPersonal requisicion);
}

public interface ICandidatoRepository
{
    Task<(IEnumerable<Candidato> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, string? busqueda = null);
    Task<Candidato?> ObtenerPorIdAsync(long id);
    Task<Candidato?> ObtenerPorEmailAsync(long empresaId, string email);
    Task<Candidato> CrearAsync(Candidato candidato);
    Task<Candidato> ActualizarAsync(Candidato candidato);
}

public interface IPostulacionRepository
{
    Task<IEnumerable<Postulacion>> ObtenerPorProcesoAsync(long procesoId);
    Task<Postulacion?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteAsync(long procesoId, long candidatoId);
    Task<Postulacion> CrearAsync(Postulacion postulacion);
    Task<Postulacion> ActualizarAsync(Postulacion postulacion);
}

// ─────────────────────────────────────────────────────
// ASISTENCIA
// ─────────────────────────────────────────────────────

public interface IAsistenciaRepository
{
    Task<IEnumerable<Asistencia>> ObtenerPorEmpleadoMesAsync(long empleadoId, int anno, int mes);
    Task<IEnumerable<Asistencia>> ObtenerPorEmpresaFechaAsync(long empresaId, DateOnly fecha);
    Task<Asistencia?> ObtenerPorEmpleadoFechaAsync(long empleadoId, DateOnly fecha);
    Task<Asistencia> CrearAsync(Asistencia asistencia);
    Task<Asistencia> ActualizarAsync(Asistencia asistencia);
}

public interface ISolicitudAusenciaRepository
{
    Task<(IEnumerable<SolicitudAusencia> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano,
        long? empleadoId = null, short? estado = null);
    Task<SolicitudAusencia?> ObtenerPorIdAsync(long id);
    Task<SolicitudAusencia> CrearAsync(SolicitudAusencia solicitud);
    Task<SolicitudAusencia> ActualizarAsync(SolicitudAusencia solicitud);
}

public interface ISaldoVacacionesRepository
{
    Task<SaldoVacaciones?> ObtenerAsync(long empleadoId, int anno);
    Task<IEnumerable<SaldoVacaciones>> ObtenerPorEmpleadoAsync(long empleadoId);
    Task<SaldoVacaciones> CrearOActualizarAsync(SaldoVacaciones saldo);
}

// ─────────────────────────────────────────────────────
// NÓMINA
// ─────────────────────────────────────────────────────

public interface IConceptoNominaRepository
{
    Task<IEnumerable<ConceptoNomina>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivos = true);
    Task<ConceptoNomina?> ObtenerPorIdAsync(long id);
    Task<bool> ExisteCodigoAsync(long empresaId, string codigo, long? excludeId = null);
    Task<ConceptoNomina> CrearAsync(ConceptoNomina concepto);
    Task<ConceptoNomina> ActualizarAsync(ConceptoNomina concepto);
    Task EliminarAsync(long id);
}

public interface IPeriodoNominaRepository
{
    Task<IEnumerable<PeriodoNomina>> ObtenerPorEmpresaAnnoAsync(long empresaId, int anno);
    Task<PeriodoNomina?> ObtenerPorIdAsync(long id);
    Task<PeriodoNomina?> ObtenerActualAsync(long empresaId);
    Task<PeriodoNomina> CrearAsync(PeriodoNomina periodo);
    Task<PeriodoNomina> ActualizarAsync(PeriodoNomina periodo);
}

public interface IRolPagoRepository
{
    Task<(IEnumerable<RolPago> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? periodoId = null);
    Task<RolPago?> ObtenerPorIdAsync(long id);
    Task<RolPago?> ObtenerConDetalleAsync(long id);
    Task<string> GenerarNumeroAsync(long empresaId);
    Task<RolPago> CrearAsync(RolPago rolPago);
    Task<RolPago> ActualizarAsync(RolPago rolPago);
    Task<RolPagoEmpleado> CrearEmpleadoAsync(RolPagoEmpleado empleado);
    Task<RolPagoEmpleado> ActualizarEmpleadoAsync(RolPagoEmpleado empleado);
    Task<IEnumerable<RolPagoDetalle>> ObtenerDetalleEmpleadoAsync(long rolPagoEmpleadoId);
    Task GuardarDetallesAsync(IEnumerable<RolPagoDetalle> detalles);
}

// ─────────────────────────────────────────────────────
// PRÉSTAMOS
// ─────────────────────────────────────────────────────

public interface IPrestamoRepository
{
    Task<(IEnumerable<Prestamo> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, long? empleadoId = null, short? estado = null);
    Task<Prestamo?> ObtenerPorIdAsync(long id);
    Task<IEnumerable<PrestamoCuota>> ObtenerCuotasAsync(long prestamoId);
    Task<string> GenerarNumeroAsync(long empresaId);
    Task<Prestamo> CrearAsync(Prestamo prestamo);
    Task<Prestamo> ActualizarAsync(Prestamo prestamo);
    Task<IEnumerable<PrestamoCuota>> ObtenerCuotasPendientesAsync(long empleadoId, DateOnly hasta);
}

// ─────────────────────────────────────────────────────
// EVALUACIÓN
// ─────────────────────────────────────────────────────

public interface IEvaluacionRepository
{
    Task<IEnumerable<EvaluacionProceso>> ObtenerProcesosAsync(long empresaId);
    Task<EvaluacionProceso?> ObtenerProcesoAsync(long id);
    Task<IEnumerable<Evaluacion>> ObtenerPorEvaluadorAsync(long evaluadorId, long empresaId, short? estado = null);
    Task<Evaluacion?> ObtenerPorIdAsync(long id);
    Task<EvaluacionProceso> CrearProcesoAsync(EvaluacionProceso proceso);
    Task<Evaluacion> CrearAsync(Evaluacion evaluacion);
    Task<Evaluacion> ActualizarAsync(Evaluacion evaluacion);
    Task GuardarRespuestasAsync(IEnumerable<EvaluacionRespuesta> respuestas);
}

// ─────────────────────────────────────────────────────
// CAPACITACIÓN
// ─────────────────────────────────────────────────────

public interface ICapacitacionRepository
{
    Task<(IEnumerable<Capacitacion> Items, int Total)> ObtenerPaginadoAsync(
        long empresaId, int pagina, int tamano, short? estado = null);
    Task<Capacitacion?> ObtenerPorIdAsync(long id);
    Task<Capacitacion?> ObtenerConParticipantesAsync(long id);
    Task<Capacitacion> CrearAsync(Capacitacion capacitacion);
    Task<Capacitacion> ActualizarAsync(Capacitacion capacitacion);
    Task InscribirEmpleadosAsync(IEnumerable<CapacitacionParticipante> participantes);
    Task<CapacitacionParticipante> ActualizarParticipanteAsync(CapacitacionParticipante participante);
}

// ─────────────────────────────────────────────────────
// PARÁMETROS
// ─────────────────────────────────────────────────────

public interface IParametroNominaRepository
{
    Task<IEnumerable<ParametroNomina>> ObtenerAsync(long empresaId, string paisCodigo, int anno);
    Task<ParametroNomina?> ObtenerAsync(long empresaId, string paisCodigo, int anno, string clave);
    Task<decimal> ObtenerValorDecimalAsync(long empresaId, string paisCodigo, int anno, string clave, decimal defaultValue = 0);
    Task<ParametroNomina> CrearOActualizarAsync(ParametroNomina parametro);
}
