using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.EventBus
{
    /// <summary>
    /// Interface para la implementacion del bus de eventos para la
    /// distribucion de eventos de dominio e intermodulares, que se
    /// implementa de diferente manera, segun las necesidades de la
    /// aplicacion
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publica el evento pasado por parametro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Publish<T>(T @event, CancellationToken cancellationToken = default)
            where T : INotification;
    }
}
