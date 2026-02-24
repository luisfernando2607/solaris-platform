// =====================================================
// INFRASTRUCTURE LAYER - Adiciones al DbContext
// Archivo: Infrastructure/Persistence/Context/SolarisDbContext.cs
// AGREGAR estas propiedades al DbContext existente
// =====================================================

// NOTA: Agrega estas líneas al SolarisDbContext.cs existente

/*
  // ── Catálogos ──────────────────────────────────────
  public DbSet<Pais> Paises => Set<Pais>();
  public DbSet<EstadoProvincia> EstadosProvincias => Set<EstadoProvincia>();
  public DbSet<Ciudad> Ciudades => Set<Ciudad>();
  public DbSet<Moneda> Monedas => Set<Moneda>();
  public DbSet<TipoIdentificacion> TiposIdentificacion => Set<TipoIdentificacion>();
  public DbSet<Impuesto> Impuestos => Set<Impuesto>();
  public DbSet<FormaPago> FormasPago => Set<FormaPago>();
  public DbSet<Banco> Bancos => Set<Banco>();
*/

// Y en OnModelCreating, agrega:
/*
  modelBuilder.ApplyConfiguration(new PaisConfiguration());
  modelBuilder.ApplyConfiguration(new EstadoProvinciaConfiguration());
  modelBuilder.ApplyConfiguration(new CiudadConfiguration());
  modelBuilder.ApplyConfiguration(new MonedaConfiguration());
  modelBuilder.ApplyConfiguration(new TipoIdentificacionConfiguration());
  modelBuilder.ApplyConfiguration(new ImpuestoConfiguration());
  modelBuilder.ApplyConfiguration(new FormaPagoConfiguration());
  modelBuilder.ApplyConfiguration(new BancoConfiguration());
*/


// =====================================================
// INFRASTRUCTURE LAYER - Dependency Injection
// Archivo: Infrastructure/DependencyInjection.cs
// AGREGAR estas líneas al método AddInfrastructure existente
// =====================================================

// NOTA: Agrega estas líneas en el método AddInfrastructure(IServiceCollection services)

/*
  // Repositorios de Catálogos
  services.AddScoped<IPaisRepository, PaisRepository>();
  services.AddScoped<IEstadoProvinciaRepository, EstadoProvinciaRepository>();
  services.AddScoped<ICiudadRepository, CiudadRepository>();
  services.AddScoped<IMonedaRepository, MonedaRepository>();
  services.AddScoped<ITipoIdentificacionRepository, TipoIdentificacionRepository>();
  services.AddScoped<IImpuestoRepository, ImpuestoRepository>();
  services.AddScoped<IFormaPagoRepository, FormaPagoRepository>();
  services.AddScoped<IBancoRepository, BancoRepository>();
*/


// =====================================================
// APPLICATION LAYER - Dependency Injection
// Archivo: Application/DependencyInjection.cs
// AGREGAR estas líneas al método AddApplication existente
// =====================================================

// NOTA: Agrega estas líneas en el método AddApplication(IServiceCollection services)

/*
  // Servicios de Catálogos
  services.AddScoped<IPaisService, PaisService>();
  services.AddScoped<IEstadoProvinciaService, EstadoProvinciaService>();
  services.AddScoped<ICiudadService, CiudadService>();
  services.AddScoped<IMonedaService, MonedaService>();
  services.AddScoped<ITipoIdentificacionService, TipoIdentificacionService>();
  services.AddScoped<IImpuestoService, ImpuestoService>();
  services.AddScoped<IFormaPagoService, FormaPagoService>();
  services.AddScoped<IBancoService, BancoService>();

  // Validadores de Catálogos (si usas el registro automático de FluentValidation)
  // Ya se registran automáticamente si usas:
  // services.AddValidatorsFromAssemblyContaining<CrearPaisValidator>();
*/
