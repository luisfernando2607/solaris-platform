using AutoMapper;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;

namespace SolarisPlatform.Application.Mappings;

public class ProyectosMappingProfile : Profile
{
    public ProyectosMappingProfile()
    {
        // ─── Proyecto ────────────────────────────────────────────
        // NOTA: GerenteProyecto, Responsable, Sucursal, Moneda son cross-schema
        // Solo existen IDs en la entidad, sin propiedades de navegación.
        // Los campos *Nombre se completan en el Service si se necesitan.

        CreateMap<Proyecto, ProyectoListDto>()
            .ConstructUsing((s, ctx) => new ProyectoListDto(
                s.Id, s.Codigo, s.Nombre,
                s.TipoProyecto, s.Estado, s.Prioridad,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.PorcentajeAvancePlan, s.PorcentajeAvanceReal,
                s.PresupuestoTotal, s.CostoRealTotal,
                null,   // GerenteProyectoNombre — completar en Service
                true))  // Activo — ajustar si BaseEntity lo tiene
            .ForAllMembers(o => o.Ignore());

        CreateMap<Proyecto, ProyectoDto>()
            .ConstructUsing((s, ctx) => new ProyectoDto(
                s.Id, s.EmpresaId, s.Codigo, s.Nombre, s.Descripcion,
                s.TipoProyecto, s.Estado, s.Prioridad,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.FechaInicioReal, s.FechaFinReal,
                s.MonedaId, null,               // MonedaNombre — cross-schema
                s.PresupuestoTotal, s.CostoRealTotal,
                s.PorcentajeAvancePlan, s.PorcentajeAvanceReal,
                s.ClienteId, null,              // ClienteNombre — cross-schema
                s.GerenteProyectoId, null,      // GerenteProyectoNombre — cross-schema
                s.ResponsableId, null,          // ResponsableNombre — cross-schema
                s.SucursalId, null,             // SucursalNombre — cross-schema
                s.Latitud, s.Longitud, s.Direccion,
                true,                           // Activo — BaseEntity no tiene este campo
                DateTime.UtcNow,                // FechaCreacion — Proyecto hereda BaseEntity (sin auditoría)
                new List<ProyectoFaseDto>()))   // Fases — se mapean abajo con AfterMap
            .AfterMap((s, d, ctx) =>
            {
                d.Fases.AddRange(ctx.Mapper.Map<List<ProyectoFaseDto>>(s.Fases));
            })
            .ForAllMembers(o => o.Ignore());

        // ─── DASHBOARD ───────────────────────────────────────────
        // FIX PRINCIPAL: Mapeo faltante Proyecto → ProyectoDashboardDto
        CreateMap<Proyecto, ProyectoDashboardDto>()
            .ConstructUsing((s, ctx) => new ProyectoDashboardDto(
                s.Id,
                s.Codigo,
                s.Nombre,
                s.Estado,
                s.PorcentajeAvancePlan,
                s.PorcentajeAvanceReal,
                s.PresupuestoTotal,
                s.CostoRealTotal,
                null,                           // DiasRestantes — calculado en Service
                false,                          // EstaRetrasado — calculado en Service
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
                true))  // Activo — ajustar si existe en BaseEntity
            .ForAllMembers(o => o.Ignore());

        // ─── Hito ────────────────────────────────────────────────
        CreateMap<ProyectoHito, ProyectoHitoDto>()
            .ConstructUsing((s, ctx) => new ProyectoHitoDto(
                s.Id, s.ProyectoId, s.Nombre, s.Descripcion,
                s.FechaCompromiso, s.FechaReal,
                s.Estado, s.PorcentajePeso,
                s.ResponsableId, null,          // ResponsableNombre — cross-schema
                s.Orden, true))                 // Activo
            .ForAllMembers(o => o.Ignore());

        CreateMap<ProyectoHito, ProyectoHitoListDto>()
            .ConstructUsing((s, ctx) => new ProyectoHitoListDto(
                s.Id, s.Nombre, s.FechaCompromiso, s.Estado,
                s.PorcentajePeso,
                null,   // ResponsableNombre — cross-schema
                s.Orden))
            .ForAllMembers(o => o.Ignore());

        // ─── Documento ───────────────────────────────────────────
        CreateMap<ProyectoDocumento, ProyectoDocumentoDto>()
            .ConstructUsing((s, ctx) => new ProyectoDocumentoDto(
                s.Id, s.ProyectoId, s.TipoDocumento,
                s.Nombre, s.Descripcion, s.UrlStorage,
                s.NombreArchivoOriginal, s.Extension, s.TamanoBytes,
                null,                           // SubidoPorNombre — cross-schema
                s.FechaSubida, true))
            .ForAllMembers(o => o.Ignore());

        // ─── WBS ─────────────────────────────────────────────────
        CreateMap<WbsNodo, WbsNodoDto>()
            .ForMember(d => d.Hijos,  o => o.MapFrom(s => s.Hijos))
            .ForMember(d => d.Tareas, o => o.MapFrom(s => s.Tareas));

        // ─── Tarea ───────────────────────────────────────────────
        CreateMap<Tarea, TareaListDto>()
            .ConstructUsing((s, ctx) => new TareaListDto(
                s.Id, s.ProyectoId, s.WbsNodoId, s.FaseId,
                s.Nombre, s.Estado, s.Prioridad,
                s.FechaInicioPlan, s.FechaFinPlan,
                s.PorcentajeAvance,
                s.ResponsableId,
                null,           // ResponsableNombre — cross-schema
                s.CuadrillaId,
                null))          // CuadrillaNombre — completar en Service
            .ForAllMembers(o => o.Ignore());

        CreateMap<Tarea, TareaDto>()
            .ForMember(d => d.ResponsableNombre,   o => o.Ignore())
            .ForMember(d => d.DependenciasOrigen,  o => o.MapFrom(s => s.DependenciasOrigen))
            .ForMember(d => d.DependenciasDestino, o => o.MapFrom(s => s.DependenciasDestino));

        CreateMap<TareaDependencia, TareaDependenciaDto>()
            .ConstructUsing((s, ctx) => new TareaDependenciaDto(
                s.Id,
                s.TareaOrigenId,
                null!,              // TareaOrigenNombre — completar en Service
                s.TareaDestinoId,
                null!,              // TareaDestinoNombre — completar en Service
                s.TipoDependencia,
                s.Desfase))
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
        CreateMap<OrdenTrabajo, OrdenTrabajoListDto>()
            .ConstructUsing((s, ctx) => new OrdenTrabajoListDto(
                s.Id, s.Numero, s.Titulo, s.Estado,
                s.ProyectoId,
                null,   // ProyectoNombre
                s.CuadrillaId,
                null,   // CuadrillaNombre
                s.TecnicoResponsableId,
                null,   // TecnicoNombre — cross-schema
                s.FechaInicioPlan, s.FechaFinPlan,
                s.FechaInicioReal, s.FechaFinReal))
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