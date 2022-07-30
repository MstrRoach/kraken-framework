using Kraken.Core.Internal;
using Kraken.Core.Internal.Domain;
using Kraken.Core.Internal.EventBus;
using Kraken.Core.Internal.Serializer;
using Kraken.Core.Internal.Transaction;
using Kraken.Host.Internal.EventBus;
using Kraken.Host.Internal.Mediator;
using Kraken.Host.Internal.Serializer;
using Kraken.Host.Internal.Storage;
using Kraken.Host.Internal.Transaction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal
{
    public static class InternalExtensions
    {
        /// <summary>
        /// Registra todos los servicios basicos para usar del kernel
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddKrakenKernel(this IServiceCollection services, List<Assembly> assemblies)
        {
            // Agregamos el serializador por defecto
            services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
            // Agregamos el host de kraken
            services.AddSingleton<IAppHost, DefaultHost>();
            // Agregamos el mediador
            services.AddMediator(assemblies);
            // Agregamos los servicios por defecto
            // Fabrica de unidad de trabajo por defecto
            services.AddSingleton<IUnitWorkFactory, DefaultUnitWorkFactory>();
            // Unidad de trabajo por defecto
            services.AddScoped<IUnitWork, DefaultUnitWork>();
            // Agregamos el bus de eventos por defecto
            services.AddScoped<IEventBus, DefaultEventBus>();
            // Agregamos los repositorios y todo lo que tenga que ver con eso
            services.AddRepositorySupport(assemblies);
            return services;
        } 
    }
}
