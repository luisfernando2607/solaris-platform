using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 4: PRESUPUESTO Y COSTOS
// Tablas: presupuesto, presupuesto_partida, costo_real
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Presupuesto del proyecto. Un proyecto puede tener múltiples versiones.
/// Solo uno puede ser el activo/aprobado.
/// </summary>
public class Presupuesto : BaseEntity
{
    public long    EmpresaId          { get; set; }
    public long    ProyectoId         { get; set; }
    public int     Version            { get; set; } = 1;
    public string  Nombre             { get; set; } = null!;
    public string? Descripcion        { get; set; }
    public decimal TotalIngresos      { get; set; }
    public decimal TotalEgresos       { get; set; }
    public decimal Contingencia       { get; set; }
    public decimal TotalNeto          { get; set; }

    /// <summary>¿Es esta la versión aprobada/activa?</summary>
    public bool    EsActivo           { get; set; } = true;
    public bool    EsAprobado         { get; set; } = false;
    public long?   AprobadoPorId      { get; set; }   // FK seg.usuario
    public DateTime? FechaAprobacion  { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto                      Proyecto { get; set; } = null!;
    public virtual ICollection<PresupuestoPartida> Partidas { get; set; } = new List<PresupuestoPartida>();
}

/// <summary>
/// Partida del presupuesto. Línea de detalle con tipo ingreso/egreso/contingencia.
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
    public decimal       Porcentaje     { get; set; }   // Ej: 12% IVA, o % de contingencia
    public decimal       Total          { get; set; }
    public int           Orden          { get; set; } = 1;

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Presupuesto                 Presupuesto { get; set; } = null!;
    public virtual ICollection<CostoReal>      CostosReales { get; set; } = new List<CostoReal>();
}

/// <summary>
/// Registro de costo real incurrido. Enlaza el costo con su partida presupuestaria.
/// Puede venir de orden de trabajo, factura, nómina o registro manual.
/// </summary>
public class CostoReal : BaseEntity
{
    public long         EmpresaId      { get; set; }
    public long         PresupuestoId  { get; set; }
    public long         PartidaId      { get; set; }
    public OrigenCosto  Origen         { get; set; } = OrigenCosto.Manual;
    public string       Concepto       { get; set; } = null!;
    public string?      Descripcion    { get; set; }
    public decimal      Monto          { get; set; }
    public DateTime     FechaRegistro  { get; set; } = DateTime.UtcNow;
    public string?      NumeroReferencia { get; set; }   // Nro. factura, OT, planilla, etc.
    public long?        OrdenTrabajoId { get; set; }     // FK si viene de OT
    public long?        RegistradoPorId { get; set; }    // FK seg.usuario

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Presupuesto        Presupuesto   { get; set; } = null!;
    public virtual PresupuestoPartida Partida       { get; set; } = null!;
    public virtual OrdenTrabajo?      OrdenTrabajo  { get; set; }
}
