using SolarisPlatform.Domain.Common;
using SolarisPlatform.Domain.Enums.Proyectos;

namespace SolarisPlatform.Domain.Entities.Proyectos;

// ──────────────────────────────────────────────────────────────────
// SUBGRUPO 9: KPIs Y ALERTAS
// Tablas: kpi_proyecto, alerta_proyecto
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// KPI calculado del proyecto. Snapshot de métricas Earned Value Management
/// calculadas en un momento específico.
/// </summary>
public class KpiProyecto : BaseEntity
{
    public long      EmpresaId        { get; set; }
    public long      ProyectoId       { get; set; }
    public TipoKpi   TipoKpi          { get; set; }
    public string    Nombre           { get; set; } = null!;
    public decimal   Valor            { get; set; }
    public decimal?  ValorMeta        { get; set; }   // Valor objetivo/umbral
    public decimal?  ValorMinimo      { get; set; }   // Umbral de alerta
    public string?   Unidad           { get; set; }   // Ej: "%", "índice", "$"
    public DateTime  FechaCalculo     { get; set; } = DateTime.UtcNow;
    public string?   Formula          { get; set; }   // Descripción o expresión de la fórmula
    public string?   Observaciones    { get; set; }

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto Proyecto  { get; set; } = null!;
}

/// <summary>
/// Alerta generada por el sistema para el proyecto.
/// Puede ser por retraso de fechas, sobrecosto, tareas vencidas, etc.
/// </summary>
public class AlertaProyecto : BaseEntity
{
    public long            EmpresaId    { get; set; }
    public long            ProyectoId   { get; set; }
    public TipoAlerta      TipoAlerta   { get; set; }
    public SeveridadAlerta Severidad    { get; set; } = SeveridadAlerta.Warning;
    public string          Titulo       { get; set; } = null!;
    public string          Mensaje      { get; set; } = null!;
    public bool            Leida        { get; set; } = false;
    public bool            Resuelta     { get; set; } = false;
    public DateTime        FechaAlerta  { get; set; } = DateTime.UtcNow;
    public DateTime?       FechaLeida   { get; set; }
    public DateTime?       FechaResuelta { get; set; }
    public long?           UsuarioId    { get; set; }   // Destinatario de la alerta (FK seg.usuario)
    public string?         UrlReferencia { get; set; }  // Deep-link al elemento en problema

    // ── Navegación ──────────────────────────────────────────────────
    public virtual Proyecto Proyecto   { get; set; } = null!;
}
