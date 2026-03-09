using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 3: CUADRILLAS Y RECURSOS
// Tablas: cuadrilla, cuadrilla_miembro, recurso_proyecto
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Cuadrilla de trabajo asignada a un proyecto.
/// BD: proy.cuadrilla — tiene columna capacidad_maxima (no capacidad_max)
/// </summary>
public class Cuadrilla : BaseEntity
{
    public long   EmpresaId      { get; set; }
    public long   ProyectoId     { get; set; }
    // FIX: BD también tiene columna "codigo"
    public string  Codigo        { get; set; } = null!;
    public string  Nombre        { get; set; } = null!;
    public string? Descripcion   { get; set; }
    public long?  LiderId        { get; set; }
    // FIX: CapacidadMax → CapacidadMaxima (BD: capacidad_maxima)
    public int    CapacidadMaxima { get; set; } = 10;

    public virtual Proyecto                      Proyecto { get; set; } = null!;
    public virtual ICollection<CuadrillaMiembro> Miembros { get; set; } = new List<CuadrillaMiembro>();
    public virtual ICollection<Tarea>            Tareas   { get; set; } = new List<Tarea>();
}

/// <summary>
/// Miembro de una cuadrilla.
/// BD: proy.cuadrilla_miembro
/// </summary>
public class CuadrillaMiembro : BaseEntity
{
    public long     EmpresaId     { get; set; }
    public long     CuadrillaId   { get; set; }
    public long     EmpleadoId    { get; set; }
    public DateOnly FechaIngreso  { get; set; }
    public DateOnly? FechaSalida  { get; set; }
    public string?  Rol           { get; set; }

    public virtual Cuadrilla Cuadrilla { get; set; } = null!;
}

/// <summary>
/// Recurso asignado al proyecto.
/// BD: proy.recurso_proyecto
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
    public long?         EmpleadoId        { get; set; }
    public long?         ProveedorId       { get; set; }

    public virtual Proyecto Proyecto   { get; set; } = null!;
    public virtual Tarea?   Tarea      { get; set; }
}
