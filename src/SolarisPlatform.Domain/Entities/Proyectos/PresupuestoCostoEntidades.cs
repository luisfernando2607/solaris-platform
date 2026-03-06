using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 4: PRESUPUESTO Y COSTOS
// Tablas: presupuesto, presupuesto_partida, costo_real
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Presupuesto del proyecto.
/// BD real: proy.presupuesto
/// Columnas: id, empresa_id, proyecto_id, version, descripcion, estado,
///           fecha_aprobacion, aprobado_por_id,
///           monto_total_mano_obra, monto_total_materiales,
///           monto_total_subcontratos, monto_total_equipos,
///           monto_total_indirectos, total_general, observaciones
///
/// FIX: La entidad anterior tenía Nombre, TotalIngresos, TotalEgresos,
///      Contingencia, TotalNeto, EsActivo, EsAprobado — NINGUNA existe en la BD.
///      Se reemplaza por la estructura real.
/// </summary>
public class Presupuesto : BaseEntity
{
    public long      EmpresaId                { get; set; }
    public long      ProyectoId               { get; set; }
    public short     Version                  { get; set; } = 1;
    public string?   Descripcion              { get; set; }
    public short     Estado                   { get; set; } = 1;   // 1=Borrador, 2=Aprobado, etc.
    public DateOnly? FechaAprobacion          { get; set; }
    public long?     AprobadoPorId            { get; set; }

    // Montos por categoría
    public decimal   MontoTotalManoObra       { get; set; }
    public decimal   MontoTotalMateriales     { get; set; }
    public decimal   MontoTotalSubcontratos   { get; set; }
    public decimal   MontoTotalEquipos        { get; set; }
    public decimal   MontoTotalIndirectos     { get; set; }
    public decimal   TotalGeneral             { get; set; }
    public string?   Observaciones            { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto                        Proyecto { get; set; } = null!;
    public virtual ICollection<PresupuestoPartida> Partidas { get; set; } = new List<PresupuestoPartida>();
}

/// <summary>
/// Partida del presupuesto.
/// BD: proy.presupuesto_partida
/// La BD sí tiene las columnas de la entidad original — sin cambios estructurales.
/// </summary>
public class PresupuestoPartida : BaseEntity
{
    public long          EmpresaId      { get; set; }
    public long          PresupuestoId  { get; set; }
    public TipoPartida   Tipo           { get; set; } = TipoPartida.Egreso;
    public string        Concepto       { get; set; } = null!;
    public string?       Descripcion    { get; set; }
    public string?       CodigoContable { get; set; }
    public decimal       Cantidad       { get; set; } = 1;
    public string?       UnidadMedida   { get; set; }
    public decimal       PrecioUnitario { get; set; }
    public decimal       Subtotal       { get; set; }
    public decimal       Porcentaje     { get; set; }
    public decimal       Total          { get; set; }
    public int           Orden          { get; set; } = 1;

    public virtual Presupuesto                 Presupuesto  { get; set; } = null!;
    public virtual ICollection<CostoReal>      CostosReales { get; set; } = new List<CostoReal>();
}

/// <summary>
/// Costo real incurrido.
/// BD: proy.costo_real
/// FIX: FechaRegistro → Fecha (columna "fecha"), sin NumeroReferencia ni Descripcion en BD.
///      BD tiene: origen_id (no en entidad original), observaciones (no en entidad original).
/// </summary>
public class CostoReal : BaseEntity
{
    public long         EmpresaId        { get; set; }
    public long         PresupuestoId    { get; set; }
    public long         PartidaId        { get; set; }
    public long?        OrdenTrabajoId   { get; set; }
    public OrigenCosto  Origen           { get; set; } = OrigenCosto.Manual;
    // FIX: BD tiene origen_id (referencia al objeto origen, ej. id de factura)
    public long?        OrigenId         { get; set; }
    public string       Concepto         { get; set; } = null!;
    public decimal      Monto            { get; set; }
    // FIX: FechaRegistro → Fecha (columna "fecha" en BD)
    public DateOnly     Fecha            { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public long?        RegistradoPorId  { get; set; }
    public string?      Observaciones    { get; set; }

    public virtual Presupuesto        Presupuesto  { get; set; } = null!;
    public virtual PresupuestoPartida Partida      { get; set; } = null!;
    public virtual OrdenTrabajo?      OrdenTrabajo { get; set; }
}
