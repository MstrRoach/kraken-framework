namespace Kraken.Core.UnitWork;

/// <summary>
/// Indica que una transaccion fallo y que se revirtio
/// </summary>
public record TransacctionFailed : UnitWorkEventBase
{
    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; set; }
}
