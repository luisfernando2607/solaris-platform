namespace SolarisPlatform.Application.DTOs.RRHH;

// ═══════════════════════════════════════════════════════════════
// DEPARTAMENTOS
// ═══════════════════════════════════════════════════════════════

public record DepartamentoDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    long? DepartamentoPadreId, string? DepartamentoPadreNombre,
    long? ResponsableId, string? ResponsableNombre,
    decimal? PresupuestoAnual, int Nivel, bool Activo,
    List<DepartamentoDto> SubDepartamentos);

public record CrearDepartamentoRequest(
    string Codigo, string Nombre, string? Descripcion,
    long? DepartamentoPadreId, long? ResponsableId, decimal? PresupuestoAnual);

public record ActualizarDepartamentoRequest(
    string Nombre, string? Descripcion,
    long? DepartamentoPadreId, long? ResponsableId,
    decimal? PresupuestoAnual, bool Activo);

// ═══════════════════════════════════════════════════════════════
// PUESTOS
// ═══════════════════════════════════════════════════════════════

public record PuestoDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    long DepartamentoId, string DepartamentoNombre,
    short NivelJerarquico, string NivelJerarquicoNombre,
    decimal? BandaSalarialMin, decimal? BandaSalarialMax,
    bool RequiereTitulo, bool Activo);

public record CrearPuestoRequest(
    string Codigo, string Nombre, string? Descripcion,
    long DepartamentoId, short NivelJerarquico,
    decimal? BandaSalarialMin, decimal? BandaSalarialMax,
    long? MonedaId, bool RequiereTitulo);

public record ActualizarPuestoRequest(
    string Nombre, string? Descripcion,
    long DepartamentoId, short NivelJerarquico,
    decimal? BandaSalarialMin, decimal? BandaSalarialMax,
    bool RequiereTitulo, bool Activo);

// ═══════════════════════════════════════════════════════════════
// EMPLEADOS
// ═══════════════════════════════════════════════════════════════

public record EmpleadoListaDto(
    long Id, string Codigo, string NombreCompleto, string NumeroIdentificacion,
    string? EmailCorporativo, string? TelefonoCelular,
    string? DepartamentoNombre, string? PuestoNombre,
    DateOnly FechaIngreso, decimal SalarioBase,
    short Estado, string EstadoNombre, string? FotoUrl);

public record EmpleadoFichaDto(
    long Id, string Codigo,
    // Identificación
    string TipoIdentificacion, string NumeroIdentificacion,
    // Nombres
    string PrimerNombre, string? SegundoNombre,
    string PrimerApellido, string? SegundoApellido,
    string NombreCompleto,
    // Personal
    DateOnly? FechaNacimiento, int? EdadAnios,
    short? Genero, string? GeneroNombre,
    short? EstadoCivil, string? EstadoCivilNombre,
    long? NacionalidadPaisId, string? NacionalidadNombre,
    // Contacto
    string? EmailPersonal, string? EmailCorporativo,
    string? TelefonoCelular, string? TelefonoFijo,
    // Dirección
    string? PaisNombre, string? EstadoProvinciaNombre, string? CiudadNombre, string? Direccion,
    // Laboral
    long? DepartamentoId, string? DepartamentoNombre,
    long? PuestoId, string? PuestoNombre,
    long? JefeDirectoId, string? JefeDirectoNombre,
    DateOnly FechaIngreso, DateOnly? FechaEgreso,
    short TipoContrato, string TipoContratoNombre,
    short ModalidadTrabajo, string ModalidadTrabajoNombre,
    decimal HorasSemanales, decimal SalarioBase, string? MonedaSimbolo,
    // Seguridad social
    string? NumeroSeguroSocial, string? NumeroAfiliacion,
    // Banco
    string? BancoNombre, short? TipoCuenta, string? NumeroCuenta,
    // Estado
    short Estado, string EstadoNombre, string? FotoUrl);

