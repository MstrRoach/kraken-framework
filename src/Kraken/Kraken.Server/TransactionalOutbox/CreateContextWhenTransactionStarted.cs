using Kraken.Module.Transactions;
using Kraken.Server.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

/// <summary>
/// Crea el contexto de bandeja de salida cuando se recibe un evento indicando que una nueva
/// transaccion se ha iniciado
/// </summary>
internal class CreateContextWhenTransactionStarted : IArchEventHandler<TransactionStarted>
{
    /// <summary>
    /// Proveedor del contexto de bandeja de salida
    /// </summary>
    private readonly ContextProvider _outboxContextProvider;

    public CreateContextWhenTransactionStarted(ContextProvider outboxContextProvider)
    {
        _outboxContextProvider = outboxContextProvider;
    }

    public Task Handle(TransactionStarted notification, CancellationToken cancellationToken)
    {
        _outboxContextProvider.Context = new OutboxContext()
        {
            TransactionId = notification.TransactionId,
            Module = notification.Module
        };
        return Task.CompletedTask;
    }
}
