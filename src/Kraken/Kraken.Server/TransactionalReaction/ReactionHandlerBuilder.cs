using Kraken.Module.Context;
using Kraken.Module.TransactionalReaction;
using Kraken.Server.Middlewares.Contexts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalReaction;

public class ReactionHandlerBuilder<TEvent, TReaction> :
    ReactionHandlerBuilderBase
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
    public override async Task Handle(ReactionMessage reactionMessage, CancellationToken cancellationToken, IServiceProvider serviceProvider, IContext context)
    {
        // Creamos un alcance para el handler
        using var scope = serviceProvider.CreateScope();
        // Especificamos el contexto
        scope.ServiceProvider.GetRequiredService<ContextProvider>().Context = context;
        // Hacemos el delegado para ejeuctar las operaciones
        Task Handler() => GetHandler<TReaction>(scope.ServiceProvider).Handle((TEvent)reactionMessage.Event, cancellationToken);
        // Obtenemos los middlewares y encadena cada uno de ellos para envolver al handler
        await scope.ServiceProvider.GetServices<IReactionMiddleware<TEvent, TReaction>>()
            .Reverse()
            .Aggregate((EventHandlerDelegate)Handler,
            (next, pipeline) =>
            () => pipeline.Handle((TEvent)reactionMessage.Event, reactionMessage, cancellationToken, next))();
    }
}