public record CrearEmpleadoRequest(
    string TipoIdentificacion, string NumeroIdentificacion,
    string PrimerNombre, string? SegundoNombre,
    string PrimerApellido, string? SegundoApellido,
    DateOnly? FechaNacimiento, short? Genero, short? EstadoCivil,
    long? NacionalidadPaisId,
    string? EmailPersonal, string? EmailCorporativo,
    string? TelefonoCelular, string? TelefonoFijo,
    long? PaisId, long? EstadoProvinciaId, long? CiudadId, string? Direccion,
    long? DepartamentoId, long? PuestoId, long? JefeDirectoId,
    DateOnly FechaIngreso, short TipoContrato, short ModalidadTrabajo,
    short JornadaLaboral, decimal HorasSemanales, decimal SalarioBase, long? MonedaId,
    string? NumeroSeguroSocial, string? NumeroAfiliacion,
    long? BancoId, short? TipoCuentaBancaria, string? NumeroCuentaBancaria,
    string? FotoUrl, long? UsuarioId);

public record ActualizarEmpleadoRequest(
    string PrimerNombre, string? SegundoNombre,
    string PrimerApellido, string? SegundoApellido,
    DateOnly? FechaNacimiento, short? Genero, short? EstadoCivil,
    string? EmailPersonal, string? EmailCorporativo,
    string? TelefonoCelular, string? TelefonoFijo,
    long? PaisId, long? EstadoProvinciaId, long? CiudadId, string? Direccion,
    long? DepartamentoId, long? PuestoId, long? JefeDirectoId,
    short TipoContrato, short ModalidadTrabajo, decimal HorasSemanales,
    string? NumeroSeguroSocial, string? NumeroAfiliacion,
    long? BancoId, short? TipoCuentaBancaria, string? NumeroCuentaBancaria,
    string? FotoUrl);

public record EgresarEmpleadoRequest(
    DateOnly FechaEgreso, short MotivoEgreso, string? Descripcion);

public record CambiarSalarioRequest(
    decimal NuevoSalario, DateOnly FechaEfectiva, string? Motivo);

public record CambiarPuestoRequest(
    long NuevoPuestoId, long? NuevoDepartamentoId,
    DateOnly FechaEfectiva, string? Motivo);

public record EmpleadoHistorialDto(
    long Id, short TipoCambio, string TipoCambioNombre,
    DateOnly FechaEfectiva,
    string? DepartamentoAnterior, string? PuestoAnterior, decimal? SalarioAnterior,
    string? DepartamentoNuevo, string? PuestoNuevo, decimal? SalarioNuevo,
    string? Motivo, string? Observaciones,
    DateTime FechaCreacion);

public record EmpleadoDocumentoDto(
    long Id, short TipoDocumento, string TipoDocumentoNombre,
    string Nombre, string? Descripcion, string? ArchivoUrl,
    DateOnly? FechaDocumento, DateOnly? FechaVencimiento, bool Activo);

public record AgregarDocumentoRequest(
    short TipoDocumento, string Nombre, string? Descripcion,
    string? ArchivoUrl, DateOnly? FechaDocumento, DateOnly? FechaVencimiento);

// ═══════════════════════════════════════════════════════════════
// ASISTENCIA
// ═══════════════════════════════════════════════════════════════

public record HorarioDto(
    long Id, string Codigo, string Nombre, short Tipo,
    TimeOnly? HoraEntrada, TimeOnly? HoraSalida,
    decimal HorasDiarias, int ToleranciEntradaMin, bool Activo);

public record CrearHorarioRequest(
    string Codigo, string Nombre, short Tipo,
    TimeOnly? HoraEntrada, TimeOnly? HoraSalida,
    decimal HorasDiarias, string? DiasLaborables,
    int ToleranciEntradaMin, int ToleranciaSalidaMin);

public record AsistenciaDto(
    long Id, long EmpleadoId, string EmpleadoNombre,
    DateOnly Fecha,
    DateTime? HoraEntrada, DateTime? HoraSalida,
    decimal? HorasTrabajadas, decimal HorasExtras, int MinutosTardanza,
    short Estado, string EstadoNombre, string? TipoAusenciaNombre,
    string? Justificacion, short FuenteRegistro);

public record MarcarAsistenciaRequest(
    long EmpleadoId, DateOnly Fecha, DateTime Hora,
    bool EsEntrada, short FuenteRegistro = 1);

public record SolicitudAusenciaDto(
    long Id, long EmpleadoId, string EmpleadoNombre,
    short Tipo, string TipoNombre,
    DateOnly FechaInicio, DateOnly FechaFin, decimal? DiasHabiles,
    string? Motivo, short Estado, string EstadoNombre,
    string? AprobadorNombre, DateTime? FechaAprobacion,
    string? ObservacionAprobador);

