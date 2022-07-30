using Humanizer;
using Kraken.Core;
using Kraken.Core.Contexts;
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
public sealed class OutboxEventHub : INotificationHandler<InterceptedEvent>
{
    /// <summary>
    /// Logger del handler
    /// </summary>
    private readonly ILogger<OutboxEventHub> _logger;

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

    public OutboxEventHub(ILogger<OutboxEventHub> logger,
        IContext context, OutboxContextAccesor outboxContextAccessor,
        IOutboxBroker outboxBroker)
    {
        _logger = logger;
        _context = context;
        _outboxContextAccessor = outboxContextAccessor;
        _outboxBroker = outboxBroker;
    }

    /// <summary>
    /// Todos los eventos de dominio son centralizados en este handler para meterlos dentro del contexto de
    /// bandeja de salida que tiene ejecutandose en el alcance actual. Ademas, se agrega directamente al stream
    /// de la bandeja de salida del modulo al que pertenece
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(InterceptedEvent notification, CancellationToken cancellationToken)
    {
        if (notification is null)
            return;

        // Reunimos la informacion para el procesamiento del evento
        var module = notification.SourceModule;
        var name = notification.Event.GetType().Name.Underscore();
        var requestId = _context.RequestId;
        var traceId = _context.TraceId;
        var userId = _context.Identity?.Id;
        var messageId = notification.Id;
        var correlationId = _context.CorrelationId;
        // Logeamos la informacion del evento almacenado
        _logger.LogInformation("Publishing a message: {Name} ({Module}) [Request ID: {RequestId}, Message ID: {MessageId}, Correlation ID: {CorrelationId}, Trace ID: '{TraceId}', User ID: '{UserId}]...",
                name, module, requestId, messageId, correlationId, traceId, userId);
        // Envolvemos el evento en una entidad enriquecida para almacenarlo
        var processMessage = new ProcessMessage
        {
            Id = notification.Id,
            CorrelationId = correlationId,
            UserId = userId,
            Event = notification.Event,
            TraceId = traceId
        };
        // Mandamos el evento al corredor de bandeja de salida
        await _outboxBroker.SendAsync(processMessage);
        // Agregamos el evento al contexto de la bandeja de salida
        _outboxContextAccessor.Context.AddProcessMessage(processMessage);
    }

}
