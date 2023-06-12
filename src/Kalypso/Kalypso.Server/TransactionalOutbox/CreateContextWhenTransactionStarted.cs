using Dottex.Kalypso.Server.Request;
using Dottex.Kalypso.Module.Transaction;
using Dottex.Kalypso.Server.TransactionalOutbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

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
