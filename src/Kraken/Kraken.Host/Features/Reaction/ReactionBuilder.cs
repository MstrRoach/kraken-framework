using Kraken.Core.Contexts;
using Kraken.Core.Reaction;
using Kraken.Host.Contexts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction;

/// <summary>
/// Base para la construccion de manejador de las reacciones
/// </summary>
public abstract class ReactionBuilderBase
{
    /// <summary>
    /// Maneja la notificacion desde un objeto anonimos
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceFactory"></param>
    /// <returns></returns>
    public async Task Handle(object notification, ProcessRecord processRecord, CancellationToken cancellationToken,
    IServiceProvider serviceProvider, IContext context) =>
        await Handle((INotification)notification, processRecord, cancellationToken, serviceProvider, context);

    /// <summary>
    /// Ejecuta el handler para el evento junto con todos los middlewares 
    /// necesarios
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public abstract Task Handle(INotification notification,
        ProcessRecord processRecord,
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
            throw new InvalidOperationException($"Error constructing handler for request of type {typeof(TReaction)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
        }

        if (handler == null)
        {
            throw new InvalidOperationException($"Handler was not found for request of type {typeof(TReaction)}. Register your handlers with the container. See the samples in GitHub for examples.");
        }

        return handler;
    }
}

/// <summary>
/// Contiene lo necesario para la ejecucion de una reaccion 
/// asociada a un evento en especifico
/// </summary>
/// <typeparam name="TEvent"></typeparam>
/// <typeparam name="TReaction"></typeparam>
public class ReactionBuilder<TEvent, TReaction> :
    ReactionBuilderBase
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
    public override Task Handle(INotification notification, ProcessRecord processRecord,
        CancellationToken cancellationToken, IServiceProvider serviceProvider, IContext context)
    {
        // Creamos un alcance para el handler
        using var scope = serviceProvider.CreateScope();
        // Especificamos el contexto
        scope.ServiceProvider.GetRequiredService<ContextAccessor>().Context = context;
        // Hacemos el delegado para ejeuctar las operaciones
        Task Handler() => GetHandler<TReaction>(scope.ServiceProvider).Handle((TEvent)notification, cancellationToken);
        // Obtenemos los middlewares y encadena cada uno de ellos para envolver al handler
        return scope.ServiceProvider.GetServices<IReactionMiddleware<TEvent,TReaction>>()
            .Reverse()
            .Aggregate((EventHandlerDelegate)Handler, 
            (next, pipeline) => 
            () => pipeline.Handle((TEvent)notification, processRecord, cancellationToken, next))
            .Invoke();
    }

    
}
