namespace Kraken.Core.Complements.UnitWork;

/// <summary>
/// Indica que una transaccion fue finalizada
/// correctammente
/// </summary>
public record TransactionCommited : UnitWorkEventBase
{

    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; set; }
}
