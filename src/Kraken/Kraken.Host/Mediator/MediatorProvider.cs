﻿using Kraken.Core.Mediator;
using Kraken.Core.UnitWork;
using Kraken.Host.Mediator.Commands;
using Kraken.Host.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Mediator
{
    internal static class MediatorProvider
    {

        /// <summary>
        /// Configura el mediador para su disponibilidad en el host
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediator(this IServiceCollection services, List<Assembly> assemblies)
        {
            var assembliesToScan = new List<Assembly>();
            assembliesToScan.AddRange(assemblies);
            var host = typeof(MediatorProvider).Assembly;
            assembliesToScan.Add(host);
            
            /*
             * Agregamos el mediador y los handlers para cada comando y query
             */
            services.AddMediatR(assembliesToScan.ToArray());
            
            /*
             * Agrega a la lista los middlewares para el pipeline de mediatr y los ejecuta en el
             * orden que se registran
             */
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandLogging<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceMiddleware<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Validation<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Transaction<,>));

            return services;
        }
    }
}