public record CrearSolicitudAusenciaRequest(
    long EmpleadoId, short Tipo,
    DateOnly FechaInicio, DateOnly FechaFin,
    string? Motivo, string? DocumentoUrl);

public record AprobarRechazarAusenciaRequest(
    bool Aprobada, string? Observacion);

public record SaldoVacacionesDto(
    long EmpleadoId, string EmpleadoNombre, int Anno,
    decimal DiasGanados, decimal DiasTomados,
    decimal DiasPendientes, decimal DiasVencidos);

// ═══════════════════════════════════════════════════════════════
// NÓMINA
// ═══════════════════════════════════════════════════════════════

public record ConceptoNominaDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    short Tipo, string TipoNombre,
    short FormaCalculo, string FormaCalculoNombre,
    decimal? Porcentaje, decimal? ValorFijo,
    bool GravableRenta, bool GravableSeguridadSocial,
    bool AplicaTodosEmpleados, bool EsObligatorio,
    int OrdenCalculo, string? PaisCodigo, bool Activo);

public record CrearConceptoNominaRequest(
    string Codigo, string Nombre, string? Descripcion,
    short Tipo, short? Subtipo, short FormaCalculo,
    decimal? Porcentaje, long? ConceptoBaseId, string? Formula,
    decimal? ValorFijo, bool GravableRenta, bool GravableSeguridadSocial,
    bool AplicaTodosEmpleados, bool EsObligatorio,
    int OrdenCalculo, string? PaisCodigo);

public record PeriodoNominaDto(
    long Id, int Anno, int NumeroPeriodo, short TipoPeriodo,
    string Descripcion, DateOnly FechaInicio, DateOnly FechaFin,
    DateOnly? FechaPago, short Estado, string EstadoNombre);

public record CrearPeriodoNominaRequest(
    int Anno, int NumeroPeriodo, short TipoPeriodo,
    string Descripcion, DateOnly FechaInicio, DateOnly FechaFin,
    DateOnly? FechaPago);

public record RolPagoDto(
    long Id, string Numero, long PeriodoId, string PeriodoDescripcion,
    short Tipo, string TipoNombre, string? Descripcion,
    decimal TotalIngresos, decimal TotalDescuentos,
    decimal TotalAportesPatronales, decimal TotalNeto,
    int TotalEmpleados, short Estado, string EstadoNombre,
    string? AprobadoPorNombre, DateTime? FechaAprobacion);

public record CrearRolPagoRequest(
    long PeriodoId, short Tipo, string? Descripcion);

public record RolPagoEmpleadoDto(
    long Id, long EmpleadoId, string EmpleadoNombre,
    string Codigo, string? PuestoNombre, string? DepartamentoNombre,
    decimal SalarioBase, decimal DiasTrabajados, decimal HorasExtras,
    decimal TotalIngresos, decimal TotalDescuentos,
    decimal TotalAportesPatronales, decimal NetoAPagar,
    short EstadoPago, string EstadoPagoNombre,
    List<RolPagoDetalleDto> Detalles);

public record RolPagoDetalleDto(
    long ConceptoId, string ConceptoCodigo, string ConceptoNombre,
    short Tipo, decimal? BaseCalculo, decimal? PorcentajeAplicado,
    decimal Cantidad, decimal Valor);

public record ParametroNominaDto(
    string PaisCodigo, int Anno, string Clave, string Valor,
    string? Descripcion, string TipoDato);

public record GuardarParametroRequest(
    string Clave, string Valor, string? Descripcion, string TipoDato = "DECIMAL");

// ═══════════════════════════════════════════════════════════════
// PRÉSTAMOS
// ═══════════════════════════════════════════════════════════════

public record PrestamoDto(
    long Id, string Numero, long EmpleadoId, string EmpleadoNombre,
    short Tipo, string TipoNombre, decimal MontoSolicitado,
    decimal? MontoAprobado, int NumeroCuotas, decimal? CuotaMensual,
    DateOnly FechaSolicitud, string? Motivo,
    short Estado, string EstadoNombre, decimal SaldoPendiente,
    List<PrestamoCuotaDto> Cuotas);

public record PrestamoCuotaDto(
    int NumeroCuota, DateOnly FechaDescuento,
    decimal ValorCuota, decimal ValorPagado,
    short Estado, string EstadoNombre);

