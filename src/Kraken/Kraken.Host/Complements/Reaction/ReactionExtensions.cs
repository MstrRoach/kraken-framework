using Kraken.Core.Outbox;
using Kraken.Core.Reaction;
using Kraken.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kraken.Core.EventBus;
using Kraken.Core.Processing;

namespace Kraken.Host.Reaction;

public static class ReactionExtensions
{
    /// <summary>
    /// Agrega el componente de reacciones a la aplicacion
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddReactions(this IServiceCollection services, List<Assembly> assemblies)
    {
        // Buscamos y registramos las reacciones
        services.LocateAndRegisterReactions(assemblies);
        // Sustituimos el procesador
        services.AddSingleton<IEventProcessor, ReactionEventProcessor>();
        // Agregamos los middlewares
        services.AddTransient(typeof(IReactionMiddleware<,>), typeof(ReactionLoggingMiddleware<,>));
        services.AddTransient(typeof(IReactionMiddleware<,>),typeof(ReactionTransactionMiddleware<,>));
        services.AddTransient(typeof(IReactionMiddleware<,>),typeof(ReactionMarkingMiddleware<,>));
        // Registramos los almacenes de reacciones
        services.LocateAndRegisterReactionStorages(assemblies);
        // Agregamos el broker para el almacenamiento de las reacciones
        services.AddSingleton<IReactionStreamFactory, DefaultReactionStreamFactory>();
        // Agregamos el reprocesador de reacciones en segundo plano
        services.AddSingleton<IProcessingServer, ReactionRedispatcher>();
        return services;
    }

    /// <summary>
    /// Busca y registra lo necesario para los almacenes de las reacciones
    /// asi como cada implementacion especifica para cada una de los almacenes
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    private static void LocateAndRegisterReactionStorages(this IServiceCollection services, List<Assembly> assemblies)
    {
        // Registro de los storages
        var storageRegistry = new ReactionStorageRegistry();
        // Buscamos todas las implementaciones de los strage
        var storages = assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type => type.GetInterface(nameof(IReactionStorage)) is not null)
            .ToList();
        // Recorremos los storages
        foreach (var storage in storages)
        {
            // Registramos el storage en el registro
            storageRegistry.Register(storage);
            // Creamos el tipo cerrado
            var reactionStreamClosedType = typeof(DefaultReactionStream<>).MakeGenericType(storage);
            // Lo registramos con alcance para llamada desde interface
            services.AddScoped(typeof(IReactionStream), reactionStreamClosedType);
            // Lo registramos para la llamada especifica
            services.AddScoped(reactionStreamClosedType);
        }
        // Registramos la instancia del registro de almacenes
        services.AddSingleton(storageRegistry);
    }

    /// <summary>
    /// Localiza y registra las reacciones, tanto en l
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    private static void LocateAndRegisterReactions(this IServiceCollection services, List<Assembly> assemblies)
    {
        // Definicion de la interface abierta
        var domainEventHandlerOpen = typeof(IDomainEventHandler<>);
        var moduleEventHandlerOpen = typeof(IModuleEventHandler<>);
        var arity = domainEventHandlerOpen.GetGenericArguments().Length;

        
        // Hacemos la busqueda en los ensamblados de los handlers de dominio
        var reactions = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsOpenGeneric())
            .Where(type => type.FindInterfacesThatClose(domainEventHandlerOpen).Any())
            .Select(type => new EventReaction(type.GetHandlerArgument(domainEventHandlerOpen.Name),type))
            .GroupBy(eventReaction => eventReaction.Event, eventReaction => eventReaction.Reaction)
            .ToList();
        // Hacemos la busqueda de los handlers de los modulos
        var moduleReactions = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsOpenGeneric())
            .Where(type => type.FindInterfacesThatClose(moduleEventHandlerOpen).Any())
            .Select(type => new EventReaction(type.GetHandlerArgument(moduleEventHandlerOpen.Name), type))
            .GroupBy(eventReaction => eventReaction.Event, eventReaction => eventReaction.Reaction)
            .ToList();

        reactions.AddRange(moduleReactions);

        // Creamos el registro de reacciones
        var reactionRegistry = new ReactionRegistry();

        // Recorremos el agrupamiento
        foreach(var reaction in reactions)
        {
            // Registramos los handlers
            reaction.ToList().ForEach(x => services.AddTransient(x));
            // Registramos la relacion entre evento y reaccion
            reactionRegistry.Register(reaction.Key, reaction.ToList());
        }
        // Agregamos como singlenton el registro de reacciones
        services.AddSingleton(reactionRegistry);
    }

    private record EventReaction(Type Event, Type Reaction);

}
