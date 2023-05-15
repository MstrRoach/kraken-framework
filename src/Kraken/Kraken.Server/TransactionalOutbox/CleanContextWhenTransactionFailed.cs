using Kraken.Module.Transactions;
using Kraken.Server.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

/// <summary>
/// Cuando la transaccion falla, debe limpiarse el contexto pues no
/// sera utilizaado
/// </summary>
internal class CleanContextWhenTransactionFailed : IArchEventHandler<TransacctionFailed>
{
    /// <summary>
    /// Proveedor de contexto de bandeja de salida
    /// </summary>
    private readonly ContextProvider _contextProvider;

    public CleanContextWhenTransactionFailed(ContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    public Task Handle(TransacctionFailed notification, CancellationToken cancellationToken)
    {
        // Limpiamos el contexto
        _outboxContextProvider.Context.Cleanup();
        // Salimos
        return Task.CompletedTask;
    }
}
