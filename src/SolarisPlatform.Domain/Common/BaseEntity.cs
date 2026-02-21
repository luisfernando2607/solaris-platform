namespace SolarisPlatform.Domain.Common;

/// <summary>
/// Entidad base con ID
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
}

/// <summary>
/// Entidad con campos de auditoría
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public long? UsuarioCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public long? UsuarioModificacion { get; set; }
}

/// <summary>
/// Entidad con soft delete
/// </summary>
public abstract class SoftDeletableEntity : AuditableEntity
{
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }
    public long? UsuarioEliminacion { get; set; }
}

/// <summary>
/// Entidad para catálogos
/// </summary>
public abstract class CatalogoEntity : BaseEntity
{
    public bool Activo { get; set; } = true;
    public int Orden { get; set; } = 0;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
