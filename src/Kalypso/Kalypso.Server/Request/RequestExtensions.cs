﻿using Dottex.Kalypso.Module.Request.Mediator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dottex.Kalypso.Server.Transaction;

namespace Dottex.Kalypso.Server.Request;

internal static class RequestExtensions
{
    public static IServiceCollection AddCommandAndQueryProcessing(this IServiceCollection services,
        List<Assembly> assemblies)
    {
        // Lista de ensamblados donde buscar comandos y queries
        var modules = new List<Assembly>(assemblies)
        {
            typeof(RequestExtensions).Assembly
        };

        //Agregamos el mediador y los handlers para cada comando y query
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(modules.ToArray());
            configuration.AddOpenBehavior(typeof(CommandLoggingMiddleware<,>));
            configuration.AddOpenBehavior(typeof(PerformanceMiddleware<,>));
            configuration.AddOpenBehavior(typeof(ContextSetterMiddleware<,>));
            configuration.AddOpenBehavior(typeof(TransactionMiddleware<,>));
        });
        services.AddScoped<IEventPublisher, DefaultEventPublisher>();
        services.AddScoped<IAppHost, DefaultAppHost>();
        // Agregamos los middlewares para el pipeline de mediatr
        return services;
    }
}
