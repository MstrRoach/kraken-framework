using Kraken.Module.Transactions;
using Kraken.Server.Outbox;
using Kraken.Server.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.OutboxOld;

internal class CreateContextWhenTransactionStarted : IArchEventHandler<TransactionStarted>
{
    /// <summary>
    /// Proveedor del contexto de bandeja de salida
    /// </summary>
    private readonly OutboxContextProvider _outboxContextProvider;

    public CreateContextWhenTransactionStarted(OutboxContextProvider outboxContextProvider)
    {
        _outboxContextProvider = outboxContextProvider;
    }

    public Task Handle(TransactionStarted notification, CancellationToken cancellationToken)
    {
        _outboxContextProvider.Context = new DefaultOutboxContext(notification.Id);
        return Task.CompletedTask;
    }
}
