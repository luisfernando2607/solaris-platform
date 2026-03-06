using AutoMapper;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;

namespace SolarisPlatform.Application.Mappings;

public class ProyectosMappingProfile : Profile
{
    public ProyectosMappingProfile()
    {
        // ─── Proyecto ────────────────────────────────────────────
        CreateMap<Proyecto, ProyectoListDto>()
            .ConstructUsing((s, ctx) => new ProyectoListDto(
                s.Id, s.Codigo, s.Nombre,
                s.TipoProyecto, s.Estado, s.Prioridad,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.PorcentajeAvancePlan, s.PorcentajeAvanceReal,
                s.PresupuestoTotal, s.CostoRealTotal,
                null,   // GerenteProyectoNombre — cross-schema
                true))  // Activo
            .ForAllMembers(o => o.Ignore());

        CreateMap<Proyecto, ProyectoDto>()
            .ConstructUsing((s, ctx) => new ProyectoDto(
                s.Id, s.EmpresaId, s.Codigo, s.Nombre, s.Descripcion,
                s.TipoProyecto, s.Estado, s.Prioridad,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.FechaInicioReal, s.FechaFinReal,
                s.MonedaId, null,
                s.PresupuestoTotal, s.CostoRealTotal,
                s.PorcentajeAvancePlan, s.PorcentajeAvanceReal,
                s.ClienteId, null,
                s.GerenteProyectoId, null,
                s.ResponsableId, null,
                s.SucursalId, null,
                s.Latitud, s.Longitud, s.Direccion,
                true,
                DateTime.UtcNow,
                new List<ProyectoFaseDto>()))
            .AfterMap((s, d, ctx) =>
            {
                d.Fases.AddRange(ctx.Mapper.Map<List<ProyectoFaseDto>>(s.Fases));
            })
            .ForAllMembers(o => o.Ignore());

        // ─── DASHBOARD ───────────────────────────────────────────
        CreateMap<Proyecto, ProyectoDashboardDto>()
            .ConstructUsing((s, ctx) => new ProyectoDashboardDto(
                s.Id, s.Codigo, s.Nombre, s.Estado,
                s.PorcentajeAvancePlan, s.PorcentajeAvanceReal,
                s.PresupuestoTotal, s.CostoRealTotal,
                null, false,
                new List<ProyectoHitoListDto>(),
                new List<AlertaProyectoDto>(),
                new List<KpiProyectoDto>()))
            .ForAllMembers(o => o.Ignore());

        // ─── Fase ────────────────────────────────────────────────
        CreateMap<ProyectoFase, ProyectoFaseDto>()
            .ConstructUsing((s, ctx) => new ProyectoFaseDto(
                s.Id, s.ProyectoId, s.Codigo, s.Nombre, s.Descripcion,
                s.Orden,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.FechaInicioReal, s.FechaFinReal,
                s.PorcentajeAvance, s.Estado,
                true))
            .ForAllMembers(o => o.Ignore());

        // ─── Hito ────────────────────────────────────────────────
        CreateMap<ProyectoHito, ProyectoHitoDto>()
            .ConstructUsing((s, ctx) => new ProyectoHitoDto(
                s.Id, s.ProyectoId, s.Nombre, s.Descripcion,
                s.FechaCompromiso, s.FechaReal,
                s.Estado, s.PorcentajePeso,
                s.ResponsableId, null,
                s.Orden, true))
            .ForAllMembers(o => o.Ignore());

        CreateMap<ProyectoHito, ProyectoHitoListDto>()
            .ConstructUsing((s, ctx) => new ProyectoHitoListDto(
                s.Id, s.Nombre, s.FechaCompromiso, s.Estado,
                s.PorcentajePeso, null, s.Orden))
            .ForAllMembers(o => o.Ignore());

        // ─── Documento ───────────────────────────────────────────
        CreateMap<ProyectoDocumento, ProyectoDocumentoDto>()
            .ConstructUsing((s, ctx) => new ProyectoDocumentoDto(
                s.Id, s.ProyectoId, s.TipoDocumento,
                s.Nombre, s.Descripcion, s.UrlStorage,
                s.NombreArchivoOriginal, s.Extension, s.TamanoBytes,
                null, s.FechaSubida, true))
            .ForAllMembers(o => o.Ignore());

        // ─── WBS ─────────────────────────────────────────────────
        CreateMap<WbsNodo, WbsNodoDto>()
            .ForMember(d => d.Hijos,  o => o.MapFrom(s => s.Hijos))
            .ForMember(d => d.Tareas, o => o.MapFrom(s => s.Tareas));

        // ─── Tarea ───────────────────────────────────────────────
        // FIX: Eliminado FaseId — Tarea ya no tiene esa propiedad (no existe en BD)
        CreateMap<Tarea, TareaListDto>()
            .ConstructUsing((s, ctx) => new TareaListDto(
                s.Id, s.ProyectoId, s.WbsNodoId,
                null,           // FIX: FaseId eliminado de entidad — pasa null
                s.Nombre, s.Estado, s.Prioridad,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.PorcentajeAvance,
                s.ResponsableId, null,
                s.CuadrillaId, null))
            .ForAllMembers(o => o.Ignore());

        CreateMap<Tarea, TareaDto>()
            .ForMember(d => d.ResponsableNombre,   o => o.Ignore())
            .ForMember(d => d.DependenciasOrigen,  o => o.MapFrom(s => s.DependenciasOrigen))
            .ForMember(d => d.DependenciasDestino, o => o.MapFrom(s => s.DependenciasDestino));

        CreateMap<TareaDependencia, TareaDependenciaDto>()
            .ConstructUsing((s, ctx) => new TareaDependenciaDto(
                s.Id,
                s.TareaOrigenId, null!,
                s.TareaDestinoId, null!,
                s.TipoDependencia, s.Desfase))
            .ForAllMembers(o => o.Ignore());

        // ─── Cuadrilla ───────────────────────────────────────────
        CreateMap<Cuadrilla, CuadrillaDto>()
            .ForMember(d => d.LiderNombre, o => o.Ignore())
            .ForMember(d => d.Miembros,    o => o.MapFrom(s => s.Miembros));

        CreateMap<CuadrillaMiembro, CuadrillaMiembroDto>()
            .ForMember(d => d.EmpleadoNombre, o => o.Ignore());

        // ─── Presupuesto ─────────────────────────────────────────
        CreateMap<Presupuesto, PresupuestoDto>()
            .ForMember(d => d.AprobadoPorNombre, o => o.Ignore())
            .ForMember(d => d.Partidas,          o => o.MapFrom(s => s.Partidas));

        CreateMap<PresupuestoPartida, PresupuestoPartidaDto>()
            .ForMember(d => d.CostosReales, o => o.MapFrom(s => s.CostosReales));

        CreateMap<CostoReal, CostoRealDto>()
            .ForMember(d => d.RegistradoPorNombre, o => o.Ignore());

        // ─── Centro de Costo ─────────────────────────────────────
        CreateMap<CentroCosto, CentroCostoDto>();

        // ─── Orden de Trabajo ────────────────────────────────────
        // FIX: Numero→Codigo, Titulo eliminado, TecnicoResponsableId→TecnicoAsignadoId
        //      Fechas: FechaInicioPlan/FinPlan/InicioReal/FinReal → FechaProgramada/Ejecucion
        CreateMap<OrdenTrabajo, OrdenTrabajoListDto>()
            .ConstructUsing((s, ctx) => new OrdenTrabajoListDto(
                s.Id,
                s.Codigo,           // FIX: era s.Numero
                s.Descripcion,      // FIX: era s.Titulo (ahora Descripcion)
                s.Estado,
                s.ProyectoId, null,
                s.CuadrillaId, null,
                s.TecnicoAsignadoId,    // FIX: era s.TecnicoResponsableId
                null,
                s.FechaProgramada,      // FIX: era FechaInicioPlan
                null,                   // FIX: FechaFinPlan no existe en BD — null
                s.FechaInicioEjecucion, // FIX: era FechaInicioReal
                s.FechaFinEjecucion))   // FIX: era FechaFinReal
            .ForAllMembers(o => o.Ignore());

        CreateMap<OrdenTrabajo, OrdenTrabajoDto>()
            .ForMember(d => d.ProyectoNombre,    o => o.Ignore())
            .ForMember(d => d.TareaNombre,       o => o.Ignore())
            .ForMember(d => d.CuadrillaNombre,   o => o.Ignore())
            .ForMember(d => d.AsignadoPorNombre, o => o.Ignore())
            .ForMember(d => d.TecnicoNombre,     o => o.Ignore())
            .ForMember(d => d.FirmadoPorNombre,  o => o.Ignore())
            .ForMember(d => d.Actividades,       o => o.MapFrom(s => s.Actividades))
            .ForMember(d => d.Materiales,        o => o.MapFrom(s => s.Materiales));

        CreateMap<OtActividad, OtActividadDto>()
            .ForMember(d => d.CompletadaPorNombre, o => o.Ignore());

        CreateMap<OtMaterial, OtMaterialDto>();

        // ─── Reporte de Avance ───────────────────────────────────
        CreateMap<ReporteAvance, ReporteAvanceDto>()
            .ForMember(d => d.ReportadoPorNombre, o => o.Ignore())
            .ForMember(d => d.Fotos,              o => o.MapFrom(s => s.Fotos));

        CreateMap<ReporteAvanceFoto, ReporteAvanceFotoDto>();

        // ─── KPI ─────────────────────────────────────────────────
        CreateMap<KpiProyecto, KpiProyectoDto>()
            .ForMember(d => d.Semaforo,
                o => o.MapFrom(s => CalcularSemaforo(s)));

        // ─── Alerta ──────────────────────────────────────────────
        CreateMap<AlertaProyecto, AlertaProyectoDto>();

        // ─── Gantt ───────────────────────────────────────────────
        CreateMap<ProyectoFase, GanttFaseDto>()
            .ConstructUsing((s, ctx) => new GanttFaseDto(
                s.Id, s.Nombre,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.PorcentajeAvance,
                new List<GanttTareaDto>()))
            .ForAllMembers(o => o.Ignore());

        // FIX: DependenciasOrigen puede ser null en algunos contextos
        CreateMap<Tarea, GanttTareaDto>()
            .ConstructUsing((s, ctx) => new GanttTareaDto(
                s.Id, s.Nombre,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.FechaInicioReal, s.FechaFinReal,
                null, null,
                s.PorcentajeAvance, s.Estado,
                s.DependenciasOrigen != null
                    ? s.DependenciasOrigen.Select(d => d.TareaDestinoId).ToList()
                    : new List<long>()))
            .ForAllMembers(o => o.Ignore());
    }

    private static string CalcularSemaforo(KpiProyecto k)
    {
        if (k.ValorMinimo.HasValue && k.Valor < k.ValorMinimo.Value) return "rojo";
        if (k.ValorMeta.HasValue   && k.Valor >= k.ValorMeta.Value)  return "verde";
        return "amarillo";
    }
}
