using Dottex.Kalypso.Server.Request;
using Dottex.Kalypso.Module.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

/// <summary>
/// Cuando la transaccion falla, debe limpiarse el contexto pues no
/// sera utilizaado
/// </summary>
internal class CleanContextWhenTransactionFailed : IArchEventHandler<TransacctionFailed>
{
    /// <summary>
    /// Proveedor de contexto de bandeja de salida
    /// </summary>
    private readonly Outbox _outbox;

    public CleanContextWhenTransactionFailed(Outbox outbox)
    {
        _outbox = outbox;
    }

    public async Task Handle(TransacctionFailed notification, CancellationToken cancellationToken)
    {
        // Cancelamos los eventos de la transaccion
        await _outbox.Cleanup(notification.TransactionId);
    }
}
