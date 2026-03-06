// ══════════════════════════════════════════════════════════════════
// INFRASTRUCTURE/DependencyInjection.cs
// Agregar dentro del método AddInfrastructure() existente:
// ══════════════════════════════════════════════════════════════════

/*
// ── DbSets — SolarisDbContext.cs (OnModelCreating) ───────────────
modelBuilder.ApplyConfiguration(new ProyectoConfiguration());
modelBuilder.ApplyConfiguration(new ProyectoHitoConfiguration());
modelBuilder.ApplyConfiguration(new ProyectoFaseConfiguration());
modelBuilder.ApplyConfiguration(new ProyectoDocumentoConfiguration());
modelBuilder.ApplyConfiguration(new WbsNodoConfiguration());
modelBuilder.ApplyConfiguration(new TareaConfiguration());
modelBuilder.ApplyConfiguration(new TareaDependenciaConfiguration());
modelBuilder.ApplyConfiguration(new CuadrillaConfiguration());
modelBuilder.ApplyConfiguration(new CuadrillaMiembroConfiguration());
modelBuilder.ApplyConfiguration(new RecursoProyectoConfiguration());
modelBuilder.ApplyConfiguration(new PresupuestoConfiguration());
modelBuilder.ApplyConfiguration(new PresupuestoPartidaConfiguration());
modelBuilder.ApplyConfiguration(new CostoRealConfiguration());
modelBuilder.ApplyConfiguration(new GanttLineaBaseConfiguration());
modelBuilder.ApplyConfiguration(new GanttProgresoConfiguration());
modelBuilder.ApplyConfiguration(new CentroCostoConfiguration());
modelBuilder.ApplyConfiguration(new AsignacionCentroCostoConfiguration());
modelBuilder.ApplyConfiguration(new OrdenTrabajoConfiguration());
modelBuilder.ApplyConfiguration(new OtActividadConfiguration());
modelBuilder.ApplyConfiguration(new OtMaterialConfiguration());
modelBuilder.ApplyConfiguration(new ReporteAvanceConfiguration());
modelBuilder.ApplyConfiguration(new ReporteAvanceFotoConfiguration());
modelBuilder.ApplyConfiguration(new KpiProyectoConfiguration());
modelBuilder.ApplyConfiguration(new AlertaProyectoConfiguration());

// ── DbSets — SolarisDbContext.cs (propiedades) ───────────────────
public DbSet<Proyecto>              Proyectos             => Set<Proyecto>();
public DbSet<ProyectoHito>          ProyectosHitos        => Set<ProyectoHito>();
public DbSet<ProyectoFase>          ProyectosFases        => Set<ProyectoFase>();
public DbSet<ProyectoDocumento>     ProyectosDocumentos   => Set<ProyectoDocumento>();
public DbSet<WbsNodo>               WbsNodos              => Set<WbsNodo>();
public DbSet<Tarea>                 Tareas                => Set<Tarea>();
public DbSet<TareaDependencia>      TareasDependencias    => Set<TareaDependencia>();
public DbSet<Cuadrilla>             Cuadrillas            => Set<Cuadrilla>();
public DbSet<CuadrillaMiembro>      CuadrillasMiembros    => Set<CuadrillaMiembro>();
public DbSet<RecursoProyecto>       RecursosProyecto      => Set<RecursoProyecto>();
public DbSet<Presupuesto>           Presupuestos          => Set<Presupuesto>();
public DbSet<PresupuestoPartida>    PresupuestosPartidas  => Set<PresupuestoPartida>();
public DbSet<CostoReal>             CostosReales          => Set<CostoReal>();
public DbSet<GanttLineaBase>        GanttLineasBase       => Set<GanttLineaBase>();
public DbSet<GanttProgreso>         GanttProgresos        => Set<GanttProgreso>();
public DbSet<CentroCosto>           CentrosCosto          => Set<CentroCosto>();
public DbSet<AsignacionCentroCosto> AsignacionesCentro    => Set<AsignacionCentroCosto>();
public DbSet<OrdenTrabajo>          OrdenesTrabajo        => Set<OrdenTrabajo>();
public DbSet<OtActividad>           OtActividades         => Set<OtActividad>();
public DbSet<OtMaterial>            OtMateriales          => Set<OtMaterial>();
public DbSet<ReporteAvance>         ReportesAvance        => Set<ReporteAvance>();
public DbSet<ReporteAvanceFoto>     ReportesAvanceFotos   => Set<ReporteAvanceFoto>();
public DbSet<KpiProyecto>           KpisProyecto          => Set<KpiProyecto>();
public DbSet<AlertaProyecto>        AlertasProyecto       => Set<AlertaProyecto>();

// ── Repositorios ─────────────────────────────────────────────────
services.AddScoped<IProyectoRepository, ProyectoRepository>();
services.AddScoped<IProyectoHitoRepository, ProyectoHitoRepository>();
services.AddScoped<IProyectoFaseRepository, ProyectoFaseRepository>();
services.AddScoped<IProyectoDocumentoRepository, ProyectoDocumentoRepository>();
services.AddScoped<IWbsNodoRepository, WbsNodoRepository>();
services.AddScoped<ITareaRepository, TareaRepository>();
services.AddScoped<ITareaDependenciaRepository, TareaDependenciaRepository>();
services.AddScoped<ICuadrillaRepository, CuadrillaRepository>();
services.AddScoped<ICuadrillaMiembroRepository, CuadrillaMiembroRepository>();
services.AddScoped<IRecursoProyectoRepository, RecursoProyectoRepository>();
services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
services.AddScoped<IPresupuestoPartidaRepository, PresupuestoPartidaRepository>();
services.AddScoped<ICostoRealRepository, CostoRealRepository>();
services.AddScoped<IGanttLineaBaseRepository, GanttLineaBaseRepository>();
services.AddScoped<IGanttProgresoRepository, GanttProgresoRepository>();
services.AddScoped<ICentroCostoRepository, CentroCostoRepository>();
services.AddScoped<IAsignacionCentroCostoRepository, AsignacionCentroCostoRepository>();
services.AddScoped<IOrdenTrabajoRepository, OrdenTrabajoRepository>();
services.AddScoped<IOtActividadRepository, OtActividadRepository>();
services.AddScoped<IOtMaterialRepository, OtMaterialRepository>();
services.AddScoped<IReporteAvanceRepository, ReporteAvanceRepository>();
services.AddScoped<IKpiProyectoRepository, KpiProyectoRepository>();
services.AddScoped<IAlertaProyectoRepository, AlertaProyectoRepository>();

// ── Servicios ─────────────────────────────────────────────────────
services.AddScoped<IProyectoService, ProyectoService>();
services.AddScoped<IProyectoFaseService, ProyectoFaseService>();
services.AddScoped<IProyectoHitoService, ProyectoHitoService>();
services.AddScoped<IProyectoDocumentoService, ProyectoDocumentoService>();
services.AddScoped<IWbsService, WbsService>();
services.AddScoped<ITareaService, TareaService>();
services.AddScoped<ICuadrillaService, CuadrillaService>();
services.AddScoped<IPresupuestoService, PresupuestoService>();
services.AddScoped<IGanttService, GanttService>();
services.AddScoped<ICentroCostoService, CentroCostoService>();
services.AddScoped<IOrdenTrabajoService, OrdenTrabajoService>();
services.AddScoped<IReporteAvanceService, ReporteAvanceService>();
services.AddScoped<IKpiService, KpiService>();
*/
