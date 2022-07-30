using Kraken.Core.Contexts;
using Kraken.Core.EventBus;
using Kraken.Core.Outbox;
using Kraken.Core.UnitWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
    /// Orquestador de los eventos
    /// </summary>
    private readonly IEventDispatcher _eventOrchestrator;

    /// <summary>
    /// Proveedor de los servicios con alcance de la aplicacion
    /// para resolver las dependencias necesarias
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public UnitWorkEventsHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        // Obtenemos el accesor del contexto
        var contextAccessor = _serviceProvider.GetRequiredService<OutboxContextAccesor>();
        // Si es nulo, entonces no esta habilitado y salimos
        if (contextAccessor is null)
            return Task.CompletedTask;
        // Creamos el contexto
        contextAccessor.Context = new DefaultOutboxContext(notification.Id);
        //_outboxContextAccesor.Context = new DefaultOutboxContext(notification.Id);
        // Salimos
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
        // Obtenemos el contexto
        var contextAccessor = _serviceProvider.GetRequiredService<OutboxContextAccesor>();
        // Obtenemos el despachador de eventos
        var eventDispatcher = _serviceProvider.GetRequiredService<IEventDispatcher>();
        // Si alguno de los dos es nulo, entonces salimos
        if (contextAccessor is null || eventDispatcher is null)
            return Task.CompletedTask;
        // Obtenemos los eventos
        var events = contextAccessor.Context.Events.ToList();
        // Los agregamos al orquestador
        foreach (var evt in events)
            eventDispatcher.EnqueueToExecute(evt);
        // Limpiamos el contexto
        contextAccessor.Context.Cleanup();
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
        // Obtenemos el accesor del contexto
        var contextAccessor = _serviceProvider.GetRequiredService<OutboxContextAccesor>();
        // Si es nulo, entonces no esta habilitado y salimos
        if (contextAccessor is null)
            return Task.CompletedTask;
        // Limpiamos el contexto
        contextAccessor.Context.Cleanup();
        //_outboxContextAccesor.Context.Cleanup();
        // Salimos
        return Task.CompletedTask;
    }
}
