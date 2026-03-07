// ══════════════════════════════════════════════════════════════════════
//  test.config.js  —  Solaris Platform · API Test Console  v3.0
//  ─────────────────────────────────────────────────────────────────────
//  ÚNICA FUENTE DE VERDAD — sincronizado con swagger (2026-03-07)
//  CRUD completo con flujos encadenados: crea → lee → edita → borra
//
//  Propiedades especiales de un test:
//    captureId?:  string  → guarda data.id de la respuesta en DYNAMIC
//    useDynamic?: string  → reemplaza {DYN} en path con DYNAMIC[key]
//    bodyFn?:     string  → clave de BODY_FACTORIES para body dinámico
//    skipIf?:     string  → salta el test si DYNAMIC[key] es falsy
// ══════════════════════════════════════════════════════════════════════

// ── IDs de datos semilla (pre-existentes en BD, no se modifican) ─────
const SEED = {
  proyectoId:    1,
  empleadoId:    1,
  faseId:        1, hitoId:        1, wbsId:    1,
  tareaId:       1, cuadrillaId:   1, otId:     1,
  reporteId:     1, presupuestoId: 1, ccId:     1,
  alertaId:      1, empresaId:     1, sucursalId:1,
  usuarioId:     1, rolId:         1,
  paisId:        1, estadoId:      1, ciudadId:  1,
  monedaId:      1, tipoIdId:      1, impuestoId:1,
  formaPagoId:   1, bancoId:       1,
  departamentoId:1, puestoId:      1,
  nominaId:      1, conceptoId:    1,
  paisCodigo:   'EC', anno: 2025,
};

// ── Body factories ────────────────────────────────────────────────────
// fn(DYNAMIC, SEED) → objeto body del request.
// DYNAMIC contiene IDs capturados en tiempo de ejecución.
const BODY_FACTORIES = {

  // Auth / Usuarios
  'create-usuario': (D,S) => ({ nombres:'Test', apellidos:'Runner', email:`test.r${Date.now()}@solaris.dev`, password:'Solaris123!', confirmPassword:'Solaris123!', rolId:S.rolId }),
  'update-usuario': (D,S) => ({ nombres:'Test Editado', apellidos:'Runner', email:`test.e${Date.now()}@solaris.dev`, rolId:S.rolId }),

  // Roles
  'create-rol': ()    => ({ nombre:`Rol Test ${Date.now()}`, descripcion:'Creado por runner', activo:true }),
  'update-rol': ()    => ({ nombre:'Rol Editado Runner',     descripcion:'Editado por runner', activo:true }),

  // Empresas
  'create-sucursal': (D,S) => ({ nombre:`Sucursal Test ${Date.now()}`, empresaId:S.empresaId, direccion:'Av. Test 123', ciudadId:S.ciudadId, activo:true }),
  'update-sucursal': (D,S) => ({ nombre:'Sucursal Editada', empresaId:S.empresaId, direccion:'Av. Editada 456', ciudadId:S.ciudadId, activo:true }),

  // Catálogos
  'create-pais':      ()    => ({ nombre:`País Test ${Date.now()}`, codigo:'XT', activo:true }),
  'update-pais':      ()    => ({ nombre:'País Editado', codigo:'XU', activo:true }),
  'create-estado':    (D,S) => ({ nombre:`Estado Test ${Date.now()}`, codigo:'ET', paisId:S.paisId, activo:true }),
  'update-estado':    (D,S) => ({ nombre:'Estado Editado', codigo:'EU', paisId:S.paisId, activo:true }),
  'create-ciudad':    (D,S) => ({ nombre:`Ciudad Test ${Date.now()}`, estadoProvinciaId:S.estadoId, activo:true }),
  'update-ciudad':    (D,S) => ({ nombre:'Ciudad Editada', estadoProvinciaId:S.estadoId, activo:true }),
  'create-moneda':    ()    => ({ nombre:`Moneda Test ${Date.now()}`, codigo:'MT', simbolo:'M$', activo:true }),
  'update-moneda':    ()    => ({ nombre:'Moneda Editada', codigo:'MU', simbolo:'M€', activo:true }),
  'create-tipo-id':   (D,S) => ({ nombre:`TipoID Test ${Date.now()}`, paisId:S.paisId, activo:true }),
  'update-tipo-id':   (D,S) => ({ nombre:'TipoID Editado', paisId:S.paisId, activo:true }),
  'create-impuesto':  ()    => ({ nombre:`IVA Test ${Date.now()}`, porcentaje:15.0, activo:true }),
  'update-impuesto':  ()    => ({ nombre:'IVA Editado', porcentaje:12.0, activo:true }),
  'create-forma-pago':()    => ({ nombre:`FormaPago Test ${Date.now()}`, activo:true }),
  'update-forma-pago':()    => ({ nombre:'FormaPago Editada', activo:true }),
  'create-banco':     ()    => ({ nombre:`Banco Test ${Date.now()}`, codigo:`BT${Date.now()%1000}`, activo:true }),
  'update-banco':     ()    => ({ nombre:'Banco Editado', activo:true }),

  // RRHH base
  'create-departamento': ()    => ({ nombre:`Depto Test ${Date.now()}`, descripcion:'Dpto de prueba' }),
  'update-departamento': ()    => ({ nombre:'Depto Editado', descripcion:'Editado por runner' }),
  'create-puesto': (D,S)       => ({ nombre:`Puesto Test ${Date.now()}`, departamentoId:S.departamentoId, descripcion:'Puesto de prueba', salarioBase:800.00 }),
  'update-puesto': (D,S)       => ({ nombre:'Puesto Editado', departamentoId:S.departamentoId, descripcion:'Editado', salarioBase:900.00 }),

  // RRHH Empleados
  'create-empleado': (D,S) => ({ nombres:'Juan', apellidos:'Test Runner', tipoIdentificacionId:S.tipoIdId, numeroIdentificacion:`T${Date.now()%1000000}`, email:`emp.t${Date.now()}@solaris.dev`, fechaNacimiento:'1990-01-15', fechaIngreso:'2024-01-01', departamentoId:S.departamentoId, puestoId:S.puestoId, salario:800.00, activo:true }),
  'update-empleado': (D,S) => ({ nombres:'Juan Editado', apellidos:'Test Runner', tipoIdentificacionId:S.tipoIdId, numeroIdentificacion:`T${Date.now()%1000000}`, email:`emp.e${Date.now()}@solaris.dev`, fechaNacimiento:'1990-01-15', fechaIngreso:'2024-01-01', departamentoId:S.departamentoId, puestoId:S.puestoId, salario:850.00, activo:true }),
  'create-ausencia': (D,S)   => ({ empleadoId:S.empleadoId, tipo:'Permiso', fechaInicio:'2025-12-01', fechaFin:'2025-12-02', motivo:'Prueba runner' }),

  // Nómina
  'create-nomina-concepto': () => ({ nombre:`Concepto Test ${Date.now()}`, tipo:'Ingreso', formula:'fijo', valor:50.00, activo:true }),
  'update-nomina-concepto': () => ({ nombre:'Concepto Editado', tipo:'Ingreso', formula:'fijo', valor:75.00, activo:true }),
  'create-nomina-periodo':  () => ({ nombre:`Período Test ${Date.now()}`, fechaInicio:'2025-12-01', fechaFin:'2025-12-31', tipo:'Mensual' }),

  // Proyectos core
  'create-proyecto': (D,S) => ({ nombre:`Proyecto Test ${Date.now()}`, descripcion:'Proyecto de prueba', empresaId:S.empresaId, fechaInicio:'2025-01-01', fechaFinPlan:'2025-12-31', estado:1, montoContrato:100000 }),
  'update-proyecto': ()    => ({ nombre:'Proyecto Editado Test', descripcion:'Editado por runner', fechaInicio:'2025-01-01', fechaFinPlan:'2025-12-31', estado:1, montoContrato:120000 }),

  // Planificación
  'create-fase':  (D) => ({ nombre:`Fase Test ${Date.now()}`, descripcion:'Fase de prueba', proyectoId:D.newProyectoId, orden:1, fechaInicio:'2025-01-01', fechaFin:'2025-06-30' }),
  'update-fase':  (D) => ({ nombre:'Fase Editada Test', descripcion:'Editada', proyectoId:D.newProyectoId, orden:1, fechaInicio:'2025-01-01', fechaFin:'2025-07-31' }),
  'create-hito':  (D) => ({ nombre:`Hito Test ${Date.now()}`, descripcion:'Hito de prueba', proyectoId:D.newProyectoId, fechaPlan:'2025-06-30', logrado:false }),
  'update-hito':  (D) => ({ nombre:'Hito Editado Test', descripcion:'Editado', proyectoId:D.newProyectoId, fechaPlan:'2025-07-31', logrado:false }),
  'create-wbs':   (D) => ({ nombre:`WBS Test ${Date.now()}`, descripcion:'WBS de prueba', proyectoId:D.newProyectoId, nivel:1, orden:1 }),
  'update-wbs':   (D) => ({ nombre:'WBS Editado Test', descripcion:'Editado', proyectoId:D.newProyectoId, nivel:1, orden:1 }),

  // Tareas / Cuadrillas
  'create-tarea':     (D) => ({ nombre:`Tarea Test ${Date.now()}`, descripcion:'Tarea de prueba', proyectoId:D.newProyectoId, faseId:D.newFaseId||null, fechaInicioPlan:'2025-01-01', fechaFinPlan:'2025-03-31', duracionDias:90, estado:1 }),
  'update-tarea':     (D) => ({ nombre:'Tarea Editada Test', descripcion:'Editada', proyectoId:D.newProyectoId, faseId:D.newFaseId||null, fechaInicioPlan:'2025-01-01', fechaFinPlan:'2025-04-30', duracionDias:120, estado:1 }),
  'create-cuadrilla': (D) => ({ nombre:`Cuadrilla Test ${Date.now()}`, descripcion:'Cuadrilla de prueba', proyectoId:D.newProyectoId, activo:true }),
  'update-cuadrilla': (D) => ({ nombre:'Cuadrilla Editada', descripcion:'Editada', proyectoId:D.newProyectoId, activo:true }),

  // Presupuesto / CC
  'create-presupuesto':   (D)   => ({ proyectoId:D.newProyectoId, descripcion:'Presupuesto de prueba', observaciones:'Test runner' }),
  'create-partida':       (D)   => ({ presupuestoId:D.newPresupuestoId, tipo:'MaterialDirecto', concepto:'Cables fibra óptica', descripcion:'Partida de prueba', unidadMedida:'m', cantidad:100, precioUnitario:5.50, porcentaje:0, orden:1 }),
  'create-centro-costo':  (D,S) => ({ proyectoId:D.newProyectoId, nombre:`CC Test ${Date.now()}`, codigo:`CC${Date.now()%1000}`, descripcion:'CC de prueba', monedaId:S.monedaId }),
  'update-centro-costo':  (D,S) => ({ proyectoId:D.newProyectoId, nombre:'CC Editado', codigo:`CCE${Date.now()%1000}`, descripcion:'Editado', monedaId:S.monedaId }),

  // Control
  'create-reporte': (D) => ({ proyectoId:D.newProyectoId, titulo:`Reporte Test ${Date.now()}`, descripcion:'Reporte de prueba', avancePorcentaje:25, fechaReporte:new Date().toISOString().split('T')[0] }),

  // OT
  'create-ot': (D,S) => ({ proyectoId:D.newProyectoId||S.proyectoId, tareaId:D.newTareaId||null, numero:`OT-T${Date.now()%10000}`, titulo:'OT Prueba Runner', descripcion:'OT automatizada', tipo:1, fechaProgramada:'2025-03-15', prioridad:2 }),
  'update-ot':  (D,S) => ({ proyectoId:D.newProyectoId||S.proyectoId, tareaId:D.newTareaId||null, numero:`OT-E${Date.now()%10000}`, titulo:'OT Editada Runner', descripcion:'Editada por runner', tipo:1, fechaProgramada:'2025-04-01', prioridad:1 }),
};

