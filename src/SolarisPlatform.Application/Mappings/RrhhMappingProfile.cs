using AutoMapper;
using SolarisPlatform.Application.DTOs.RRHH;
using SolarisPlatform.Domain.Entities.RRHH;

namespace SolarisPlatform.Application.Mappings;

public class RrhhMappingProfile : Profile
{
    public RrhhMappingProfile()
    {
        // ─── Departamento ───────────────────────────────────────
        CreateMap<Departamento, DepartamentoDto>()
            .ForMember(d => d.DepartamentoPadreNombre,
                o => o.MapFrom(s => s.DepartamentoPadre != null ? s.DepartamentoPadre.Nombre : null))
            .ForMember(d => d.ResponsableNombre,
                o => o.MapFrom(s => s.Responsable != null ? s.Responsable.NombreCompleto : null))
            .ForMember(d => d.SubDepartamentos,
                o => o.MapFrom(s => s.SubDepartamentos));

        CreateMap<CrearDepartamentoRequest, Departamento>();

        // ─── Puesto ─────────────────────────────────────────────
        CreateMap<Puesto, PuestoDto>()
            .ConstructUsing((s, ctx) => new PuestoDto(
                s.Id, s.Codigo, s.Nombre, s.Descripcion,
                s.DepartamentoId,
                s.Departamento != null ? s.Departamento.Nombre : string.Empty,
                s.NivelJerarquico,
                ObtenerNivelNombre(s.NivelJerarquico),
                s.BandaSalarialMin, s.BandaSalarialMax,
                s.RequiereTitulo, true))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearPuestoRequest, Puesto>();

        // ─── Empleado ───────────────────────────────────────────
        CreateMap<Empleado, EmpleadoListaDto>()
            .ConstructUsing((s, ctx) => new EmpleadoListaDto(
                s.Id, s.Codigo,
                s.NombreCompleto,
                s.NumeroIdentificacion,
                s.EmailCorporativo, s.TelefonoCelular,
                s.Departamento != null ? s.Departamento.Nombre : null,
                s.Puesto != null ? s.Puesto.Nombre : null,
                s.FechaIngreso, s.SalarioBase,
                s.Estado, ObtenerEstadoEmpleadoNombre(s.Estado),
                s.FotoUrl))
            .ForAllMembers(o => o.Ignore());

        CreateMap<Empleado, EmpleadoFichaDto>()
            .ForMember(d => d.EdadAnios,
                o => o.MapFrom(s => s.FechaNacimiento.HasValue
                    ? (int?)CalcularEdad(s.FechaNacimiento.Value) : null))
            .ForMember(d => d.GeneroNombre,
                o => o.MapFrom(s => s.Genero.HasValue ? ObtenerGeneroNombre(s.Genero.Value) : null))
            .ForMember(d => d.EstadoCivilNombre,
                o => o.MapFrom(s => s.EstadoCivil.HasValue ? ObtenerEstadoCivilNombre(s.EstadoCivil.Value) : null))
            .ForMember(d => d.DepartamentoNombre,
                o => o.MapFrom(s => s.Departamento != null ? s.Departamento.Nombre : null))
            .ForMember(d => d.PuestoNombre,
                o => o.MapFrom(s => s.Puesto != null ? s.Puesto.Nombre : null))
            .ForMember(d => d.JefeDirectoNombre,
                o => o.MapFrom(s => s.JefeDirecto != null ? s.JefeDirecto.NombreCompleto : null))
            .ForMember(d => d.TipoContratoNombre,
                o => o.MapFrom(s => ObtenerTipoContratoNombre(s.TipoContrato)))
            .ForMember(d => d.ModalidadTrabajoNombre,
                o => o.MapFrom(s => ObtenerModalidadNombre(s.ModalidadTrabajo)))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoEmpleadoNombre(s.Estado)));

        CreateMap<CrearEmpleadoRequest, Empleado>();

        // ─── Historial ──────────────────────────────────────────
        CreateMap<EmpleadoHistorial, EmpleadoHistorialDto>()
            .ForMember(d => d.TipoCambioNombre,
                o => o.MapFrom(s => ObtenerTipoCambioNombre(s.TipoCambio)));

        // ─── Documentos ─────────────────────────────────────────
        CreateMap<EmpleadoDocumento, EmpleadoDocumentoDto>()
            .ForMember(d => d.TipoDocumentoNombre,
                o => o.MapFrom(s => ObtenerTipoDocumentoNombre(s.TipoDocumento)));

        CreateMap<AgregarDocumentoRequest, EmpleadoDocumento>();

        // ─── Asistencia ─────────────────────────────────────────
        CreateMap<Horario, HorarioDto>();
        CreateMap<CrearHorarioRequest, Horario>();

        CreateMap<Asistencia, AsistenciaDto>()
            .ForMember(d => d.EmpleadoNombre,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.NombreCompleto : string.Empty))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoAsistenciaNombre(s.Estado)))
            .ForMember(d => d.TipoAusenciaNombre,
                o => o.MapFrom(s => s.TipoAusencia.HasValue ? ObtenerTipoAusenciaNombre(s.TipoAusencia.Value) : null));

        CreateMap<SolicitudAusencia, SolicitudAusenciaDto>()
            .ForMember(d => d.EmpleadoNombre,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.NombreCompleto : string.Empty))
            .ForMember(d => d.TipoNombre,
                o => o.MapFrom(s => ObtenerTipoAusenciaNombre(s.Tipo)))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoAusenciaNombre(s.Estado)))
            .ForMember(d => d.AprobadorNombre,
                o => o.MapFrom(s => s.Aprobador != null ? s.Aprobador.NombreCompleto : null));

        CreateMap<SaldoVacaciones, SaldoVacacionesDto>()
            .ForMember(d => d.EmpleadoNombre,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.NombreCompleto : string.Empty));

        // ─── Nómina ─────────────────────────────────────────────
        CreateMap<ConceptoNomina, ConceptoNominaDto>()
            .ForMember(d => d.TipoNombre,
                o => o.MapFrom(s => ObtenerTipoConceptoNombre(s.Tipo)))
            .ForMember(d => d.FormaCalculoNombre,
                o => o.MapFrom(s => ObtenerFormaCalculoNombre(s.FormaCalculo)));

        CreateMap<CrearConceptoNominaRequest, ConceptoNomina>();

        CreateMap<PeriodoNomina, PeriodoNominaDto>()
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoPeriodoNombre(s.Estado)));

        CreateMap<RolPago, RolPagoDto>()
            .ForMember(d => d.PeriodoDescripcion,
                o => o.MapFrom(s => s.Periodo != null ? s.Periodo.Descripcion : string.Empty))
            .ForMember(d => d.TipoNombre,
                o => o.MapFrom(s => ObtenerTipoRolNombre(s.Tipo)))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoRolNombre(s.Estado)));

        CreateMap<RolPagoEmpleado, RolPagoEmpleadoDto>()
            .ForMember(d => d.EmpleadoNombre,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.NombreCompleto : string.Empty))
            .ForMember(d => d.Codigo,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.Codigo : string.Empty))
            .ForMember(d => d.EstadoPagoNombre,
                o => o.MapFrom(s => ObtenerEstadoPagoNombre(s.EstadoPago)));

        CreateMap<RolPagoDetalle, RolPagoDetalleDto>()
            .ForMember(d => d.ConceptoCodigo,
                o => o.MapFrom(s => s.Concepto != null ? s.Concepto.Codigo : string.Empty))
            .ForMember(d => d.ConceptoNombre,
                o => o.MapFrom(s => s.Concepto != null ? s.Concepto.Nombre : string.Empty));

        CreateMap<ParametroNomina, ParametroNominaDto>();

        // ─── Préstamos ──────────────────────────────────────────
        CreateMap<Prestamo, PrestamoDto>()
            .ForMember(d => d.EmpleadoNombre,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.NombreCompleto : string.Empty))
            .ForMember(d => d.TipoNombre,
                o => o.MapFrom(s => ObtenerTipoPrestamoNombre(s.Tipo)))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoPrestamoNombre(s.Estado)));

        CreateMap<PrestamoCuota, PrestamoCuotaDto>()
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoCuotaNombre(s.Estado)));

        // ─── Evaluación ─────────────────────────────────────────
        CreateMap<PlantillaEvaluacion, PlantillaEvaluacionDto>();
        CreateMap<PlantillaCriterio, PlantillaCriterioDto>();

        CreateMap<EvaluacionProceso, EvaluacionProcesoDto>()
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoProcesoNombre(s.Estado)))
            .ForMember(d => d.TotalEvaluaciones,
                o => o.MapFrom(s => s.Evaluaciones.Count))
            .ForMember(d => d.EvaluacionesCompletadas,
                o => o.MapFrom(s => s.Evaluaciones.Count(e => e.Estado == 3)));

        CreateMap<Evaluacion, EvaluacionDto>()
            .ForMember(d => d.EvaluadoNombre,
                o => o.MapFrom(s => s.Evaluado != null ? s.Evaluado.NombreCompleto : string.Empty))
            .ForMember(d => d.EvaluadorNombre,
                o => o.MapFrom(s => s.Evaluador != null ? s.Evaluador.NombreCompleto : string.Empty))
            .ForMember(d => d.TipoEvaluadorNombre,
                o => o.MapFrom(s => ObtenerTipoEvaluadorNombre(s.TipoEvaluador)))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoEvaluacionNombre(s.Estado)));

        // ─── Capacitación ───────────────────────────────────────
        CreateMap<Capacitacion, CapacitacionDto>()
            .ForMember(d => d.TipoNombre,
                o => o.MapFrom(s => ObtenerTipoCapacitacionNombre(s.Tipo)))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoCapacitacionNombre(s.Estado)))
            .ForMember(d => d.TotalInscritos,
                o => o.MapFrom(s => s.Participantes.Count));

        CreateMap<Capacitacion, CapacitacionDetalleDto>();

        CreateMap<CapacitacionParticipante, CapacitacionParticipanteDto>()
            .ForMember(d => d.EmpleadoNombre,
                o => o.MapFrom(s => s.Empleado != null ? s.Empleado.NombreCompleto : string.Empty))
            .ForMember(d => d.EstadoNombre,
                o => o.MapFrom(s => ObtenerEstadoParticipanteNombre(s.Estado)));
    }

    // ─── Helpers de nombres ──────────────────────────────────────
    private static int CalcularEdad(DateOnly fechaNac)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var edad = hoy.Year - fechaNac.Year;
        if (fechaNac > hoy.AddYears(-edad)) edad--;
        return edad;
    }

    private static string ObtenerGeneroNombre(short g) => g switch { 1 => "Masculino", 2 => "Femenino", 3 => "Otro", _ => "No especificado" };
    private static string ObtenerEstadoCivilNombre(short e) => e switch { 1 => "Soltero/a", 2 => "Casado/a", 3 => "Divorciado/a", 4 => "Viudo/a", 5 => "Unión Libre", _ => "No especificado" };
    private static string ObtenerEstadoEmpleadoNombre(short e) => e switch { 1 => "Activo", 2 => "Licencia", 3 => "Vacaciones", 4 => "Egresado", _ => "Desconocido" };
    private static string ObtenerTipoContratoNombre(short t) => t switch { 1 => "Indefinido", 2 => "Plazo Fijo", 3 => "Obra/Servicio", 4 => "Pasantía", _ => "Desconocido" };
    private static string ObtenerModalidadNombre(short m) => m switch { 1 => "Presencial", 2 => "Remoto", 3 => "Híbrido", _ => "Desconocido" };
    private static string ObtenerNivelNombre(short n) => n switch { 1 => "Operativo", 2 => "Supervisor", 3 => "Gerencia", 4 => "Dirección", _ => "Desconocido" };
    private static string ObtenerTipoCambioNombre(short t) => t switch { 1 => "Ingreso", 2 => "Cambio de Puesto", 3 => "Cambio de Salario", 4 => "Cambio de Departamento", 5 => "Licencia", 6 => "Egreso", _ => "Desconocido" };
    private static string ObtenerTipoDocumentoNombre(short t) => t switch { 1 => "Contrato", 2 => "Título", 3 => "Certificado", 4 => "Identificación", 5 => "Otro", _ => "Otro" };
    private static string ObtenerEstadoAsistenciaNombre(short e) => e switch { 1 => "Presente", 2 => "Ausente", 3 => "Tardanza", 4 => "Medio Día", 5 => "Feriado", _ => "Desconocido" };
    private static string ObtenerTipoAusenciaNombre(short t) => t switch { 1 => "Vacación", 2 => "Permiso Personal", 3 => "Licencia Médica", 4 => "Maternidad/Paternidad", 5 => "Calamidad", _ => "Otro" };
    private static string ObtenerEstadoAusenciaNombre(short e) => e switch { 1 => "Pendiente", 2 => "Aprobada", 3 => "Rechazada", 4 => "Cancelada", _ => "Desconocido" };
    private static string ObtenerTipoConceptoNombre(short t) => t switch { 1 => "Ingreso", 2 => "Descuento", 3 => "Aporte Patronal", _ => "Desconocido" };
    private static string ObtenerFormaCalculoNombre(short f) => f switch { 1 => "Valor Fijo", 2 => "% del Salario", 3 => "% de otro concepto", 4 => "Fórmula", 5 => "Manual", _ => "Desconocido" };
    private static string ObtenerEstadoPeriodoNombre(short e) => e switch { 1 => "Abierto", 2 => "En Proceso", 3 => "Cerrado", 4 => "Pagado", _ => "Desconocido" };
    private static string ObtenerTipoRolNombre(short t) => t switch { 1 => "Normal", 2 => "Liquidación", 3 => "Décimo 13", 4 => "Décimo 14", 5 => "Utilidades", 6 => "Especial", _ => "Desconocido" };
    private static string ObtenerEstadoRolNombre(short e) => e switch { 1 => "Borrador", 2 => "Calculado", 3 => "Aprobado", 4 => "Pagado", 5 => "Anulado", _ => "Desconocido" };
    private static string ObtenerEstadoPagoNombre(short e) => e switch { 1 => "Pendiente", 2 => "Pagado", 3 => "Rechazado", _ => "Desconocido" };
    private static string ObtenerTipoPrestamoNombre(short t) => t switch { 1 => "Préstamo", 2 => "Anticipo", 3 => "Fondo de Reserva", _ => "Desconocido" };
    private static string ObtenerEstadoPrestamoNombre(short e) => e switch { 1 => "Pendiente", 2 => "Aprobado", 3 => "Rechazado", 4 => "En Pago", 5 => "Cancelado", _ => "Desconocido" };
    private static string ObtenerEstadoCuotaNombre(short e) => e switch { 1 => "Pendiente", 2 => "Pagada", 3 => "Parcial", _ => "Desconocido" };
    private static string ObtenerEstadoProcesoNombre(short e) => e switch { 1 => "Configurando", 2 => "Activo", 3 => "Cerrado", _ => "Desconocido" };
    private static string ObtenerTipoEvaluadorNombre(short t) => t switch { 1 => "Autoevaluación", 2 => "Jefe Directo", 3 => "Par", 4 => "Subordinado", 5 => "Cliente", _ => "Desconocido" };
    private static string ObtenerEstadoEvaluacionNombre(short e) => e switch { 1 => "Pendiente", 2 => "En Proceso", 3 => "Completada", 4 => "Revisada", _ => "Desconocido" };
    private static string ObtenerTipoCapacitacionNombre(short t) => t switch { 1 => "Interna", 2 => "Externa", 3 => "Online", 4 => "Taller", _ => "Otro" };
    private static string ObtenerEstadoCapacitacionNombre(short e) => e switch { 1 => "Planificada", 2 => "En Curso", 3 => "Finalizada", 4 => "Cancelada", _ => "Desconocido" };
    private static string ObtenerEstadoParticipanteNombre(short e) => e switch { 1 => "Inscrito", 2 => "En Curso", 3 => "Aprobado", 4 => "Reprobado", 5 => "Desistió", _ => "Desconocido" };
}
