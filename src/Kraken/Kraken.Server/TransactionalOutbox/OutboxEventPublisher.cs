using Humanizer;
using Kraken.Module;
using Kraken.Module.Context;
using Kraken.Module.TransactionalOutbox;
using Kraken.Module.Request.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kraken.Module.OutboxOld;

namespace Kraken.Server.TransactionalOutbox;

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
/// - Evento de modulo
///     Estos eventos se escriben igual en la bandeja de salida del modulo que lo genera, para
///     reenviarse una vez que se confirmen las transacciones se envien directamente a un 
///     procesador de eventos asincronos
/// - Evento de arquitectura
///     Estos eventos deben enviarse directamente al procesamiento de eventos pues estos, 
///     deben de desencadenar reacciones de forma inmediata
/// </summary>
internal class OutboxEventPublisher : IEventPublisher
{
    /// <summary>
    /// Bus de eventos internos para los eventos generales
    /// </summary>
    private readonly IEventPublisher _inner;

    /// <summary>
    /// Logger del bus de eventos
    /// </summary>
    private readonly ILogger<OutboxEventPublisher> _logger;

    /// <summary>
    /// Contexto de la solicitud actual
    /// </summary>
    private readonly IContext _context;

    /// <summary>
    /// Proveedor de contexto para el comando actual ejecutado
    /// </summary>
    private readonly ContextProvider _contextProvider;

    /// <summary>
    /// Bandeja de entrada encargada de coordinar las operaciones
    /// </summary>
    private readonly Outbox _outbox;

    public OutboxEventPublisher(IEventPublisher inner,
        ILogger<OutboxEventPublisher> logger,
        IContext context,
        ContextProvider contextProvider,
        Outbox outbox)
    {
        _inner = inner;
        _logger = logger;
        _context = context;
        _contextProvider = contextProvider;
        _outbox = outbox;
    }

    /// <summary>
    /// Se encarga de procesar todos los eventos del modulo a traves de la distincion entre
    /// eventos base de notificacion, eventos de conponente, eventos de modulo y eventos de
    /// dominio, para tratar a cada uno de ellos de manera especial segun la bandeja de salida
    /// transaccional
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : INotification
    {
        _logger.LogInformation("[OUTBOX EVENT BUS] Start event processing . . .");
        // Si la notificacion es nula, salimos
        if (@event is null) return;
        // Si no es un evento de bandeja de salida
        if (IsNotTransactionalEvent(@event))
        {
            // Publicamos el evento en el publisher por defecto
            _logger.LogInformation("[OUTBOX EVENT BUS] Publish notification in system . . .");
            await _inner.Publish(@event);
            _logger.LogInformation("[OUTBOX EVENT BUS] Notification dispatching successfully");
            return;
        }
        _logger.LogInformation("[OUTBOX EVENT BUS] Creating message from notification");
        // Convertimos el tipo a un evento base, para obtener la informacion
        var message = @event as IEvent ??
            throw new InvalidOperationException("Event can not convert as IEvent");
        _logger.LogInformation("[OUTBOX EVENT BUS] Get information abount message");
        // Reunimos la informacion del evento
        var module = message.GetModuleName();
        var name = message.GetType().Name.Underscore();
        var requestId = _context.RequestId;
        var traceId = _context.TraceId;
        var userId = _context.Identity?.Id;
        var username = _context.Identity?.Name;
        var messageId = message.Id;
        var correlationId = _context.CorrelationId;
        // Logeamos la informacion del evento almacenado
        _logger.LogInformation("Publishing a message: {Name} ({Module}) [Request ID: {RequestId}, Message ID: {MessageId}, Correlation ID: {CorrelationId}, Trace ID: '{TraceId}', User ID: '{UserId}]...",
                name, module, requestId, messageId, correlationId, traceId, userId);
        // Creamos el envoltorio para el evento que permite almacenar el contexto
        var eventMessage = new Module.TransactionalOutbox.OutboxMessage(
            messageId,
            correlationId,
            _contextProvider.Context.TransactionId,
            module,
            userId,
            username,
            traceId,
            message
            );
        // Mandamos a guardar el menssaje en la bandeja de salida
        _logger.LogInformation("[OUTBOX EVENT BUS] Store message in outbox");
        await _outbox.Save(eventMessage);
        _logger.LogInformation("[OUTBOX EVENT BUS] Save event in context for later processing");
        // Ya no agregamos el evento al contexto a la espera de enviarlo
        //_contextProvider.Context.AddEventMessage(eventMessage);
        _logger.LogInformation("[OUTBOX EVENT BUS] Finish event processing");
    }

    /// <summary>
    /// Revisa si la notificacion contiene ademas una implementacion de
    /// otro tipo de evento que necesita manejo especifico por el bus de
    /// evento
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    private bool IsNotTransactionalEvent(INotification notification)
        => notification is not IDomainEvent && notification is not IModuleEvent;
}
