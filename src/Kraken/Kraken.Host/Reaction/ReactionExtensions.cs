using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
using Kraken.Core.Reaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        services.AddTransient(typeof(IReactionMiddleware<,>), typeof(ReactionLogging<,>));
        services.AddTransient(typeof(IReactionMiddleware<,>),typeof(ReactionTransaction<,>));
        // Registramos los almacenes de reacciones
        services.LocateAndRegisterReactionStorages(assemblies);
        // Agregamos el broker para el almacenamiento de las reacciones
        services.AddSingleton<IReactionStreamFactory, DefaultReactionStreamFactory>();

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
        // Registramos cada storage en el registro centralizado
        storages.ForEach(x => storageRegistry.Register(x));
        // Registramos el generico para el reaction log
        services.AddScoped(typeof(DefaultReactionStream<>));
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
        var eventHandlerOpenInterface = typeof(IDomainEventHandler<>);
        var arity = eventHandlerOpenInterface.GetGenericArguments().Length;

        
        // Hacemos la busqueda en los ensamblados
        var reactions = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsOpenGeneric())
            .Where(type => type.FindInterfacesThatClose(eventHandlerOpenInterface).Any())
            .Select(type => new EventReaction(type.GetHandlerArgument(eventHandlerOpenInterface.Name),type))
            .GroupBy(eventReaction => eventReaction.Event, eventReaction => eventReaction.Reaction)
            .ToList();

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

    /// <summary>
    /// Busca las interfaces cerradass que coinciden con la interface abierta proporcionada
    /// </summary>
    /// <param name="pluggedType"></param>
    /// <param name="templateType"></param>
    /// <returns></returns>
    private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
    {
        return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
    }

    /// <summary>
    /// Realiza la busqueda recursiva en todos los tipos derivados para encontrar las interfaces
    /// cerradas que cummplen con la interface abierta
    /// </summary>
    /// <param name="pluggedType"></param>
    /// <param name="templateType"></param>
    /// <returns></returns>
    private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
    {
        if (pluggedType == null) yield break;

        if (!pluggedType.IsConcrete()) yield break;

        if (templateType.GetTypeInfo().IsInterface)
        {
            foreach (
                var interfaceType in
                pluggedType.GetInterfaces()
                    .Where(type => type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
            {
                yield return interfaceType;
            }
        }
        else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
                 (pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType))
        {
            yield return pluggedType.GetTypeInfo().BaseType;
        }

        if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

        foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
        {
            yield return interfaceType;
        }
    }

    /// <summary>
    /// Indica si el tipo especificado es concreto y si es interface
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsConcrete(this Type type)
    {
        return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
    }

    /// <summary>
    /// Indica si el tipo es un generico abierto
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsOpenGeneric(this Type type)
    {
        return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
    }

    /// <summary>
    /// Obtiene el parametro generico del tipo que implementa la interface
    /// </summary>
    /// <param name="type"></param>
    /// <param name="openInterface"></param>
    /// <returns></returns>
    private static Type GetHandlerArgument(this Type type, string openInterface)
    {
        return type.GetInterface(openInterface).GetGenericArguments().FirstOrDefault();
    }

    private record EventReaction(Type Event, Type Reaction);

}
