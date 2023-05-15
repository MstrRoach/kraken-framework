using Kraken.Module.Transactions;
using Kraken.Server.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

internal class ConfirmEventsWhenTransactionCommited : IArchEventHandler<TransactionCommited>
{

    /// <summary>
    /// Componente que coordina las operaciones de la
    /// bandeja de salida
    /// </summary>
    private readonly Outbox _outbox;

    public ConfirmEventsWhenTransactionCommited(Outbox outbox)
    {
        _outbox = outbox;
    }

    public async Task Handle(TransactionCommited notification, CancellationToken cancellationToken)
    {
        await _outbox.Publish(notification.TransactionId);
    }
}
