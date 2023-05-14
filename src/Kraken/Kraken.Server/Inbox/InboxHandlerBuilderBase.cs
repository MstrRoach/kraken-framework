using Kraken.Module.Context;
using Kraken.Module.Inbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

public abstract class InboxHandlerBuilderBase
{
    /// <summary>
    /// Maneja la notificacion desde un objeto anonimos
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceFactory"></param>
    /// <returns></returns>
    public async Task Handle(object notification, InboxMessage inboxMessage, CancellationToken cancellationToken,
    IServiceProvider serviceProvider, IContext context) =>
        await Handle((INotification)notification, inboxMessage, cancellationToken, serviceProvider, context);

    /// <summary>
    /// Ejecuta el handler para el evento junto con todos los middlewares 
    /// necesarios
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public abstract Task Handle(INotification notification,
        InboxMessage inboxMessage,
        CancellationToken cancellationToken,
        IServiceProvider serviceProvider,
        IContext context);

    /// <summary>
    /// Obtiene el handler especificado para cierta reaccion
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="factory"></param>
    /// <returns></returns>
    protected static TReaction GetHandler<TReaction>(IServiceProvider factory)
    {
        TReaction handler;

        try
        {
            handler = factory.GetRequiredService<TReaction>();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error constructing handler for request of type {typeof(TReaction)}.", e);
        }

        if (handler == null)
        {
            throw new InvalidOperationException($"Handler was not found for request of type {typeof(TReaction)}.");
        }

        return handler;
    }
}
