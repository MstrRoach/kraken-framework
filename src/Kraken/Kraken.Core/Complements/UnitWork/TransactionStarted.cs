namespace Kraken.Core.UnitWork;

/// <summary>
/// Evento para indicar que una transaccion ha sido iniciada
/// </summary>
public record TransactionStarted : UnitWorkEventBase
{
    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; set; }
}
