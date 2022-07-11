using Humanizer;
using Kraken.Core.Events;
using Kraken.Core.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Events
{
    /// <summary>
    /// Handler que se ocupa para evaluar el tipo de evento lanzado.
    /// Los 3 tipos de eventos se administran de forma diferente.
    /// - Evento de dominio
    ///     Estos eventos se escriben en el stream del modulo, 
    ///     para tener un registro de lo que ha pasado en la aplicacion.
    ///     Cuando se agregan al stream del modulo, tambien se agregan
    ///     en la lista de espera en el canal de distribucion sincrono, 
    ///     a la espera que su transaccion sea completada.
    /// - Evento de modulo
    ///     Estos eventos tambien se escriben en el stream del modulo, pero sus reacciones se escriben
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class OutboxProcessingHandler<T> : IDomainEventHandler<T>
        where T : IDomainEvent
    {
        /// <summary>
        /// Contiene la distribucion y almacenamiento para los eventos del
        /// modulo
        /// </summary>
        private readonly IOutboxBroker _outboxBroker;

        /// <summary>
        /// Despachador de eventos
        /// </summary>
        private readonly IEventDispatcher _eventDispatcher;

        private readonly ILogger<OutboxProcessingHandler<T>> _logger;

        public OutboxProcessingHandler(ILogger<OutboxProcessingHandler<T>> logger,
            IOutboxBroker outboxBroker, IEventDispatcher eventDispatcher)
        {
            _logger = logger;
            _outboxBroker = outboxBroker;
            _eventDispatcher = eventDispatcher;
        }

        public async Task Handle(IDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification is null)
                return;
            // Necesitamos tener el contexto del host,
            // con el contexto de identidad para crear
            // el contexto de eventos y administrar la
            // informacion de tracking en cada evento

            // Obtenemos la informacion 
            var module = notification.GetModuleName();
            var name = notification.GetType().Name.Underscore();
            // Logeamos un poco de informacion
            _logger.LogInformation("Publishing a message: {Name} ({Module}) [Request ID: RequestId, Message ID: MessageId, Correlation ID: CorrelationId, Trace ID: 'TraceId', User ID: 'UserId]...",
                name, module);
            // Lo agregamos en el outbox al que pertenece
            await _outboxBroker.SendAsync((IKrakenEvent)notification);
            // Lo agregamos a la lista de espera del despachador
            await _eventDispatcher.AddToWaitingList((IKrakenEvent)notification);
            // Salimos por que se cumplio con lo esperado
        }

    }
}
