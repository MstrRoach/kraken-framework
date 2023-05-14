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
    public static IServiceCollection AddReactions(this IServiceCollection services)
    {
        // Definicion de interfaces abiertas
        var domainEventHandlerOpenType = typeof(IDomainEventHandler<>);
        var moduleEventHandlerOpenType = typeof(IModuleEventHandler<>);
        return services;
    }

    private static ReactionRegistry LocateAndRegisterReactions(this List<Assembly> assemblies)
    {
        return null;
    }
}
