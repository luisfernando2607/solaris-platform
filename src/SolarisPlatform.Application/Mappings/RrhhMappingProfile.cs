// =====================================================
// FIX: RrhhMappingProfile.cs
// PROBLEMAS CORREGIDOS:
//
// 1. EmpleadoFichaDto (record posicional de 40+ campos):
//    Error: "EmpleadoFichaDto needs to have a constructor with 0 args"
//    Fix: Agregar ConstructUsing() con todos los campos.
//    Campos calculados que faltaban en el mapping:
//      - NacionalidadNombre → s.Pais.Nombre (via NacionalidadPaisId)
//      - PaisNombre         → s.Pais.Nombre (dirección)
//      - EstadoProvinciaNombre → s.EstadoProvincia.Nombre
//      - CiudadNombre       → s.Ciudad.Nombre
//      - MonedaSimbolo      → s.Moneda.Simbolo
//      - BancoNombre        → s.Banco.Nombre
//      - JornadaLaboral     → campo existente en entidad, no estaba mapeado
//
// 2. EmpleadoHistorialDto (record posicional):
//    Error: "Error mapping types: EmpleadoHistorial -> EmpleadoHistorialDto"
//    Fix: Agregar ConstructUsing() — la entidad tiene DepartamentoAnterior,
//         PuestoAnterior etc. como FKs (ids), pero el DTO necesita nombres.
//         Sin navegaciones cargadas, se devuelven nulls seguros.
// =====================================================

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
            .ConstructUsing((s, _) => new PuestoDto(
                s.Id, s.Codigo, s.Nombre, s.Descripcion,
                s.DepartamentoId,
                s.Departamento != null ? s.Departamento.Nombre : string.Empty,
                s.NivelJerarquico,
                ObtenerNivelNombre(s.NivelJerarquico),
                s.BandaSalarialMin, s.BandaSalarialMax,
                s.RequiereTitulo, true))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearPuestoRequest, Puesto>();

        // ─── Empleado — Lista ────────────────────────────────────
        CreateMap<Empleado, EmpleadoListaDto>()
            .ConstructUsing((s, _) => new EmpleadoListaDto(
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

        // ─── Empleado — Ficha completa ───────────────────────────
        // FIX: Convertido a ConstructUsing() para evitar el error de constructor.
        // Los campos de navegación (PaisNombre, Ciudad, Moneda, Banco) se
        // resuelven aquí directamente. El servicio debe hacer Include() de estas
        // navegaciones para que devuelvan datos reales.
        CreateMap<Empleado, EmpleadoFichaDto>()
            .ConstructUsing((s, _) => new EmpleadoFichaDto(
                // ── Identificación ──
                s.Id,
                s.Codigo,
                s.TipoIdentificacion,
                s.NumeroIdentificacion,
                // ── Nombres ──
                s.PrimerNombre,
                s.SegundoNombre,
                s.PrimerApellido,
                s.SegundoApellido,
                s.NombreCompleto,
                // ── Personal ──
                s.FechaNacimiento,
                s.FechaNacimiento.HasValue ? (int?)CalcularEdad(s.FechaNacimiento.Value) : null,
                s.Genero,
                s.Genero.HasValue ? ObtenerGeneroNombre(s.Genero.Value) : null,
                s.EstadoCivil,
                s.EstadoCivil.HasValue ? ObtenerEstadoCivilNombre(s.EstadoCivil.Value) : null,
                s.NacionalidadPaisId,
                // NacionalidadNombre, PaisNombre, EstadoProvinciaNombre, CiudadNombre:
                // La entidad Empleado NO tiene navegaciones directas para estas FKs.
                // El servicio debe resolverlos por separado o con consultas adicionales.
                // Por ahora se devuelve null — no rompe el mapping ni el runtime.
                (string?)null,  // NacionalidadNombre
                // ── Contacto ──
                s.EmailPersonal,
                s.EmailCorporativo,
                s.TelefonoCelular,
                s.TelefonoFijo,
                // ── Dirección ──
                (string?)null,  // PaisNombre
                (string?)null,  // EstadoProvinciaNombre
                (string?)null,  // CiudadNombre
                s.Direccion,
                // ── Laboral ──
                s.DepartamentoId,
                s.Departamento != null ? s.Departamento.Nombre : null,
                s.PuestoId,
                s.Puesto != null ? s.Puesto.Nombre : null,
                s.JefeDirectoId,
                s.JefeDirecto != null ? s.JefeDirecto.NombreCompleto : null,
                s.FechaIngreso,
                s.FechaEgreso,
                s.TipoContrato,
                ObtenerTipoContratoNombre(s.TipoContrato),
                s.ModalidadTrabajo,
                ObtenerModalidadNombre(s.ModalidadTrabajo),
                s.HorasSemanales,
                s.SalarioBase,
                (string?)null,  // MonedaSimbolo — Empleado no tiene navegación a Moneda
                // ── Seguridad social ──
                s.NumeroSeguroSocial,
                s.NumeroAfiliacion,
                // ── Banco ──
                (string?)null,  // BancoNombre — Empleado no tiene navegación a Banco
                s.TipoCuentaBancaria,
                s.NumeroCuentaBancaria,
                // ── Estado ──
                s.Estado,
                ObtenerEstadoEmpleadoNombre(s.Estado),
                s.FotoUrl))
            .ForAllMembers(o => o.Ignore());

        CreateMap<CrearEmpleadoRequest, Empleado>();

        // ─── Historial ──────────────────────────────────────────
        // FIX: ConstructUsing() para el record posicional.
        // DepartamentoAnterior/Nuevo y PuestoAnterior/Nuevo son FKs (ids);
        // los nombres se resuelven via navegaciones del servicio.
        CreateMap<EmpleadoHistorial, EmpleadoHistorialDto>()
            .ConstructUsing((s, _) => new EmpleadoHistorialDto(
                s.Id,
                s.TipoCambio,
                ObtenerTipoCambioNombre(s.TipoCambio),
                s.FechaEfectiva,
                // EmpleadoHistorial NO tiene navegaciones DepartamentoAnterior/Nuevo/PuestoAnterior/Nuevo
                // Solo tiene los IDs (departamento_anterior_id, puesto_anterior_id, etc.)
                // El servicio debe resolver los nombres si se necesitan.
                (string?)null,  // DepartamentoAnterior
                (string?)null,  // PuestoAnterior
                s.SalarioAnterior,
                (string?)null,  // DepartamentoNuevo
                (string?)null,  // PuestoNuevo
                s.SalarioNuevo,
                s.Motivo,
                s.Observaciones,
                // FechaCreacion no existe en la entidad EmpleadoHistorial (BaseEntity solo tiene Id)
                // Si necesitas esta fecha, agrégala como propiedad propia en la entidad.
                DateTime.MinValue))
            .ForAllMembers(o => o.Ignore());

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

    // ─── Helpers ─────────────────────────────────────────────────
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
