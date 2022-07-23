using Kraken.Core.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    public static class OutboxExtensions
    {
        /// <summary>
        /// Convierte un evento de dominio, en un evento centralizado para
        /// la bandeja de salida transaccional que permitira centralizar el
        /// evento para almacenarlo y posteriormente procesarlo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mediator"></param>
        /// <param name="domainEvent"></param>
        public static async Task SendToOutbox<T>(this IMediator mediator, T domainEvent)
            where T : IDomainEvent
        {
            // Si el evento esta vacio salimos
            if (domainEvent is null)
                return;
            // Creamos el evento de bandeja de salida transaccional
            var @event = new InterceptedDomainEvent
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType(),
                Event = domainEvent
            };
            // Lo distribuimos en el mediator
            await mediator.Publish(@event);
        }
    }
}
