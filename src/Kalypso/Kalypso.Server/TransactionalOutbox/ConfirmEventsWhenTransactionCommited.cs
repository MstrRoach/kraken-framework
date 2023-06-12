using Dottex.Kalypso.Server.Request;
using Dottex.Kalypso.Module.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

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
