using Kraken.Core.Internal.EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal.EventBus
{
    /// <summary>
    /// Bus de eventos que distribuye todos los eventos localmente
    /// haciendo uso de mediatr para ejecutar los eventos
    /// </summary>
    public class DefaultEventBus : IEventBus
    {
        /// <summary>
        /// Publicador interno de los mensajes
        /// </summary>
        private readonly IPublisher _publisher;

        /// <summary>
        /// Constructor del publicador nativo de mediatr
        /// </summary>
        /// <param name="publisher"></param>
        public DefaultEventBus(IPublisher publisher)
        {
            _publisher = publisher;
        }

        /// <summary>
        /// Realiza la publicacion de los eventos de la aplicacion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : INotification
        {
            // Como no tenemos ninguna otra estrategia, solo despachamos el evento
            // haciendo uso del publicador que ya tiene todos los handlers para
            // las notificaciones mapeadas y construidas internamente
            await _publisher.Publish(@event, cancellationToken);
        }
    }
}
