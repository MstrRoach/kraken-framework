using Kraken.Core.Events;
using Kraken.Core.Events.Outbox;
using Kraken.Core.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Events
{
    internal static class OutboxExtensions
    {
        /// <summary>
        /// Agrega el soporte para outbox y todo el mecanismo de eventos
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOutbox(this IServiceCollection services)
        {
            // Componente utilizado para distribuir los eventos para cada Outbox de cada modulo
            services.AddSingleton<IOutboxBroker, DefaultOutboxBroker>();
            // Componente encargado de procesar en segundo plano los eventos, los agregamos como instancia y como proceso
            services.AddSingleton<IEventDispatcher, DefaultEventDispatcher>()
                .TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, IEventDispatcher>(sp => sp.GetRequiredService<IEventDispatcher>()));
            // Retornamos los servicios
            return services;
        }
    }
}
