using Kraken.Core.Contexts;
using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
using Kraken.Core.UnitWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox;

/// <summary>
/// Administra todos los eventos que el componente de la unidad de trabajo publica
/// </summary>
internal class UnitWorkEventsHandler : IComponentEventHandler<TransactionStarted>,
    IComponentEventHandler<TransactionCommited>,
    IComponentEventHandler<TransacctionFailed>
{
    /// <summary>
    /// Accesor al contexto de bandeja de salida
    /// </summary>
    private readonly OutboxContextAccesor _outboxContextAccesor;

    /// <summary>
    /// Acceso al contexto de solicitud actual
    /// </summary>
    private readonly IContext _context;

    /// <summary>
    /// Orquestador de los eventos
    /// </summary>
    private readonly IEventDispatcher _eventOrchestrator;

    public UnitWorkEventsHandler(OutboxContextAccesor outboxContextAccesor, IContext context,
        IEventDispatcher eventOrchestrator)
    {
        _outboxContextAccesor = outboxContextAccesor;
        _context = context;
        _eventOrchestrator = eventOrchestrator;
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
    public Task Handle(TransactionCommited notification, CancellationToken cancellationToken)
    {
        // Obtenemos los eventos
        var events = _outboxContextAccesor.Context.Events.ToList();
        // Los agregamos al orquestador
        foreach (var evt in events)
            _eventOrchestrator.EnqueueToExecute(evt);
        // Limpiamos el contexto
        _outboxContextAccesor.Context.Cleanup();
        // Salimos
        return Task.CompletedTask;
    }

    /// <summary>
    /// Cuando la transaccion falle, debemos de eliminar el contexto, para no ejecutar los
    /// eventos resultantes
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task Handle(TransacctionFailed notification, CancellationToken cancellationToken)
    {
        _outboxContextAccesor.Context.Cleanup();
        return Task.CompletedTask;
    }
}