public record CrearPrestamoRequest(
    long EmpleadoId, short Tipo, decimal MontoSolicitado,
    int NumeroCuotas, DateOnly FechaSolicitud,
    DateOnly? FechaPrimerDescuento, string? Motivo);

public record AprobarPrestamoRequest(
    decimal MontoAprobado, int NumeroCuotas,
    DateOnly FechaPrimerDescuento);

// ═══════════════════════════════════════════════════════════════
// EVALUACIÓN
// ═══════════════════════════════════════════════════════════════

public record PlantillaEvaluacionDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    short Tipo, decimal EscalaMin, decimal EscalaMax,
    bool Activo, List<PlantillaCriterioDto> Criterios);

public record PlantillaCriterioDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    decimal Peso, int Orden, long? CriterioPadreId,
    List<PlantillaCriterioDto> SubCriterios);

public record EvaluacionProcesoDto(
    long Id, string Nombre, string? Descripcion,
    int Anno, string? Periodo, DateOnly FechaInicio, DateOnly FechaFin,
    short Estado, string EstadoNombre,
    int TotalEvaluaciones, int EvaluacionesCompletadas);

public record EvaluacionDto(
    long Id, long EvaluadoId, string EvaluadoNombre,
    long EvaluadorId, string EvaluadorNombre,
    short TipoEvaluador, string TipoEvaluadorNombre,
    decimal? PuntuacionTotal, short Estado, string EstadoNombre,
    string? ComentarioGeneral, DateTime? FechaCompletado,
    List<EvaluacionRespuestaDto> Respuestas);

public record EvaluacionRespuestaDto(
    long CriterioId, string CriterioNombre,
    decimal Peso, decimal? Puntuacion, string? Comentario);

public record CompletarEvaluacionRequest(
    string? ComentarioGeneral,
    List<RespuestaRequest> Respuestas);

public record RespuestaRequest(long CriterioId, decimal? Puntuacion, string? Comentario);

// ═══════════════════════════════════════════════════════════════
// CAPACITACIÓN
// ═══════════════════════════════════════════════════════════════

public record CapacitacionDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    short Tipo, string TipoNombre, short Modalidad,
    string? Instructor, string? Institucion,
    DateOnly FechaInicio, DateOnly FechaFin,
    decimal? HorasDuracion, decimal? Costo,
    int? Cupos, short Estado, string EstadoNombre,
    int TotalInscritos);

public record CapacitacionDetalleDto(
    long Id, string Codigo, string Nombre, string? Descripcion,
    short Tipo, short Modalidad, string? Instructor, string? Institucion,
    DateOnly FechaInicio, DateOnly FechaFin,
    decimal? HorasDuracion, decimal? Costo, int? Cupos,
    short Estado, List<CapacitacionParticipanteDto> Participantes);

public record CapacitacionParticipanteDto(
    long EmpleadoId, string EmpleadoNombre,
    short Estado, string EstadoNombre,
    decimal? AsistenciaPorcentaje, decimal? Calificacion,
    string? CertificadoUrl, DateOnly? FechaCertificado);

public record CrearCapacitacionRequest(
    string Codigo, string Nombre, string? Descripcion,
    short Tipo, short Modalidad, string? Instructor, string? Institucion,
    DateOnly FechaInicio, DateOnly FechaFin,
    decimal? HorasDuracion, decimal? Costo, long? MonedaId,
    int? Cupos, string? DescripcionCertificado);

public record InscribirEmpleadosRequest(List<long> EmpleadoIds);

public record ActualizarParticipanteRequest(
    short Estado, decimal? AsistenciaPorcentaje,
    decimal? Calificacion, string? CertificadoUrl,
    DateOnly? FechaCertificado, string? Observaciones);

// ═══════════════════════════════════════════════════════════════
// DASHBOARD
// ═══════════════════════════════════════════════════════════════

public record RrhhDashboardDto(
    int TotalEmpleados, int EmpleadosActivos,
    int AusentesHoy, int SolicitudesPendientes,
    int NominaEmpleadosMes, decimal NominaTotalMes,
    int CapacitacionesActivas, int EvaluacionesPendientes,
    List<EmpleadosPorDepartamentoDto> EmpleadosPorDepartamento);

public record EmpleadosPorDepartamentoDto(string Departamento, int Total);
