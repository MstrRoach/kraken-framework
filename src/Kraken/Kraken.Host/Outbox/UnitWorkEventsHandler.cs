using Kraken.Core.Contexts;
using Kraken.Core.Mediator;
using Kraken.Core.UnitWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox;

internal class CreateOutboxContextHandler : INotificationHandler<TransactionStarted>
{
    private readonly OutboxContextAccesor _outboxContextAccesor;
    public CreateOutboxContextHandler(OutboxContextAccesor outboxContextAccesor)
    {
        _outboxContextAccesor = outboxContextAccesor;
    }

    public Task Handle(TransactionStarted notification, CancellationToken cancellationToken)
    {
        _outboxContextAccesor.Context = new DefaultOutboxContext(notification.Id);
        return Task.CompletedTask;
    }
}



/// <summary>
/// Administra todos los eventos que el componente de la unidad de trabajo publica
/// </summary>
internal class UnitWorkEventsHandler 
    //, 
    //IComponentEventHandler<TransactionCommited>, 
    //IComponentEventHandler<TransacctionFailed>
{
    /// <summary>
    /// Accesor al contexto de bandeja de salida
    /// </summary>
    private readonly OutboxContextAccesor _outboxContextAccesor;

    public UnitWorkEventsHandler(OutboxContextAccesor outboxContextAccesor)
    {
        _outboxContextAccesor = outboxContextAccesor;
    }

    /// <summary>
    /// Cuando la transaccion inicie, debemmos de crear el contexto 
    /// de la bandeja de salida
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task Handle(TransactionStarted notification, CancellationToken cancellationToken)
    {
        // Creamos el contexto
        _outboxContextAccesor.Context = new DefaultOutboxContext(notification.Id);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cuando la transaccion finalice, debemos de publicar los eventos en el canal
    /// de eventos para su procesamiento
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task Handle(TransactionCommited notification, CancellationToken cancellationToken)
    {
        var events = _outboxContextAccesor.Context.DomainEvents.ToList();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cuando la transaccion falle, debemos de eliminar el contexto, para no ejecutar los
    /// eventos resultantes
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task Handle(TransacctionFailed notification, CancellationToken cancellationToken)
    {
        _outboxContextAccesor.Context.Cleanup();
        return Task.CompletedTask;
    }
}
