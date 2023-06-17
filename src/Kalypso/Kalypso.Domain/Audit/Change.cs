namespace Dottex.Kalypso.Domain.Audit;

/// <summary>
/// Contiene informacion puntual de los cambios
/// </summary>
public record Change
{
    /// <summary>
    /// Id de la entidad
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// Nombre de la entidad que almacena
    /// </summary>
    public string Entity { get; set; }

    /// <summary>
    /// Indica la operacion realizada en la entidad
    /// </summary>
    public AuditOperation Operation { get; set; }

    /// <summary>
    /// Cambios realizados en la entidad
    /// </summary>
    public object Delta { get; set; }

    /// <summary>
    /// Indica el usuario que realizó el cambio
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Fecha en la que se realizo el cambio
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

public enum AuditOperation { Create, Update, Delete }
