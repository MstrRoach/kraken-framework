using Kraken.Server.Request;
using Kraken.Module.Outbox;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kraken.Module.Transactions;

namespace Kraken.Server.OutboxOld;

internal class CleanContextWhenTransactionFailed : IArchEventHandler<TransacctionFailed>
{
    /// <summary>
    /// Accesor al contexto de la bandeja de salidda
    /// </summary>
    private readonly OutboxContextProvider _outboxContextProvider;

    public CleanContextWhenTransactionFailed(OutboxContextProvider outboxContextProvider)
    {
        _outboxContextProvider = outboxContextProvider;
    }

    public Task Handle(TransacctionFailed notification, CancellationToken cancellationToken)
    {
        // Limpiamos el contexto
        _outboxContextProvider.Context.Cleanup();
        // Salimos
        return Task.CompletedTask;
    }
}
