using Kraken.Module.Context;
using Kraken.Module.Inbox;
using Kraken.Server.Middlewares.Contexts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

/// <summary>
/// Objeto encargado de construir toda la solicitud que maneja al evento
/// y con ello todas las operaciones necesarias
/// </summary>
/// <typeparam name="TEvent"></typeparam>
/// <typeparam name="TReaction"></typeparam>
public class InboxHandlerBuilder<TEvent, TReaction> :
    InboxHandlerBuilderBase
    where TEvent : INotification
    where TReaction : INotificationHandler<TEvent>
{

    /// <summary>
    /// Ejecuta el evento con los valores mas especificados
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public override async Task Handle(INotification notification, InboxMessage reactionMessage,
        CancellationToken cancellationToken, IServiceProvider serviceProvider, IContext context)
    {
        // Creamos un alcance para el handler
        using var scope = serviceProvider.CreateScope();
        // Especificamos el contexto
        scope.ServiceProvider.GetRequiredService<ContextProvider>().Context = context;
        // Hacemos el delegado para ejeuctar las operaciones
        Task Handler() => GetHandler<TReaction>(scope.ServiceProvider).Handle((TEvent)notification, cancellationToken);
        // Obtenemos los middlewares y encadena cada uno de ellos para envolver al handler
        await scope.ServiceProvider.GetServices<IInboxMiddleware<TEvent, TReaction>>()
            .Reverse()
            .Aggregate((EventHandlerDelegate)Handler,
            (next, pipeline) =>
            () => pipeline.Handle((TEvent)notification, reactionMessage, cancellationToken, next))();
    }


}
