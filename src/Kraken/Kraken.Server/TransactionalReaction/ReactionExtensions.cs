using Kraken.Module;
using Kraken.Module.Processing;
using Kraken.Module.Request.Mediator;
using Kraken.Module.TransactionalOutbox;
using Kraken.Module.TransactionalReaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalReaction;

internal static class ReactionExtensions
{
    /// <summary>
    /// Agrega el soporte para reacciones transaccionales en los modulos
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTransactionalReaction(this IServiceCollection services, List<Assembly> assemblies, ServiceDescriptor? reactionStorageDescriptor)
    {
        // Obtenemos el registro de handlers
        var inboxHandlerRegistry = assemblies.GetInboxHandlerRegistry();
        // Registramos cada uno de los handlers 
        inboxHandlerRegistry
            .GetAllHandlers()
            .ToList()
            .ForEach(x => services.AddTransient(x));
        // Lo registramos como singleton
        services.AddSingleton(inboxHandlerRegistry);
        // Registramos el procesador de eventos
        services.AddSingleton<IOutboxDispatcher, ReactionOutboxDispatcher>();
        // Registramos el centralizador de reacciones
        services.AddSingleton<Reactor>();
        // Aggregamos el servicio de almacenamiento por defecto
        services.Add(reactionStorageDescriptor ?? ServiceDescriptor.Describe(
            typeof(IReactionStorage),
            typeof(DefaultReactionStorage),
            ServiceLifetime.Singleton
            ));
        // Agregamos el broker
        services.AddSingleton<ReactionBroker>();
        // Registraos el broker en los servicios de procesamiento
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingService,ReactionBroker>(sp => sp.GetRequiredService<ReactionBroker>()));
        // Registramos el procesador
        services.AddSingleton<ReactionProcessor>();
        // Agregamos los middlewares
        services.AddTransient(typeof(IReactionMiddleware<,>), typeof(ReactionLoggingMiddleware<,>));
        services.AddTransient(typeof(IReactionMiddleware<,>), typeof(ReactionTransactionalMiddleware<,>));
        return services;
    }

    /// <summary>
    /// Crea el registro de reacciones a partir de las reacciones de dominio y de modulos
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    private static ReactionRegistry GetInboxHandlerRegistry(this List<Assembly> assemblies)
    {
        // Definicion de interfaces abiertas
        var domainEventHandlerOpenType = typeof(IDomainEventHandler<>);
        var moduleEventHandlerOpenType = typeof(IModuleEventHandler<>);

        // Hacemos la busqueda en los ensamblados de los handlers de dominio
        var domainEventReactions = assemblies.LocateHandlersFor(domainEventHandlerOpenType);
        var moduleEventReactions = assemblies.LocateHandlersFor(moduleEventHandlerOpenType);

        // Reunimos todas las reacciones en una sola lista
        domainEventReactions.AddRange(moduleEventReactions);

        // Creamos el contenedor de las reacciones
        var reactionRegistry = new ReactionRegistry();

        // Agregamos cada reaccion agrupada al registro de reacciones
        domainEventReactions.ForEach(x => reactionRegistry.Register(x.Key, x.ToList()));
        // Retornamos la lista de reacciones registradas
        return reactionRegistry;
    }

    /// <summary>
    /// Lambda para la busqueda de las reacciones a eventos de un tipo determinado
    /// </summary>
    /// <param name="assemblies"></param>
    /// <param name="openTypeToSearch"></param>
    /// <returns></returns>
    private static List<IGrouping<Type, Type>> LocateHandlersFor(this List<Assembly> assemblies, Type openTypeToSearch) =>
        assemblies
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => !type.IsOpenGeneric())
        .Where(type => type.FindInterfacesThatClose(openTypeToSearch).Any())
        .Select(type => new ReactionEvent(type.GetHandlerArgument(openTypeToSearch.Name), type))
        .GroupBy(eventReaction => eventReaction.Event, eventReaction => eventReaction.Handler)
        .ToList();
}
