using AutoMapper;
using SolarisPlatform.Application.DTOs.Proyectos;
using SolarisPlatform.Domain.Entities.Proyectos;

namespace SolarisPlatform.Application.Mappings;

public class ProyectoMappingProfile : Profile
{
    public ProyectoMappingProfile()
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
        // FIX F2: WbsNodoDto es positional record — requiere ConstructUsing
        // CodigoWbs → Codigo, PesoRelativo → PorcentajePeso
        CreateMap<WbsNodo, WbsNodoDto>()
            .ConstructUsing((s, ctx) => new WbsNodoDto(
                s.Id, s.ProyectoId, s.FaseId, s.PadreId,
                s.CodigoWbs, s.Nombre, s.Descripcion,
                s.TipoNodo, s.Nivel, s.Orden,
                s.PorcentajeAvance, s.PesoRelativo, s.EsHoja, true,
                new List<WbsNodoDto>(), new List<TareaListDto>()))
            .AfterMap((s, d, ctx) =>
            {
                if (s.Hijos  != null) d.Hijos.AddRange(ctx.Mapper.Map<List<WbsNodoDto>>(s.Hijos));
                if (s.Tareas != null) d.Tareas.AddRange(ctx.Mapper.Map<List<TareaListDto>>(s.Tareas));
            })
            .ForAllMembers(o => o.Ignore());

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
            .ConstructUsing((s, ctx) => new CuadrillaDto(
                s.Id, s.ProyectoId, s.Nombre, s.Descripcion,
                s.LiderId, null, s.CapacidadMaxima, true,
                new List<CuadrillaMiembroDto>()))
            .AfterMap((s, d, ctx) =>
            {
                if (s.Miembros != null)
                    d.Miembros.AddRange(ctx.Mapper.Map<List<CuadrillaMiembroDto>>(s.Miembros));
            })
            .ForAllMembers(o => o.Ignore());

        CreateMap<CuadrillaMiembro, CuadrillaMiembroDto>()
            .ConstructUsing((s, ctx) => new CuadrillaMiembroDto(
                s.Id, s.EmpleadoId, string.Empty,
                s.FechaIngreso, s.FechaSalida,
                s.Rol.HasValue ? s.Rol.Value.ToString() : null, true))
            .ForAllMembers(o => o.Ignore());

        // ─── Presupuesto ─────────────────────────────────────────
        // FIX F5: PresupuestoDto es positional record con campos que difieren de la entidad
        // Entidad: Version, Descripcion, Estado, TotalGeneral, MontoTotal*
        // DTO:     Nombre, TotalIngresos, TotalEgresos, Contingencia, EsActivo, EsAprobado
        CreateMap<Presupuesto, PresupuestoDto>()
            .ConstructUsing((s, ctx) => new PresupuestoDto(
                s.Id, s.ProyectoId,
                s.Version,
                s.Descripcion ?? $"Presupuesto v{s.Version}",  // Nombre ← Descripcion
                s.Descripcion,
                s.MontoTotalManoObra + s.MontoTotalMateriales,  // TotalIngresos (aproximado)
                s.MontoTotalSubcontratos + s.MontoTotalEquipos + s.MontoTotalIndirectos, // TotalEgresos
                0m,                           // Contingencia (no existe en BD)
                s.TotalGeneral,               // TotalNeto
                s.Estado == 1,                // EsActivo (Estado=1 es Borrador/activo)
                s.Estado == 2,                // EsAprobado (Estado=2)
                s.AprobadoPorId, null,
                s.FechaAprobacion.HasValue
                    ? s.FechaAprobacion.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null,
                DateTime.UtcNow,              // FechaCreacion (no en entidad, usar UtcNow)
                new List<PresupuestoPartidaDto>()))
            .AfterMap((s, d, ctx) =>
            {
                if (s.Partidas != null)
                    d.Partidas.AddRange(ctx.Mapper.Map<List<PresupuestoPartidaDto>>(s.Partidas));
            })
            .ForAllMembers(o => o.Ignore());

        CreateMap<PresupuestoPartida, PresupuestoPartidaDto>()
            .ForMember(d => d.CostosReales, o => o.MapFrom(s => s.CostosReales));

        CreateMap<CostoReal, CostoRealDto>()
            .ForMember(d => d.RegistradoPorNombre, o => o.Ignore());

        // ─── Centro de Costo ─────────────────────────────────────
        CreateMap<CentroCosto, CentroCostoDto>()
            .ConstructUsing((s, ctx) => new CentroCostoDto(
                s.Id, s.ProyectoId, s.Codigo, s.Nombre, s.Descripcion,
                s.PresupuestoAnual, 0m, true))
            .ForAllMembers(o => o.Ignore());

        // ─── Orden de Trabajo ────────────────────────────────────
        // FIX: Alineado con entidad real en BD y con OrdenTrabajoListDto actualizado:
        //   s.Codigo           → param Codigo        (era s.Numero)
        //   s.Descripcion      → param Titulo        (era s.Titulo; ahora nullable → sin CS8604)
        //   s.TecnicoAsignadoId→ param TecnicoAsignadoId (era s.TecnicoResponsableId)
        //   s.FechaProgramada  → param FechaProgramada    (era FechaInicioPlan)
        //   null               → param FechaFinPlan       (no existe en BD)
        //   s.FechaInicioEjecucion → param FechaInicioEjecucion (era FechaInicioReal)
        //   s.FechaFinEjecucion    → param FechaFinEjecucion    (era FechaFinReal)
        CreateMap<OrdenTrabajo, OrdenTrabajoListDto>()
            .ConstructUsing((s, ctx) => new OrdenTrabajoListDto(
                s.Id,
                s.Codigo,
                s.Descripcion,          // FIX: nullable — elimina CS8604
                s.Estado,
                s.ProyectoId, null,
                s.CuadrillaId, null,
                s.TecnicoAsignadoId,    // FIX: era s.TecnicoResponsableId
                null,
                s.FechaProgramada,      // FIX: era FechaInicioPlan
                null,                   // FIX: FechaFinPlan no existe en BD
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
        // FIX: ReporteAvanceDto positional record — entidad tiene AvanceGeneral/AvanceCosto
        // DTO pide: OrdenTrabajoId, PorcentajeAvancePlan, PorcentajeAvanceReal, Bac/Ev/Pv/Ac/Cpi/Spi
        CreateMap<ReporteAvance, ReporteAvanceDto>()
            .ConstructUsing((s, ctx) => new ReporteAvanceDto(
                s.Id, s.ProyectoId,
                null,                  // OrdenTrabajoId — no existe en entidad
                s.FechaReporte, s.Titulo,
                s.Observaciones,       // Descripcion ← Observaciones
                s.AvanceGeneral,       // PorcentajeAvancePlan ← AvanceGeneral
                s.AvanceCosto,         // PorcentajeAvanceReal ← AvanceCosto
                null, null, null, null, null, null,  // Bac/Ev/Pv/Ac/Cpi/Spi — no en BD
                null,                  // ReportadoPorNombre
                s.Observaciones,
                null, null,            // Latitud/Longitud — no en entidad
                new List<ReporteAvanceFotoDto>()))
            .AfterMap((s, d, ctx) =>
            {
                if (s.Fotos != null)
                    d.Fotos.AddRange(ctx.Mapper.Map<List<ReporteAvanceFotoDto>>(s.Fotos));
            })
            .ForAllMembers(o => o.Ignore());

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