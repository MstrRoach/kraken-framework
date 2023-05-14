using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Reaction;

public interface IReactionMiddleware<TEvent, TReaction>
        where TEvent : INotification
        where TReaction : INotificationHandler<TEvent>
{
    /// <summary>
    /// Administra la operacion del evento a traves del pipeline
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    Task Handle(TEvent @event, ReactionMessage processRecord, CancellationToken cancellationToken, EventHandlerDelegate next);
}

/// <summary>
/// Delegado que representa una accion que se ejecuta internamente
/// </summary>
/// <returns></returns>
public delegate Task EventHandlerDelegate();
