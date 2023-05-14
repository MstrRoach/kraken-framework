using Kraken.Module.Transactions;
using Kraken.Server.Request;

namespace Kraken.Server.Outbox;

internal class EnqueueEventsWhenTransactionCommited : IArchEventHandler<TransactionCommited>
{
    /// <summary>
    /// Proveedor del contexto de bandeja de salida
    /// </summary>
    private readonly OutboxContextProvider _outboxContextProvider;

    /// <summary>
    /// Broker encargado de distribuir los eventos
    /// </summary>
    private readonly DefaultOutboxBroker _outboxBroker;

    public EnqueueEventsWhenTransactionCommited(OutboxContextProvider outboxContextProvider, DefaultOutboxBroker outboxBroker)
    {
        _outboxContextProvider = outboxContextProvider;
        _outboxBroker = outboxBroker;
    }

    public Task Handle(TransactionCommited notification, CancellationToken cancellationToken)
    {
        // Obtenemos los eventos
        var events = _outboxContextProvider.Context.Events;
        // Los agregamos al despachador de eventos
        foreach (var evt in events)
            _outboxBroker.EnqueueToExecute(evt);
        // Limpiamos el contexto
        _outboxContextProvider.Context.Cleanup();
        // Salimos
        return Task.CompletedTask;
    }
}
