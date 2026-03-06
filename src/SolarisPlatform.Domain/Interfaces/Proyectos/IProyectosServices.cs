// ──────────────────────────────────────────────────────────────────
// NOTA DE ARQUITECTURA:
// Las interfaces de servicios del módulo Proyectos se definen en:
//   Application/Common/Interfaces/Proyectos/IProyectosServices.cs
//
// Este archivo es un placeholder que documenta qué servicios existen.
// Las interfaces reales están en la capa Application (Paso 2).
// ──────────────────────────────────────────────────────────────────
//
// Servicios a implementar en Application/Common/Interfaces/Proyectos/:
//
//   IProyectoService        → CRUD proyectos, cambio de estado, avance
//   IProyectoFaseService    → CRUD fases
//   IProyectoHitoService    → CRUD hitos, marcar logrado
//   IWbsService             → Árbol WBS, CRUD nodos
//   ITareaService           → CRUD tareas, dependencias, avance
//   ICuadrillaService       → CRUD cuadrillas, agregar/remover miembros
//   IPresupuestoService     → Versiones, partidas, aprobación, costos reales
//   IGanttService           → Vista Gantt, línea base, progreso
//   IOrdenTrabajoService    → CRUD OT, estados, firma digital, actividades
//   IReporteAvanceService   → Reportes periódicos, fotos de campo
//   IKpiService             → Cálculo EVM, alertas
//   ICentroCostoService     → CRUD centros de costo, asignaciones
//
