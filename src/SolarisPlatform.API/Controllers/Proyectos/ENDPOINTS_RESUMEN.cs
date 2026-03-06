// ══════════════════════════════════════════════════════════════════
// PROGRAM.CS — Sin cambios requeridos
// Los controllers se registran automáticamente con AddControllers()
// que ya existe en el Program.cs actual.
// ══════════════════════════════════════════════════════════════════

// ══════════════════════════════════════════════════════════════════
// SWAGGER — Anotaciones de grupos para organizar los endpoints
// Agregar en los controllers si deseas agrupar en Swagger UI.
// Ejemplo de uso en cualquier controller:
// [ApiExplorerSettings(GroupName = "Proyectos")]
// ══════════════════════════════════════════════════════════════════

// ══════════════════════════════════════════════════════════════════
// RESUMEN DE TODOS LOS ENDPOINTS — 65 endpoints totales
// ══════════════════════════════════════════════════════════════════

/*
╔══════════════════════════════════════════════════════════════════╗
║  MÓDULO PROYECTOS — ENDPOINTS REST COMPLETOS                     ║
╠══════════════════════════════════════════════════════════════════╣
║  PROYECTOS (8 endpoints)                                         ║
║  GET    /api/proy/proyectos                                      ║
║  GET    /api/proy/proyectos/{id}                                 ║
║  GET    /api/proy/proyectos/{id}/dashboard                       ║
║  POST   /api/proy/proyectos                                      ║
║  PUT    /api/proy/proyectos/{id}                                 ║
║  DELETE /api/proy/proyectos/{id}                                 ║
║  PATCH  /api/proy/proyectos/{id}/estado                          ║
║  PATCH  /api/proy/proyectos/{id}/avance                          ║
╠══════════════════════════════════════════════════════════════════╣
║  FASES (5 endpoints)                                             ║
║  GET    /api/proy/proyectos/{pid}/fases                          ║
║  GET    /api/proy/proyectos/{pid}/fases/{id}                     ║
║  POST   /api/proy/proyectos/{pid}/fases                          ║
║  PUT    /api/proy/proyectos/{pid}/fases/{id}                     ║
║  DELETE /api/proy/proyectos/{pid}/fases/{id}                     ║
╠══════════════════════════════════════════════════════════════════╣
║  HITOS (6 endpoints)                                             ║
║  GET    /api/proy/proyectos/{pid}/hitos                          ║
║  GET    /api/proy/proyectos/{pid}/hitos/{id}                     ║
║  POST   /api/proy/proyectos/{pid}/hitos                          ║
║  PUT    /api/proy/proyectos/{pid}/hitos/{id}                     ║
║  DELETE /api/proy/proyectos/{pid}/hitos/{id}                     ║
║  PATCH  /api/proy/proyectos/{pid}/hitos/{id}/logrado             ║
╠══════════════════════════════════════════════════════════════════╣
║  DOCUMENTOS (3 endpoints)                                        ║
║  GET    /api/proy/proyectos/{pid}/documentos                     ║
║  POST   /api/proy/proyectos/{pid}/documentos                     ║
║  DELETE /api/proy/proyectos/{pid}/documentos/{id}                ║
╠══════════════════════════════════════════════════════════════════╣
║  WBS (5 endpoints)                                               ║
║  GET    /api/proy/proyectos/{pid}/wbs                            ║
║  GET    /api/proy/proyectos/{pid}/wbs/{id}                       ║
║  POST   /api/proy/proyectos/{pid}/wbs                            ║
║  PUT    /api/proy/proyectos/{pid}/wbs/{id}                       ║
║  DELETE /api/proy/proyectos/{pid}/wbs/{id}                       ║
╠══════════════════════════════════════════════════════════════════╣
║  TAREAS (9 endpoints)                                            ║
║  GET    /api/proy/proyectos/{pid}/tareas                         ║
║  GET    /api/proy/proyectos/{pid}/tareas/por-fase/{fid}          ║
║  GET    /api/proy/proyectos/{pid}/tareas/{id}                    ║
║  POST   /api/proy/proyectos/{pid}/tareas                         ║
║  PUT    /api/proy/proyectos/{pid}/tareas/{id}                    ║
║  DELETE /api/proy/proyectos/{pid}/tareas/{id}                    ║
║  PATCH  /api/proy/proyectos/{pid}/tareas/{id}/estado             ║
║  PATCH  /api/proy/proyectos/{pid}/tareas/{id}/avance             ║
║  POST   /api/proy/proyectos/{pid}/tareas/{id}/dependencias       ║
║  DELETE /api/proy/proyectos/{pid}/tareas/{tid}/dependencias/{id} ║
╠══════════════════════════════════════════════════════════════════╣
║  CUADRILLAS (7 endpoints)                                        ║
║  GET    /api/proy/proyectos/{pid}/cuadrillas                     ║
║  GET    /api/proy/proyectos/{pid}/cuadrillas/{id}                ║
║  POST   /api/proy/proyectos/{pid}/cuadrillas                     ║
║  PUT    /api/proy/proyectos/{pid}/cuadrillas/{id}                ║
║  DELETE /api/proy/proyectos/{pid}/cuadrillas/{id}                ║
║  POST   /api/proy/proyectos/{pid}/cuadrillas/{id}/miembros       ║
║  DELETE /api/proy/proyectos/{pid}/cuadrillas/{id}/miembros       ║
╠══════════════════════════════════════════════════════════════════╣
║  PRESUPUESTO (9 endpoints)                                       ║
║  GET    /api/proy/proyectos/{pid}/presupuesto                    ║
║  GET    /api/proy/proyectos/{pid}/presupuesto/activo             ║
║  GET    /api/proy/proyectos/{pid}/presupuesto/{id}               ║
║  GET    /api/proy/proyectos/{pid}/presupuesto/ejecucion          ║
║  POST   /api/proy/proyectos/{pid}/presupuesto                    ║
║  POST   /api/proy/proyectos/{pid}/presupuesto/{id}/partidas      ║
║  PATCH  /api/proy/proyectos/{pid}/presupuesto/{id}/aprobar       ║
║  POST   /api/proy/proyectos/{pid}/presupuesto/costos             ║
║  GET    /api/proy/proyectos/{pid}/presupuesto/costos             ║
╠══════════════════════════════════════════════════════════════════╣
║  GANTT (3 endpoints)                                             ║
║  GET    /api/proy/proyectos/{pid}/gantt                          ║
║  POST   /api/proy/proyectos/{pid}/gantt/linea-base               ║
║  POST   /api/proy/proyectos/{pid}/gantt/progreso                 ║
╠══════════════════════════════════════════════════════════════════╣
║  CENTROS DE COSTO (6 endpoints)                                  ║
║  GET    /api/proy/proyectos/{pid}/centros-costo                  ║
║  GET    /api/proy/proyectos/{pid}/centros-costo/{id}             ║
║  POST   /api/proy/proyectos/{pid}/centros-costo                  ║
║  PUT    /api/proy/proyectos/{pid}/centros-costo/{id}             ║
║  DELETE /api/proy/proyectos/{pid}/centros-costo/{id}             ║
║  POST   /api/proy/proyectos/{pid}/centros-costo/{id}/asignaciones║
╠══════════════════════════════════════════════════════════════════╣
║  ÓRDENES DE TRABAJO (9 endpoints)                                ║
║  GET    /api/proy/ordenes-trabajo                                ║
║  GET    /api/proy/ordenes-trabajo/{id}                           ║
║  POST   /api/proy/ordenes-trabajo                                ║
║  PUT    /api/proy/ordenes-trabajo/{id}                           ║
║  DELETE /api/proy/ordenes-trabajo/{id}                           ║
║  PATCH  /api/proy/ordenes-trabajo/{id}/estado                    ║
║  PATCH  /api/proy/ordenes-trabajo/{id}/actividades               ║
║  PATCH  /api/proy/ordenes-trabajo/{id}/firma                     ║
║  GET    /api/proy/proyectos/{pid}/ordenes-trabajo                ║
╠══════════════════════════════════════════════════════════════════╣
║  REPORTES DE AVANCE (5 endpoints)                                ║
║  GET    /api/proy/proyectos/{pid}/reportes                       ║
║  GET    /api/proy/proyectos/{pid}/reportes/{id}                  ║
║  POST   /api/proy/proyectos/{pid}/reportes                       ║
║  POST   /api/proy/proyectos/{pid}/reportes/{id}/fotos            ║
║  DELETE /api/proy/proyectos/{pid}/reportes/{id}/fotos/{fotoId}   ║
╠══════════════════════════════════════════════════════════════════╣
║  KPIs (2 endpoints)                                              ║
║  GET    /api/proy/proyectos/{pid}/kpis                           ║
║  POST   /api/proy/proyectos/{pid}/kpis/calcular                  ║
╠══════════════════════════════════════════════════════════════════╣
║  ALERTAS (4 endpoints)                                           ║
║  GET    /api/proy/proyectos/{pid}/alertas                        ║
║  PATCH  /api/proy/proyectos/{pid}/alertas/{id}/leida             ║
║  PATCH  /api/proy/proyectos/{pid}/alertas/marcar-todas-leidas    ║
║  GET    /api/proy/alertas/no-leidas-count                        ║
╠══════════════════════════════════════════════════════════════════╣
║  TOTAL: 66 endpoints — 14 controllers                            ║
╚══════════════════════════════════════════════════════════════════╝
*/
