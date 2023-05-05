using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Request.Events;

/// <summary>
/// Interface para la implementacion del bus de eventos para la
/// distribucion de eventos de dominio, intermodulares y de arquitectura.
/// </summary>
public interface IEventPublisher
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