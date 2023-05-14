using Kraken.Module;
using Kraken.Module.Outbox;
using Kraken.Module.Request.Mediator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

public static class InboxExtensions
{

    /// <summary>
    /// Agrega el componente de inbox a la aplicacion
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTransactionalInbox(this IServiceCollection services, List<Assembly> assemblies)
    {
        // Obtenemos el registro de handlers
        var inboxHandlerRegistry = assemblies.GetInboxHandlerRegistry();
        // Registramos cada uno de los handlers 
        inboxHandlerRegistry
            .GetAllHandlers()
            .ToList()
            .ForEach(x => services.AddTransient(x));
        // Lo registramos como singlenton
        services.AddSingleton(inboxHandlerRegistry);
        // Registramos el procesador para que lo tome en lugar del default
        services.AddSingleton<IOutboxDispatcher, DefaultInboxDispatcher>();
        // Registramos el accesor al almacen de handlers
        services.AddSingleton<DefaultInboxStorageAccessor>();
        // Registramos el almacen por defecto
        services.AddScoped(typeof(DefaultInboxStorage<>));
        // Registramos los middlewares

        return services;
    }

    /// <summary>
    /// Crea el registro de reacciones a partir de las reacciones de dominio y de modulos
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    private static InboxHandlerRegistry GetInboxHandlerRegistry(this List<Assembly> assemblies)
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
        var reactionRegistry = new InboxHandlerRegistry();

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
        .Select(type => new InboxEvent(type.GetHandlerArgument(openTypeToSearch.Name), type))
        .GroupBy(eventReaction => eventReaction.Event, eventReaction => eventReaction.Handler)
        .ToList();
}
