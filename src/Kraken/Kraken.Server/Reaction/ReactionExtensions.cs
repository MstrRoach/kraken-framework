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

namespace Kraken.Server.Reaction;

public static class ReactionExtensions
{

    /// <summary>
    /// Agrega el componente de reacciones a la aplicacion
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddReactions(this IServiceCollection services, List<Assembly> assemblies)
    {
        // Obtenemos el registro de reacciones
        var reactionRegistry = assemblies.GetReactionRegistry();
        // Registramos cada unos de las reacciones 
        reactionRegistry
            .GetAllReactions()
            .ToList()
            .ForEach(x => services.AddTransient(x));
        // Lo registramos como singlenton
        services.AddSingleton(reactionRegistry);
        // Registramos el procesador para que lo tome en lugar del default
        services.AddSingleton<IOutboxDispatcher, DefaultReactionDispatcher>();
        // Registramos el accesor al almacen de reacciones
        services.AddSingleton<DefaultReactionStorageAccessor>();
        // Registramos el almacen por defecto
        services.AddScoped(typeof(DefaultReactionStorage<>));
        return services;
    }

    /// <summary>
    /// Crea el registro de reacciones a partir de las reacciones de dominio y de modulos
    /// </summary>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    private static ReactionRegistry GetReactionRegistry(this List<Assembly> assemblies)
    {
        // Definicion de interfaces abiertas
        var domainEventHandlerOpenType = typeof(IDomainEventHandler<>);
        var moduleEventHandlerOpenType = typeof(IModuleEventHandler<>);

        // Hacemos la busqueda en los ensamblados de los handlers de dominio
        var domainEventReactions = assemblies.LocateReactionsFor(domainEventHandlerOpenType);
        var moduleEventReactions = assemblies.LocateReactionsFor(moduleEventHandlerOpenType);

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
    private static List<IGrouping<Type, Type>> LocateReactionsFor(this List<Assembly> assemblies, Type openTypeToSearch) => 
        assemblies
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => !type.IsOpenGeneric())
        .Where(type => type.FindInterfacesThatClose(openTypeToSearch).Any())
        .Select(type => new EventReaction(type.GetHandlerArgument(openTypeToSearch.Name), type))
        .GroupBy(eventReaction => eventReaction.Event, eventReaction => eventReaction.Reaction)
        .ToList();
}
