using Kraken.Core.Mediator.Events;
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
        /// <param name="event"></param>
        public static async Task ToOutbox<T>(this IMediator mediator, T @event)
            where T : INotification
        {
            // Si el evento esta vacio salimos
            if (@event is null)
                return;
            // Contenedor del evento
            InterceptedEvent interceptedEvent = null;

            if(@event.TryConvert<IDomainEvent>(out var domainEvent))
            {
                // Creamos el evento de bandeja de salida transaccional
                interceptedEvent = new InterceptedEvent
                {
                    Id = domainEvent.Id,
                    Type = domainEvent.GetType(),
                    Event = domainEvent,
                    SourceModule = domainEvent.GetModuleName()
                };
            }

            if(@event.TryConvert<IModuleEvent>(out var moduleEvent))
            {
                // Creamos el evento de bandeja de salida transaccional
                interceptedEvent = new InterceptedEvent
                {
                    Id = moduleEvent.Id,
                    Type = moduleEvent.GetType(),
                    Event = moduleEvent,
                    SourceModule = moduleEvent.GetModuleName()
                };
            }

            // Si el modulo interceptado es null, salimos
            if (interceptedEvent is null)
                return;
            
            // Lo distribuimos en el mediator
            await mediator.Publish(interceptedEvent);
        }

        /// <summary>
        /// Convierte la entrada en la salida, si implementan el tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inbound"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryConvert<T>(this INotification inbound, out T result)
        {
            result = default;
            if(inbound is T)
                result = (T)inbound;
            return result is not null;
        }

        
    }



}