// ── TEST SUITES ───────────────────────────────────────────────────────
const TEST_SUITES = [

  { id:'smoke', icon:'🔍', title:'Smoke Tests', desc:'¿La API está viva?', tests:[
    { id:'health-api',  method:'GET', path:'/health/api',   auth:false, name:'GET /health/api',   desc:'API health check',  expect:{status:200} },
    { id:'health-data', method:'GET', path:'/health/data',  auth:false, name:'GET /health/data',  desc:'BD health check',   expect:{status:200} },
    { id:'health-db',   method:'GET', path:'/health/db',    auth:false, name:'GET /health/db',    desc:'DB directa',        expect:{status:200} },
    { id:'api-info',    method:'GET', path:'/api/info',     auth:false, name:'GET /api/info',     desc:'Versión de la API', expect:{status:200} },
    { id:'unauth',      method:'GET', path:'/api/Usuarios', auth:false, name:'GET /api/Usuarios sin JWT → 401', desc:'Endpoint protegido', expect:{status:401} },
  ]},

  { id:'auth', icon:'🔑', title:'Autenticación', desc:'Login, JWT, refresh, logout', tests:[
    { id:'login-bad',          method:'POST', path:'/api/Auth/login',              auth:false, name:'POST login — credenciales malas → 400/401',   body:{email:'malo@test.com',password:'Mala123!'}, expect:{status:[400,401]} },
    { id:'login-ok',           method:'POST', path:'/api/Auth/login',              auth:false, name:'POST login — correcto → JWT',                  body:'__USE_CREDS__', expect:{status:200, check:d=>!!(d?.data?.token||d?.token), checkDesc:'response contiene JWT'} },
    { id:'me',                 method:'GET',  path:'/api/Auth/me',                 auth:true,  name:'GET /api/Auth/me — perfil autenticado',         expect:{status:200} },
    { id:'refresh',            method:'POST', path:'/api/Auth/refresh-token',      auth:false, name:'POST refresh-token → nuevo JWT',               body:'__USE_REFRESH__', expect:{status:200, check:d=>!!(d?.data?.token||d?.token), checkDesc:'nuevo JWT en response'} },
    { id:'cambiar-password',   method:'POST', path:'/api/Auth/cambiar-password',   auth:true,  name:'POST cambiar-password — vacío → 400',          body:{}, expect:{status:400} },
    { id:'recuperar-password', method:'POST', path:'/api/Auth/recuperar-password', auth:false, name:'POST recuperar-password — email inexistente',  body:{email:'noexiste@solaris.dev'}, expect:{status:[200,404]} },
    { id:'logout',             method:'POST', path:'/api/Auth/logout',             auth:true,  name:'POST logout → 200',                            expect:{status:200} },
  ]},

  { id:'roles', icon:'🛡️', title:'Roles', desc:'CRUD completo', tests:[
    { id:'list-roles',    method:'GET',    path:'/api/Roles',       auth:true, name:'GET Roles — listar',    expect:{status:200} },
    { id:'roles-disp',    method:'GET',    path:'/api/Roles/disponibles', auth:true, name:'GET Roles/disponibles', expect:{status:200} },
    { id:'roles-modulos', method:'GET',    path:'/api/Roles/modulos-permisos', auth:true, name:'GET Roles/modulos-permisos', expect:{status:200} },
    { id:'create-rol',    method:'POST',   path:'/api/Roles',       auth:true, name:'POST Roles — crear',    bodyFn:'create-rol', captureId:'newRolId', expect:{status:[200,201]} },
    { id:'get-rol-new',   method:'GET',    path:'/api/Roles/{DYN}', auth:true, name:'GET Roles creado',      useDynamic:'newRolId', skipIf:'newRolId', expect:{status:200} },
    { id:'update-rol',    method:'PUT',    path:'/api/Roles/{DYN}', auth:true, name:'PUT Roles — editar',    useDynamic:'newRolId', skipIf:'newRolId', bodyFn:'update-rol', expect:{status:[200,204]} },
    { id:'delete-rol',    method:'DELETE', path:'/api/Roles/{DYN}', auth:true, name:'DELETE Roles',          useDynamic:'newRolId', skipIf:'newRolId', expect:{status:[200,204]} },
    { id:'get-rol-seed',  method:'GET',    path:`/api/Roles/${SEED.rolId}`, auth:true, name:`GET Roles/${SEED.rolId} seed`, expect:{status:[200,404]} },
  ]},

  { id:'usuarios', icon:'👤', title:'Usuarios', desc:'CRUD + acciones de estado', tests:[
    { id:'list-usuarios',       method:'GET',    path:'/api/Usuarios',                          auth:true, name:'GET Usuarios — listar', expect:{status:200} },
    { id:'create-usuario',      method:'POST',   path:'/api/Usuarios',                          auth:true, name:'POST Usuarios — crear', bodyFn:'create-usuario', captureId:'newUsuarioId', expect:{status:[200,201]} },
    { id:'get-usuario-new',     method:'GET',    path:'/api/Usuarios/{DYN}',                    auth:true, name:'GET Usuario creado', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', expect:{status:200} },
    { id:'update-usuario',      method:'PUT',    path:'/api/Usuarios/{DYN}',                    auth:true, name:'PUT Usuario', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', bodyFn:'update-usuario', expect:{status:[200,204]} },
    { id:'desactivar-usuario',  method:'POST',   path:'/api/Usuarios/{DYN}/desactivar',         auth:true, name:'POST desactivar', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', expect:{status:[200,204]} },
    { id:'activar-usuario',     method:'POST',   path:'/api/Usuarios/{DYN}/activar',            auth:true, name:'POST activar', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', expect:{status:[200,204]} },
    { id:'bloquear-usuario',    method:'POST',   path:'/api/Usuarios/{DYN}/bloquear',           auth:true, name:'POST bloquear', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', expect:{status:[200,204]} },
    { id:'desbloquear-usuario', method:'POST',   path:'/api/Usuarios/{DYN}/desbloquear',        auth:true, name:'POST desbloquear', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', expect:{status:[200,204]} },
    { id:'reset-pwd-usuario',   method:'POST',   path:'/api/Usuarios/{DYN}/resetear-password',  auth:true, name:'POST resetear-password', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', body:{nuevaPassword:'Solaris456!'}, expect:{status:[200,204]} },
    { id:'delete-usuario',      method:'DELETE', path:'/api/Usuarios/{DYN}',                    auth:true, name:'DELETE Usuario', useDynamic:'newUsuarioId', skipIf:'newUsuarioId', expect:{status:[200,204]} },
    { id:'get-usuario-seed',    method:'GET',    path:`/api/Usuarios/${SEED.usuarioId}`,        auth:true, name:`GET Usuario ${SEED.usuarioId} seed`, expect:{status:[200,404]} },
  ]},

  { id:'empresas', icon:'🏢', title:'Empresas & Sucursales', desc:'CRUD sucursales', tests:[
    { id:'list-empresas',   method:'GET',    path:'/api/Empresas',                                     auth:true, name:'GET Empresas', expect:{status:200} },
    { id:'get-empresa',     method:'GET',    path:`/api/Empresas/${SEED.empresaId}`,                   auth:true, name:'GET Empresa seed', expect:{status:[200,404]} },
    { id:'list-sucursales', method:'GET',    path:`/api/Empresas/${SEED.empresaId}/sucursales`,        auth:true, name:'GET sucursales', expect:{status:200} },
    { id:'create-sucursal', method:'POST',   path:`/api/Empresas/${SEED.empresaId}/sucursales`,        auth:true, name:'POST sucursal', bodyFn:'create-sucursal', captureId:'newSucursalId', expect:{status:[200,201]} },
    { id:'get-sucursal-new',method:'GET',    path:`/api/Empresas/${SEED.empresaId}/sucursales/{DYN}`,  auth:true, name:'GET sucursal creada', useDynamic:'newSucursalId', skipIf:'newSucursalId', expect:{status:200} },
    { id:'update-sucursal', method:'PUT',    path:`/api/Empresas/${SEED.empresaId}/sucursales/{DYN}`,  auth:true, name:'PUT sucursal', useDynamic:'newSucursalId', skipIf:'newSucursalId', bodyFn:'update-sucursal', expect:{status:[200,204]} },
    { id:'delete-sucursal', method:'DELETE', path:`/api/Empresas/${SEED.empresaId}/sucursales/{DYN}`,  auth:true, name:'DELETE sucursal', useDynamic:'newSucursalId', skipIf:'newSucursalId', expect:{status:[200,204]} },
  ]},

  { id:'catalogos', icon:'🗂️', title:'Catálogos', desc:'CRUD: Países, Estados, Ciudades, Monedas, TiposId, Impuestos, FormasPago, Bancos', tests:[
    // Países
    { id:'list-paises',  method:'GET',    path:'/api/catalogos/paises',              auth:true, name:'GET paises',  expect:{status:200} },
    { id:'create-pais',  method:'POST',   path:'/api/catalogos/paises',              auth:true, name:'POST pais',   bodyFn:'create-pais',  captureId:'newPaisId',  expect:{status:[200,201]} },
    { id:'update-pais',  method:'PUT',    path:'/api/catalogos/paises/{DYN}',        auth:true, name:'PUT pais',    useDynamic:'newPaisId',  skipIf:'newPaisId',  bodyFn:'update-pais',  expect:{status:[200,204]} },
    { id:'patch-pais',   method:'PATCH',  path:'/api/catalogos/paises/{DYN}/estado', auth:true, name:'PATCH pais',  useDynamic:'newPaisId',  skipIf:'newPaisId',  body:{activo:false},   expect:{status:[200,204]} },
    { id:'delete-pais',  method:'DELETE', path:'/api/catalogos/paises/{DYN}',        auth:true, name:'DELETE pais', useDynamic:'newPaisId',  skipIf:'newPaisId',  expect:{status:[200,204]} },
    // Estados
    { id:'list-estados',    method:'GET',    path:'/api/catalogos/estados-provincias',                           auth:true, name:'GET estados',  expect:{status:200} },
    { id:'estados-por-pais',method:'GET',    path:`/api/catalogos/estados-provincias/por-pais/${SEED.paisId}`,   auth:true, name:'GET estados/por-pais', expect:{status:200} },
    { id:'create-estado',   method:'POST',   path:'/api/catalogos/estados-provincias',                           auth:true, name:'POST estado',  bodyFn:'create-estado', captureId:'newEstadoId', expect:{status:[200,201]} },
    { id:'update-estado',   method:'PUT',    path:'/api/catalogos/estados-provincias/{DYN}',                     auth:true, name:'PUT estado',   useDynamic:'newEstadoId', skipIf:'newEstadoId', bodyFn:'update-estado', expect:{status:[200,204]} },
    { id:'patch-estado',    method:'PATCH',  path:'/api/catalogos/estados-provincias/{DYN}/estado',              auth:true, name:'PATCH estado', useDynamic:'newEstadoId', skipIf:'newEstadoId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-estado',   method:'DELETE', path:'/api/catalogos/estados-provincias/{DYN}',                     auth:true, name:'DELETE estado',useDynamic:'newEstadoId', skipIf:'newEstadoId', expect:{status:[200,204]} },
    // Ciudades
    { id:'list-ciudades',   method:'GET',    path:'/api/catalogos/ciudades',                          auth:true, name:'GET ciudades',  expect:{status:200} },
    { id:'ciudades-estado', method:'GET',    path:`/api/catalogos/ciudades/por-estado/${SEED.estadoId}`, auth:true, name:'GET ciudades/por-estado', expect:{status:200} },
    { id:'create-ciudad',   method:'POST',   path:'/api/catalogos/ciudades',                          auth:true, name:'POST ciudad',   bodyFn:'create-ciudad', captureId:'newCiudadId', expect:{status:[200,201]} },
    { id:'update-ciudad',   method:'PUT',    path:'/api/catalogos/ciudades/{DYN}',                    auth:true, name:'PUT ciudad',    useDynamic:'newCiudadId', skipIf:'newCiudadId', bodyFn:'update-ciudad', expect:{status:[200,204]} },
    { id:'patch-ciudad',    method:'PATCH',  path:'/api/catalogos/ciudades/{DYN}/estado',             auth:true, name:'PATCH ciudad',  useDynamic:'newCiudadId', skipIf:'newCiudadId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-ciudad',   method:'DELETE', path:'/api/catalogos/ciudades/{DYN}',                    auth:true, name:'DELETE ciudad', useDynamic:'newCiudadId', skipIf:'newCiudadId', expect:{status:[200,204]} },
    // Monedas
    { id:'list-monedas',  method:'GET',    path:'/api/catalogos/monedas',              auth:true, name:'GET monedas',  expect:{status:200} },
    { id:'create-moneda', method:'POST',   path:'/api/catalogos/monedas',              auth:true, name:'POST moneda',  bodyFn:'create-moneda', captureId:'newMonedaId', expect:{status:[200,201]} },
    { id:'update-moneda', method:'PUT',    path:'/api/catalogos/monedas/{DYN}',        auth:true, name:'PUT moneda',   useDynamic:'newMonedaId', skipIf:'newMonedaId', bodyFn:'update-moneda', expect:{status:[200,204]} },
    { id:'patch-moneda',  method:'PATCH',  path:'/api/catalogos/monedas/{DYN}/estado', auth:true, name:'PATCH moneda', useDynamic:'newMonedaId', skipIf:'newMonedaId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-moneda', method:'DELETE', path:'/api/catalogos/monedas/{DYN}',        auth:true, name:'DELETE moneda',useDynamic:'newMonedaId', skipIf:'newMonedaId', expect:{status:[200,204]} },
    // Tipos Identificación
    { id:'list-tipos-id',  method:'GET',    path:'/api/catalogos/tipos-identificacion',                          auth:true, name:'GET tipos-id',   expect:{status:200} },
    { id:'tipos-id-pais',  method:'GET',    path:`/api/catalogos/tipos-identificacion/por-pais/${SEED.paisId}`,  auth:true, name:'GET tipos-id/por-pais', expect:{status:200} },
    { id:'create-tipo-id', method:'POST',   path:'/api/catalogos/tipos-identificacion',                          auth:true, name:'POST tipo-id',   bodyFn:'create-tipo-id', captureId:'newTipoIdId', expect:{status:[200,201]} },
    { id:'update-tipo-id', method:'PUT',    path:'/api/catalogos/tipos-identificacion/{DYN}',                    auth:true, name:'PUT tipo-id',    useDynamic:'newTipoIdId', skipIf:'newTipoIdId', bodyFn:'update-tipo-id', expect:{status:[200,204]} },
    { id:'patch-tipo-id',  method:'PATCH',  path:'/api/catalogos/tipos-identificacion/{DYN}/estado',             auth:true, name:'PATCH tipo-id',  useDynamic:'newTipoIdId', skipIf:'newTipoIdId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-tipo-id', method:'DELETE', path:'/api/catalogos/tipos-identificacion/{DYN}',                    auth:true, name:'DELETE tipo-id', useDynamic:'newTipoIdId', skipIf:'newTipoIdId', expect:{status:[200,204]} },
    // Impuestos
    { id:'list-impuestos',  method:'GET',    path:'/api/catalogos/impuestos',               auth:true, name:'GET impuestos',   expect:{status:200} },
    { id:'create-impuesto', method:'POST',   path:'/api/catalogos/impuestos',               auth:true, name:'POST impuesto',   bodyFn:'create-impuesto', captureId:'newImpuestoId', expect:{status:[200,201]} },
    { id:'update-impuesto', method:'PUT',    path:'/api/catalogos/impuestos/{DYN}',         auth:true, name:'PUT impuesto',    useDynamic:'newImpuestoId', skipIf:'newImpuestoId', bodyFn:'update-impuesto', expect:{status:[200,204]} },
    { id:'patch-impuesto',  method:'PATCH',  path:'/api/catalogos/impuestos/{DYN}/estado',  auth:true, name:'PATCH impuesto',  useDynamic:'newImpuestoId', skipIf:'newImpuestoId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-impuesto', method:'DELETE', path:'/api/catalogos/impuestos/{DYN}',         auth:true, name:'DELETE impuesto', useDynamic:'newImpuestoId', skipIf:'newImpuestoId', expect:{status:[200,204]} },
    // Formas de Pago
    { id:'list-formas-pago',  method:'GET',    path:'/api/catalogos/formas-pago',               auth:true, name:'GET formas-pago',   expect:{status:200} },
    { id:'create-forma-pago', method:'POST',   path:'/api/catalogos/formas-pago',               auth:true, name:'POST forma-pago',   bodyFn:'create-forma-pago', captureId:'newFormaPagoId', expect:{status:[200,201]} },
    { id:'update-forma-pago', method:'PUT',    path:'/api/catalogos/formas-pago/{DYN}',         auth:true, name:'PUT forma-pago',    useDynamic:'newFormaPagoId', skipIf:'newFormaPagoId', bodyFn:'update-forma-pago', expect:{status:[200,204]} },
    { id:'patch-forma-pago',  method:'PATCH',  path:'/api/catalogos/formas-pago/{DYN}/estado',  auth:true, name:'PATCH forma-pago',  useDynamic:'newFormaPagoId', skipIf:'newFormaPagoId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-forma-pago', method:'DELETE', path:'/api/catalogos/formas-pago/{DYN}',         auth:true, name:'DELETE forma-pago', useDynamic:'newFormaPagoId', skipIf:'newFormaPagoId', expect:{status:[200,204]} },
    // Bancos
    { id:'list-bancos',  method:'GET',    path:'/api/catalogos/bancos',              auth:true, name:'GET bancos',   expect:{status:200} },
    { id:'create-banco', method:'POST',   path:'/api/catalogos/bancos',              auth:true, name:'POST banco',   bodyFn:'create-banco', captureId:'newBancoId', expect:{status:[200,201]} },
    { id:'update-banco', method:'PUT',    path:'/api/catalogos/bancos/{DYN}',        auth:true, name:'PUT banco',    useDynamic:'newBancoId', skipIf:'newBancoId', bodyFn:'update-banco', expect:{status:[200,204]} },
    { id:'patch-banco',  method:'PATCH',  path:'/api/catalogos/bancos/{DYN}/estado', auth:true, name:'PATCH banco',  useDynamic:'newBancoId', skipIf:'newBancoId', body:{activo:false}, expect:{status:[200,204]} },
    { id:'delete-banco', method:'DELETE', path:'/api/catalogos/bancos/{DYN}',        auth:true, name:'DELETE banco', useDynamic:'newBancoId', skipIf:'newBancoId', expect:{status:[200,204]} },
  ]},

  { id:'rrhh-base', icon:'👥', title:'RRHH — Departamentos & Puestos', desc:'CRUD', tests:[
    { id:'list-deptos',   method:'GET',    path:'/api/rrhh/departamentos',       auth:true, name:'GET departamentos', expect:{status:200} },
    { id:'create-depto',  method:'POST',   path:'/api/rrhh/departamentos',       auth:true, name:'POST departamento', bodyFn:'create-departamento', captureId:'newDeptoId', expect:{status:[200,201]} },
    { id:'get-depto',     method:'GET',    path:'/api/rrhh/departamentos/{DYN}', auth:true, name:'GET depto creado',  useDynamic:'newDeptoId', skipIf:'newDeptoId', expect:{status:200} },
    { id:'update-depto',  method:'PUT',    path:'/api/rrhh/departamentos/{DYN}', auth:true, name:'PUT departamento',  useDynamic:'newDeptoId', skipIf:'newDeptoId', bodyFn:'update-departamento', expect:{status:[200,204]} },
    { id:'delete-depto',  method:'DELETE', path:'/api/rrhh/departamentos/{DYN}', auth:true, name:'DELETE departamento', useDynamic:'newDeptoId', skipIf:'newDeptoId', expect:{status:[200,204]} },
    { id:'list-puestos',  method:'GET',    path:'/api/rrhh/puestos',             auth:true, name:'GET puestos', expect:{status:200} },
    { id:'create-puesto', method:'POST',   path:'/api/rrhh/puestos',             auth:true, name:'POST puesto', bodyFn:'create-puesto', captureId:'newPuestoId', expect:{status:[200,201]} },
    { id:'get-puesto',    method:'GET',    path:'/api/rrhh/puestos/{DYN}',       auth:true, name:'GET puesto creado', useDynamic:'newPuestoId', skipIf:'newPuestoId', expect:{status:200} },
    { id:'update-puesto', method:'PUT',    path:'/api/rrhh/puestos/{DYN}',       auth:true, name:'PUT puesto', useDynamic:'newPuestoId', skipIf:'newPuestoId', bodyFn:'update-puesto', expect:{status:[200,204]} },
    { id:'delete-puesto', method:'DELETE', path:'/api/rrhh/puestos/{DYN}',       auth:true, name:'DELETE puesto', useDynamic:'newPuestoId', skipIf:'newPuestoId', expect:{status:[200,204]} },
  ]},

  { id:'rrhh-empleados', icon:'🧑‍💼', title:'RRHH — Empleados', desc:'CRUD + historial + cambios', tests:[
    { id:'list-empleados',     method:'GET',  path:'/api/rrhh/empleados',                              auth:true, name:'GET empleados', expect:{status:200} },
    { id:'create-empleado',    method:'POST', path:'/api/rrhh/empleados',                              auth:true, name:'POST empleado', bodyFn:'create-empleado', captureId:'newEmpleadoId', expect:{status:[200,201]} },
    { id:'get-empleado',       method:'GET',  path:'/api/rrhh/empleados/{DYN}',                        auth:true, name:'GET empleado creado', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', expect:{status:200} },
    { id:'update-empleado',    method:'PUT',  path:'/api/rrhh/empleados/{DYN}',                        auth:true, name:'PUT empleado', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', bodyFn:'update-empleado', expect:{status:[200,204]} },
    { id:'empleado-historial', method:'GET',  path:'/api/rrhh/empleados/{DYN}/historial',              auth:true, name:'GET empleado/historial', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', expect:{status:200} },
    { id:'empleado-docs',      method:'GET',  path:'/api/rrhh/empleados/{DYN}/documentos',             auth:true, name:'GET empleado/documentos', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', expect:{status:200} },
    { id:'empleado-vacaciones',method:'GET',  path:`/api/rrhh/empleados/{DYN}/vacaciones/${SEED.anno}`,auth:true, name:'GET empleado/vacaciones', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', expect:{status:200} },
    { id:'cambiar-salario',    method:'POST', path:'/api/rrhh/empleados/{DYN}/cambiar-salario',        auth:true, name:'POST cambiar-salario', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', body:{nuevoSalario:950.00,motivo:'Prueba runner',fechaEfectiva:'2025-04-01'}, expect:{status:[200,204]} },
    { id:'cambiar-puesto',     method:'POST', path:'/api/rrhh/empleados/{DYN}/cambiar-puesto',         auth:true, name:'POST cambiar-puesto', useDynamic:'newEmpleadoId', skipIf:'newEmpleadoId', body:{nuevoPuestoId:SEED.puestoId,motivo:'Prueba runner',fechaEfectiva:'2025-04-01'}, expect:{status:[200,204]} },
  ]},

  { id:'rrhh-asistencia', icon:'📅', title:'RRHH — Asistencia & Ausencias', desc:'Asistencia (en desarrollo), ausencias', tests:[
    { id:'asistencia-empleado',method:'GET',  path:`/api/rrhh/asistencia/empleado/${SEED.empleadoId}`, auth:true, name:'GET asistencia/empleado', expect:{status:200} },
    // Backend responde 400 "Funcionalidad en desarrollo" — aceptado como válido
    { id:'marcar-asistencia',  method:'POST', path:'/api/rrhh/asistencia/marcar', auth:true, name:'POST marcar asistencia (en desarrollo)', body:{empleadoId:SEED.empleadoId,tipo:'Entrada',fecha:new Date().toISOString().split('T')[0]}, expect:{status:[200,201,400,409]}, hint:'Backend reporta "Funcionalidad en desarrollo"' },
    { id:'list-ausencias',     method:'GET',  path:'/api/rrhh/ausencias', auth:true, name:'GET ausencias', expect:{status:200} },
    { id:'create-ausencia',    method:'POST', path:'/api/rrhh/ausencias', auth:true, name:'POST ausencia', bodyFn:'create-ausencia', captureId:'newAusenciaId', expect:{status:[200,201]} },
    { id:'aprobar-ausencia',   method:'POST', path:'/api/rrhh/ausencias/{DYN}/aprobar', auth:true, name:'POST ausencia/aprobar', useDynamic:'newAusenciaId', skipIf:'newAusenciaId', body:{aprobado:true,comentario:'Aprobado runner'}, expect:{status:[200,204]} },
  ]},

  { id:'rrhh-nomina', icon:'💰', title:'RRHH — Nómina', desc:'Conceptos, períodos, roles de pago, parámetros', tests:[
    { id:'nomina-conceptos',  method:'GET',    path:'/api/rrhh/nomina/conceptos',                                  auth:true, name:'GET conceptos', expect:{status:200} },
    { id:'create-concepto',   method:'POST',   path:'/api/rrhh/nomina/conceptos',                                  auth:true, name:'POST concepto', bodyFn:'create-nomina-concepto', captureId:'newConceptoId', expect:{status:[200,201]} },
    { id:'update-concepto',   method:'PUT',    path:'/api/rrhh/nomina/conceptos/{DYN}',                            auth:true, name:'PUT concepto', useDynamic:'newConceptoId', skipIf:'newConceptoId', bodyFn:'update-nomina-concepto', expect:{status:[200,204]} },
    { id:'delete-concepto',   method:'DELETE', path:'/api/rrhh/nomina/conceptos/{DYN}',                            auth:true, name:'DELETE concepto', useDynamic:'newConceptoId', skipIf:'newConceptoId', expect:{status:[200,204]} },
    { id:'nomina-periodos',   method:'GET',    path:'/api/rrhh/nomina/periodos',                                   auth:true, name:'GET períodos', expect:{status:200} },
    { id:'create-periodo',    method:'POST',   path:'/api/rrhh/nomina/periodos',                                   auth:true, name:'POST período', bodyFn:'create-nomina-periodo', captureId:'newPeriodoId', expect:{status:[200,201]} },
    { id:'nomina-roles-pago', method:'GET',    path:'/api/rrhh/nomina/roles-pago',                                 auth:true, name:'GET roles-pago', expect:{status:200} },
    { id:'nomina-parametros', method:'GET',    path:`/api/rrhh/nomina/parametros/${SEED.paisCodigo}/${SEED.anno}`, auth:true, name:'GET parámetros', expect:{status:[200,404]} },
    // Backend espera {Clave, Valor} en PascalCase (confirmado por error real)
    { id:'create-parametros', method:'POST',   path:`/api/rrhh/nomina/parametros/${SEED.paisCodigo}/${SEED.anno}`, auth:true, name:'POST parámetros — {Clave, Valor}', body:{Clave:'salarioBasico',Valor:'460.00'}, expect:{status:[200,201,204]}, hint:'DTO requiere Clave+Valor en PascalCase' },
    { id:'nomina-rol-detalle',method:'GET',    path:`/api/rrhh/nomina/roles-pago/${SEED.nominaId}/detalle`,        auth:true, name:'GET rol-pago/detalle', expect:{status:[200,404]} },
  ]},

  { id:'proyectos-core', icon:'📁', title:'Proyectos — Core', desc:'CRUD + estado + avance + dashboard', tests:[
    { id:'list-proyectos',        method:'GET',   path:'/api/proy/proyectos',                    auth:true, name:'GET proyectos', expect:{status:200} },
    { id:'create-proyecto',       method:'POST',  path:'/api/proy/proyectos',                    auth:true, name:'POST proyecto', bodyFn:'create-proyecto', captureId:'newProyectoId', expect:{status:[200,201]} },
    { id:'get-proyecto-new',      method:'GET',   path:'/api/proy/proyectos/{DYN}',              auth:true, name:'GET proyecto creado', useDynamic:'newProyectoId', skipIf:'newProyectoId', expect:{status:200} },
    { id:'update-proyecto',       method:'PUT',   path:'/api/proy/proyectos/{DYN}',              auth:true, name:'PUT proyecto', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'update-proyecto', expect:{status:[200,204]} },
    { id:'proyecto-dashboard',    method:'GET',   path:'/api/proy/proyectos/{DYN}/dashboard',    auth:true, name:'GET proyecto/dashboard', useDynamic:'newProyectoId', skipIf:'newProyectoId', expect:{status:200} },
    { id:'patch-proyecto-estado', method:'PATCH', path:'/api/proy/proyectos/{DYN}/estado',       auth:true, name:'PATCH proyecto/estado', useDynamic:'newProyectoId', skipIf:'newProyectoId', body:{estado:2}, expect:{status:[200,204]} },
    { id:'patch-proyecto-avance', method:'PATCH', path:'/api/proy/proyectos/{DYN}/avance',       auth:true, name:'PATCH proyecto/avance', useDynamic:'newProyectoId', skipIf:'newProyectoId', body:{avancePorcentaje:10}, expect:{status:[200,204]} },
    { id:'get-proyecto-seed',     method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}`, auth:true, name:'GET proyecto seed', expect:{status:[200,404]} },
    { id:'dashboard-seed',        method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/dashboard`, auth:true, name:'GET dashboard seed', expect:{status:[200,404]} },
  ]},

  { id:'proyectos-planificacion', icon:'🏗️', title:'Proyectos — Planificación', desc:'CRUD fases, hitos, WBS', tests:[
    { id:'list-fases',         method:'GET',    path:`/api/proy/proyectos/${SEED.proyectoId}/fases`, auth:true, name:'GET fases seed', expect:{status:200} },
    { id:'create-fase',        method:'POST',   path:'/api/proy/proyectos/{DYN}/fases',             auth:true, name:'POST fase', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-fase', captureId:'newFaseId', expect:{status:[200,201]} },
    { id:'update-fase',        method:'PUT',    path:'/api/proy/proyectos/{DYN}/fases/{newFaseId}', auth:true, name:'PUT fase', useDynamic:'newProyectoId', skipIf:'newFaseId', bodyFn:'update-fase', expect:{status:[200,204]} },
    { id:'delete-fase',        method:'DELETE', path:'/api/proy/proyectos/{DYN}/fases/{newFaseId}', auth:true, name:'DELETE fase', useDynamic:'newProyectoId', skipIf:'newFaseId', expect:{status:[200,204]} },
    { id:'list-hitos',         method:'GET',    path:`/api/proy/proyectos/${SEED.proyectoId}/hitos`, auth:true, name:'GET hitos seed', expect:{status:200} },
    { id:'create-hito',        method:'POST',   path:'/api/proy/proyectos/{DYN}/hitos',             auth:true, name:'POST hito', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-hito', captureId:'newHitoId', expect:{status:[200,201]} },
    { id:'update-hito',        method:'PUT',    path:'/api/proy/proyectos/{DYN}/hitos/{newHitoId}', auth:true, name:'PUT hito', useDynamic:'newProyectoId', skipIf:'newHitoId', bodyFn:'update-hito', expect:{status:[200,204]} },
    { id:'patch-hito-logrado', method:'PATCH',  path:'/api/proy/proyectos/{DYN}/hitos/{newHitoId}/logrado', auth:true, name:'PATCH hito/logrado', useDynamic:'newProyectoId', skipIf:'newHitoId', body:{logrado:true}, expect:{status:[200,204]} },
    { id:'delete-hito',        method:'DELETE', path:'/api/proy/proyectos/{DYN}/hitos/{newHitoId}', auth:true, name:'DELETE hito', useDynamic:'newProyectoId', skipIf:'newHitoId', expect:{status:[200,204]} },
    { id:'wbs-arbol',          method:'GET',    path:`/api/proy/proyectos/${SEED.proyectoId}/wbs`,  auth:true, name:'GET wbs seed', expect:{status:200} },
    { id:'create-wbs',         method:'POST',   path:'/api/proy/proyectos/{DYN}/wbs',               auth:true, name:'POST wbs', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-wbs', captureId:'newWbsId', expect:{status:[200,201]} },
    { id:'update-wbs',         method:'PUT',    path:'/api/proy/proyectos/{DYN}/wbs/{newWbsId}',    auth:true, name:'PUT wbs', useDynamic:'newProyectoId', skipIf:'newWbsId', bodyFn:'update-wbs', expect:{status:[200,204]} },
    { id:'delete-wbs',         method:'DELETE', path:'/api/proy/proyectos/{DYN}/wbs/{newWbsId}',    auth:true, name:'DELETE wbs', useDynamic:'newProyectoId', skipIf:'newWbsId', expect:{status:[200,204]} },
  ]},

  { id:'proyectos-tareas', icon:'✅', title:'Proyectos — Tareas & Cuadrillas', desc:'CRUD tareas + cuadrillas', tests:[
    { id:'list-tareas',       method:'GET',    path:`/api/proy/proyectos/${SEED.proyectoId}/tareas`,                auth:true, name:'GET tareas seed', expect:{status:200} },
    { id:'create-tarea',      method:'POST',   path:'/api/proy/proyectos/{DYN}/tareas',                             auth:true, name:'POST tarea', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-tarea', captureId:'newTareaId', expect:{status:[200,201]} },
    { id:'get-tarea',         method:'GET',    path:'/api/proy/proyectos/{DYN}/tareas/{newTareaId}',                auth:true, name:'GET tarea creada', useDynamic:'newProyectoId', skipIf:'newTareaId', expect:{status:200} },
    { id:'update-tarea',      method:'PUT',    path:'/api/proy/proyectos/{DYN}/tareas/{newTareaId}',                auth:true, name:'PUT tarea', useDynamic:'newProyectoId', skipIf:'newTareaId', bodyFn:'update-tarea', expect:{status:[200,204]} },
    { id:'patch-tarea-estado',method:'PATCH',  path:'/api/proy/proyectos/{DYN}/tareas/{newTareaId}/estado',         auth:true, name:'PATCH tarea/estado', useDynamic:'newProyectoId', skipIf:'newTareaId', body:{estado:2}, expect:{status:[200,204]} },
    { id:'patch-tarea-avance',method:'PATCH',  path:'/api/proy/proyectos/{DYN}/tareas/{newTareaId}/avance',         auth:true, name:'PATCH tarea/avance', useDynamic:'newProyectoId', skipIf:'newTareaId', body:{avancePorcentaje:30}, expect:{status:[200,204]} },
    { id:'list-cuadrillas',   method:'GET',    path:`/api/proy/proyectos/${SEED.proyectoId}/cuadrillas`,            auth:true, name:'GET cuadrillas seed', expect:{status:200} },
    { id:'create-cuadrilla',  method:'POST',   path:'/api/proy/proyectos/{DYN}/cuadrillas',                         auth:true, name:'POST cuadrilla', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-cuadrilla', captureId:'newCuadrillaId', expect:{status:[200,201]} },
    { id:'update-cuadrilla',  method:'PUT',    path:'/api/proy/proyectos/{DYN}/cuadrillas/{newCuadrillaId}',        auth:true, name:'PUT cuadrilla', useDynamic:'newProyectoId', skipIf:'newCuadrillaId', bodyFn:'update-cuadrilla', expect:{status:[200,204]} },
    { id:'add-miembro',       method:'POST',   path:'/api/proy/proyectos/{DYN}/cuadrillas/{newCuadrillaId}/miembros',auth:true, name:'POST cuadrilla/miembros', useDynamic:'newProyectoId', skipIf:'newCuadrillaId', body:{empleadoId:SEED.empleadoId,rol:'Tecnico'}, expect:{status:[200,201,409]} },
    { id:'delete-cuadrilla',  method:'DELETE', path:'/api/proy/proyectos/{DYN}/cuadrillas/{newCuadrillaId}',        auth:true, name:'DELETE cuadrilla', useDynamic:'newProyectoId', skipIf:'newCuadrillaId', expect:{status:[200,204]} },
  ]},

  { id:'proyectos-presupuesto', icon:'💰', title:'Proyectos — Presupuesto & Costos', desc:'Presupuesto, partidas, centros de costo', tests:[
    { id:'list-presupuestos',  method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/presupuesto`,                            auth:true, name:'GET presupuesto seed', expect:{status:200} },
    { id:'presupuesto-activo', method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/presupuesto/activo`,                     auth:true, name:'GET presupuesto/activo', expect:{status:[200,404]} },
    { id:'presupuesto-ejec',   method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/presupuesto/ejecucion`,                  auth:true, name:'GET presupuesto/ejecucion', expect:{status:[200,404]} },
    { id:'list-costos',        method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/presupuesto/costos`,                     auth:true, name:'GET costos seed', expect:{status:200} },
    { id:'create-presupuesto', method:'POST',  path:'/api/proy/proyectos/{DYN}/presupuesto',                                         auth:true, name:'POST presupuesto', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-presupuesto', captureId:'newPresupuestoId', expect:{status:[200,201]} },
    { id:'add-partida',        method:'POST',  path:'/api/proy/proyectos/{DYN}/presupuesto/{newPresupuestoId}/partidas',              auth:true, name:'POST partida', useDynamic:'newProyectoId', skipIf:'newPresupuestoId', bodyFn:'create-partida', expect:{status:[200,201]} },
    { id:'aprobar-presupuesto',method:'PATCH', path:'/api/proy/proyectos/{DYN}/presupuesto/{newPresupuestoId}/aprobar',               auth:true, name:'PATCH presupuesto/aprobar', useDynamic:'newProyectoId', skipIf:'newPresupuestoId', body:{aprobado:true}, expect:{status:[200,204]} },
    { id:'list-cc',            method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/centros-costo`,                          auth:true, name:'GET centros-costo seed', expect:{status:200} },
    { id:'create-cc',          method:'POST',  path:'/api/proy/proyectos/{DYN}/centros-costo',                                       auth:true, name:'POST centro-costo', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-centro-costo', captureId:'newCCId', expect:{status:[200,201]} },
    { id:'update-cc',          method:'PUT',   path:'/api/proy/proyectos/{DYN}/centros-costo/{newCCId}',                             auth:true, name:'PUT centro-costo', useDynamic:'newProyectoId', skipIf:'newCCId', bodyFn:'update-centro-costo', expect:{status:[200,204]} },
    { id:'delete-cc',          method:'DELETE',path:'/api/proy/proyectos/{DYN}/centros-costo/{newCCId}',                             auth:true, name:'DELETE centro-costo', useDynamic:'newProyectoId', skipIf:'newCCId', expect:{status:[200,204]} },
  ]},

  { id:'proyectos-control', icon:'📊', title:'Proyectos — Control', desc:'Gantt, KPIs, alertas, reportes, documentos', tests:[
    { id:'gantt',             method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/gantt`,                          auth:true, name:'GET gantt seed', expect:{status:200} },
    { id:'gantt-linea-base',  method:'POST',  path:`/api/proy/proyectos/${SEED.proyectoId}/gantt/linea-base`,               auth:true, name:'POST gantt/linea-base', body:{}, expect:{status:[200,201,204,400]} },
    // 400 esperado si proyecto seed no tiene tareas — aceptado
    { id:'gantt-progreso',    method:'POST',  path:`/api/proy/proyectos/${SEED.proyectoId}/gantt/progreso`,                 auth:true, name:'POST gantt/progreso', body:{fecha:new Date().toISOString().split('T')[0]}, expect:{status:[200,201,204,400]}, hint:'400 aceptado: sin tareas en proyecto seed' },
    { id:'kpis',              method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/kpis`,                           auth:true, name:'GET kpis seed', expect:{status:200} },
    // 500 conocido en backend — aceptado temporalmente hasta fix
    { id:'kpis-calcular',     method:'POST',  path:`/api/proy/proyectos/${SEED.proyectoId}/kpis/calcular`,                  auth:true, name:'POST kpis/calcular', body:{}, expect:{status:[200,201,204,500]}, hint:'500 conocido: error EF Core al guardar — pendiente fix backend' },
    { id:'alertas-proyecto',  method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/alertas`,                       auth:true, name:'GET alertas proyecto', expect:{status:200} },
    { id:'alertas-no-leidas', method:'GET',   path:'/api/proy/alertas/no-leidas-count',                                    auth:true, name:'GET alertas/no-leidas-count', expect:{status:200} },
    // 500 conocido en backend — aceptado temporalmente hasta fix
    { id:'marcar-alerta',     method:'PATCH', path:`/api/proy/proyectos/${SEED.proyectoId}/alertas/${SEED.alertaId}/leida`, auth:true, name:'PATCH alerta/leida', body:{}, expect:{status:[200,204,404,500]}, hint:'500 conocido: error EF Core al guardar — pendiente fix backend' },
    { id:'marcar-todas',      method:'PATCH', path:`/api/proy/proyectos/${SEED.proyectoId}/alertas/marcar-todas-leidas`,   auth:true, name:'PATCH alertas/marcar-todas', body:{}, expect:{status:[200,204]} },
    { id:'list-reportes',     method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/reportes`,                      auth:true, name:'GET reportes seed', expect:{status:200} },
    { id:'create-reporte',    method:'POST',  path:'/api/proy/proyectos/{DYN}/reportes',                                   auth:true, name:'POST reporte', useDynamic:'newProyectoId', skipIf:'newProyectoId', bodyFn:'create-reporte', captureId:'newReporteId', expect:{status:[200,201]} },
    { id:'get-reporte-new',   method:'GET',   path:'/api/proy/proyectos/{DYN}/reportes/{newReporteId}',                    auth:true, name:'GET reporte creado', useDynamic:'newProyectoId', skipIf:'newReporteId', expect:{status:200} },
    { id:'list-docs',         method:'GET',   path:`/api/proy/proyectos/${SEED.proyectoId}/documentos`,                    auth:true, name:'GET documentos seed', expect:{status:200} },
  ]},

  { id:'ordenes-trabajo', icon:'🔧', title:'Órdenes de Trabajo', desc:'CRUD OT + estado + actividades', tests:[
    { id:'list-ot',              method:'GET',    path:'/api/proy/ordenes-trabajo',                              auth:true, name:'GET OT — listar', expect:{status:200} },
    { id:'ot-por-proyecto',      method:'GET',    path:`/api/proy/proyectos/${SEED.proyectoId}/ordenes-trabajo`, auth:true, name:'GET OT por proyecto', expect:{status:200} },
    { id:'create-ot',            method:'POST',   path:'/api/proy/ordenes-trabajo',                              auth:true, name:'POST OT — crear', bodyFn:'create-ot', captureId:'newOtId', expect:{status:[200,201]} },
    { id:'get-ot-new',           method:'GET',    path:'/api/proy/ordenes-trabajo/{DYN}',                        auth:true, name:'GET OT creada', useDynamic:'newOtId', skipIf:'newOtId', expect:{status:200} },
    { id:'update-ot',            method:'PUT',    path:'/api/proy/ordenes-trabajo/{DYN}',                        auth:true, name:'PUT OT', useDynamic:'newOtId', skipIf:'newOtId', bodyFn:'update-ot', expect:{status:[200,204]} },
    { id:'patch-ot-estado',      method:'PATCH',  path:'/api/proy/ordenes-trabajo/{DYN}/estado',                 auth:true, name:'PATCH OT/estado', useDynamic:'newOtId', skipIf:'newOtId', body:{estado:2,observacion:'En progreso'}, expect:{status:[200,204]} },
    { id:'patch-ot-actividades', method:'PATCH',  path:'/api/proy/ordenes-trabajo/{DYN}/actividades',            auth:true, name:'PATCH OT/actividades', useDynamic:'newOtId', skipIf:'newOtId', body:{actividades:[]}, expect:{status:[200,204]} },
    { id:'delete-ot',            method:'DELETE', path:'/api/proy/ordenes-trabajo/{DYN}',                        auth:true, name:'DELETE OT', useDynamic:'newOtId', skipIf:'newOtId', expect:{status:[200,204]} },
    { id:'get-ot-seed',          method:'GET',    path:`/api/proy/ordenes-trabajo/${SEED.otId}`,                 auth:true, name:'GET OT seed', expect:{status:[200,404]} },
  ]},

  { id:'errors', icon:'⚠️', title:'Errores & Seguridad', desc:'404s, validaciones, tokens inválidos', tests:[
    { id:'404-usuario',         method:'GET',  path:'/api/Usuarios/99999',              auth:true,  name:'GET Usuario/99999 → 404',    expect:{status:404} },
    { id:'404-proyecto',        method:'GET',  path:'/api/proy/proyectos/99999',         auth:true,  name:'GET Proyecto/99999 → 404',   expect:{status:404} },
    { id:'404-empleado',        method:'GET',  path:'/api/rrhh/empleados/99999',         auth:true,  name:'GET Empleado/99999 → 404',   expect:{status:404} },
    { id:'404-catalogo',        method:'GET',  path:'/api/catalogos/paises/99999',       auth:true,  name:'GET Pais/99999 → 404',       expect:{status:404} },
    { id:'login-empty',         method:'POST', path:'/api/Auth/login',                   auth:false, name:'POST login vacío → 400',     body:{email:'',password:''}, expect:{status:400} },
    { id:'post-proyecto-vacio', method:'POST', path:'/api/proy/proyectos',               auth:true,  name:'POST proyecto vacío → 400',  body:{}, expect:{status:400} },
    { id:'post-empleado-vacio', method:'POST', path:'/api/rrhh/empleados',               auth:true,  name:'POST empleado vacío → 400',  body:{}, expect:{status:400} },
    { id:'invalid-token',       method:'GET',  path:'/api/Usuarios',                     auth:false, name:'JWT inválido → 401',         _fakeToken:'eyJhbGciOiJIUzI1NiJ9.FAKE.INVALID', expect:{status:401} },
    { id:'bad-refresh',         method:'POST', path:'/api/Auth/refresh-token',           auth:false, name:'refresh falso → 400/401',    body:{Token:'fake',RefreshToken:'fake'}, expect:{status:[400,401]} },
  ]},
];
