using Kraken.Server.Request;
using Kraken.Standard.Outbox;
using Kraken.Standard.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Outbox;

/// <summary>
/// Contiene los handlers para reaccionar a los eventos
/// lanzados por la unidad de trabajo en turno
/// </summary>
internal class UnitWorkEventHandlers : IArchEventHandler<TransactionStarted>,
    IArchEventHandler<TransactionCommited>,
    IArchEventHandler<TransacctionFailed>
{
    private readonly OutboxContextProvider _outboxContextProvider;

    public UnitWorkEventHandlers(OutboxContextProvider outboxContextProvider)
    {
        _outboxContextProvider = outboxContextProvider;
    }

    /// <summary>
    /// Cuando la transaccion inicie, debemmos de crear el contexto 
    /// de la bandeja de salida
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task Handle(TransactionStarted notification, CancellationToken cancellationToken)
    {
        _outboxContextProvider.Context = new DefaultOutboxContext(notification.Id);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cuando la transaccion finalice, debemos de publicar los eventos en el canal
    /// de eventos para su procesamiento
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task Handle(TransactionCommited notification, CancellationToken cancellationToken)
    {
        // Obtenemos el despachador de eventos
        // Si no hay despachador entonces salimos
        // Obtenemos los eventos
        var events = _outboxContextProvider.Context.Events;
        // Los agregamos al despachador de eventos
        // Limpiamos el contexto
        _outboxContextProvider.Context.Cleanup();
        // Salimos
        return Task.CompletedTask;
    }

    public Task Handle(TransacctionFailed notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
