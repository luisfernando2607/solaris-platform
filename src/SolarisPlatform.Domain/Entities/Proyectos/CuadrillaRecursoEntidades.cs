using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 3: CUADRILLAS Y RECURSOS
// Tablas: cuadrilla, cuadrilla_miembro, recurso_proyecto
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Cuadrilla de trabajo asignada a un proyecto.
/// Agrupa empleados para ejecutar tareas de campo.
/// </summary>
public class Cuadrilla : BaseEntity
{
    public long   EmpresaId      { get; set; }
    public long   ProyectoId     { get; set; }
    public string Nombre         { get; set; } = null!;
    public string? Descripcion   { get; set; }
    public long?  LiderId        { get; set; }   // FK rrhh.empleado - líder de cuadrilla
    public int    CapacidadMax   { get; set; } = 10;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto                     Proyecto { get; set; } = null!;
    public virtual ICollection<CuadrillaMiembro> Miembros { get; set; } = new List<CuadrillaMiembro>();
    public virtual ICollection<Tarea>           Tareas   { get; set; } = new List<Tarea>();
}

/// <summary>
/// Miembro de una cuadrilla. Relaciona empleados con cuadrillas.
/// </summary>
public class CuadrillaMiembro : BaseEntity
{
    public long     EmpresaId     { get; set; }
    public long     CuadrillaId   { get; set; }
    public long     EmpleadoId    { get; set; }   // FK rrhh.empleado
    public DateOnly FechaIngreso  { get; set; }
    public DateOnly? FechaSalida  { get; set; }
    public string?  Rol           { get; set; }   // Ej: "Técnico", "Ayudante", "Supervisor"

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Cuadrilla Cuadrilla { get; set; } = null!;
}

/// <summary>
/// Recurso asignado al proyecto (humano, material, equipo, servicio).
/// Representa el plan de recursos y su consumo real.
/// </summary>
public class RecursoProyecto : BaseEntity
{
    public long          EmpresaId         { get; set; }
    public long          ProyectoId        { get; set; }
    public long?         TareaId           { get; set; }
    public TipoRecurso   TipoRecurso       { get; set; } = TipoRecurso.Humano;
    public string        Nombre            { get; set; } = null!;
    public string?       Codigo            { get; set; }
    public UnidadRecurso UnidadMedida      { get; set; } = UnidadRecurso.Unidad;
    public decimal       CantidadPlan      { get; set; }
    public decimal       CantidadReal      { get; set; }
    public decimal       CostoUnitario     { get; set; }
    public decimal       CostoTotalPlan    { get; set; }
    public decimal       CostoTotalReal    { get; set; }
    public long?         EmpleadoId        { get; set; }   // FK rrhh.empleado (si es humano)
    public long?         ProveedorId       { get; set; }   // futuro crm.proveedor

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto Proyecto   { get; set; } = null!;
    public virtual Tarea?   Tarea      { get; set; }
}
