using Humanizer;
using Kraken.Core;
using Kraken.Core.Contexts;
using Kraken.Core.Internal.EventBus;
using Kraken.Core.Outbox;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox;

/// <summary>
/// Centralizador de los eventos de dominio. En kraken se definen diferentes tipos de
/// eventos que se derivan de INotification. Uno de ellos, los eventos de dominio estan
/// pensados para la distribucion de los eventos que suceden en el negocio. 
/// - Evento de dominio
///     Estos eventos se escriben en la bandeja de salida del modulo, para tener un registro
///     de lo que ha pasado en la aplicacion. Cuando se agregan al stream del modulo, se agregan
///     tambien en el contexto de la transaccion que permite centralizar todos los eventos por
///     transaccion, cuando la transaccion se completa, entonces estos eventos son lanzados al
///     canal de procesamiento de eventos asincronos
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class OutboxEventBus : IEventBus
    //INotificationHandler<InterceptedEvent>
{
    /// <summary>
    /// Logger del handler
    /// </summary>
    private readonly ILogger<OutboxEventBus> _logger;

    /// <summary>
    /// Contexto de la solicitud
    /// </summary>
    private readonly IContext _context;

    /// <summary>
    /// Accesor para el contexto de la bandeja de salida
    /// </summary>
    private readonly OutboxContextAccesor _outboxContextAccessor;

    /// <summary>
    /// Corredor para todos las bandejas de salida para cada modulo
    /// </summary>
    private readonly IOutboxBroker _outboxBroker;

    /// <summary>
    /// Publicador de eventos planos 
    /// </summary>
    private readonly IPublisher _publisher;

    public OutboxEventBus(ILogger<OutboxEventBus> logger,
        IContext context, OutboxContextAccesor outboxContextAccessor,
        IOutboxBroker outboxBroker, IPublisher publisher, IServiceProvider serviceProvider)
    {
        var id = Thread.CurrentThread.ManagedThreadId;
        _logger = logger;
        _context = context;
        _outboxContextAccessor = outboxContextAccessor;
        _outboxBroker = outboxBroker;
        _publisher = publisher;
    }

    /// <summary>
    /// Se encarga de procesar todos los eventos del modulo a traves de la distincion entre
    /// eventos base de notificacion, eventos de conponente, eventos de modulo y eventos de
    /// dominio, para tratar a cada uno de ellos de manera especial segun la bandeja de salida
    /// transaccional
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task Publish<T>(T notification, CancellationToken cancellationToken = default) where T : INotification
    {
        _logger.LogInformation("[OUTBOX EVENT BUS] Start event processing . . .");
        // Si la notificacion es nula, salimos
        if (notification is null) return;
        // Si no es un evento especial, lo mandamos con el mediador
        if (IsNotDomainOrModuleEvent(notification))
        {
            _logger.LogInformation("[OUTBOX EVENT BUS] Publish notification in system . . .");
            await _publisher.Publish(notification, cancellationToken);
            _logger.LogInformation("[OUTBOX EVENT BUS] Notification dispatching successfully");
            return;
        }
        _logger.LogInformation("[OUTBOX EVENT BUS] Creating message from notification");
        // Convertimos el tipo a un evento base, para obtener la informacion
        var message = notification as IEvent ?? 
            throw new InvalidOperationException("Event can not parsear as IEvent");
        _logger.LogInformation("[OUTBOX EVENT BUS] Get information abount message");
        // Reunimos la informacion del evento
        var module = message.GetModuleName();
        var name = message.GetType().Name.Underscore();
        var requestId = _context.RequestId;
        var traceId = _context.TraceId;
        var userId = _context.Identity?.Id;
        var messageId = message.Id;
        var correlationId = _context.CorrelationId;
        // Logeamos la informacion del evento almacenado
        _logger.LogInformation("Publishing a message: {Name} ({Module}) [Request ID: {RequestId}, Message ID: {MessageId}, Correlation ID: {CorrelationId}, Trace ID: '{TraceId}', User ID: '{UserId}]...",
                name, module, requestId, messageId, correlationId, traceId, userId);
        // Envolvemos el evento en una entidad enriquecida para almacenarlo
        var processMessage = new ProcessMessage
        {
            Id = messageId,
            CorrelationId = correlationId,
            UserId = userId,
            Event = message,
            TraceId = traceId
        };
        _logger.LogInformation("[OUTBOX EVENT BUS] Store message in outbox");
        // Mandamos el evento al corredor de bandeja de salida
        await _outboxBroker.SendAsync(processMessage);
        _logger.LogInformation("[OUTBOX EVENT BUS] Save event in context for later processing");
        // Agregamos el evento al contexto de la bandeja de salida
        _outboxContextAccessor.Context.AddProcessMessage(processMessage);
        _logger.LogInformation("[OUTBOX EVENT BUS] Finish event processing");
    }

    /// <summary>
    /// Revisa si la notificacion contiene ademas una implementacion de
    /// otro tipo de evento que necesita manejo especifico por el bus de
    /// evento
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    private bool IsNotDomainOrModuleEvent(INotification notification)
        => notification is not IDomainEvent && notification is not IModuleEvent;
    
     
}
