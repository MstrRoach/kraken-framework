using Kraken.Core.Internal.EventBus;
using Kraken.Core.Outbox;
using Kraken.Core.Processing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    public static class OutboxExtensions
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services, List<Assembly> assemblies)
        {
            // Agregamos el bus de eventos basado en la bandeja de salida
            services.AddTransient<IEventBus, OutboxEventBus>();
            // Agregamos como singlenton el accesor para el contexto
            services.AddScoped<OutboxContextAccesor>();
            // El outbox hub se registra automaticamente cuando se analizan los modulos
            // Registramos el broker de las bandejas de salida
            services.AddScoped<IOutboxBroker, DefaultOutboxBroker>();
            // Buscamos todas las bandejas de salida
            var stores = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterface(nameof(IOutboxStorage)) is not null)
                .ToList();
            // Creamos el registro de bandejas de salida
            var outboxRegistry = new OutboxStoreRegistry();
            // Recorremos las bandejas encontradas
            foreach (var store in stores)
            {
                var outboxOpenType = typeof(DefaultOutbox<>).MakeGenericType(store);
                // Registramos la bandeja de salida especifica
                services.AddScoped(outboxOpenType);
                //services.TryAddEnumerable(new ServiceDescriptor(outboxOpenType, outboxOpenType, ServiceLifetime.Scoped));
                // Agregamos el servicio para recuperar 
                services.AddScoped<IOutboxStorage>(sp => sp.GetService(store) as IOutboxStorage);
                outboxRegistry.Register(store);
                // El registro de la bandeja de salida de modulo como dependencia independeiente se deja
                // a criterio del usuario el registro o la omision de la uniddad de trabajo para cada
                // modulo
            }
            // Registramos la instancia del registro de bandejas de salida
            services.AddSingleton(outboxRegistry);
            // Registramos la instancia del orquestador de eventos
            services.AddSingleton<IEventDispatcher, DefaultEventDispatcher>();
            // Registramos el elemento a la lista de servidores de procesamiento
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, IEventDispatcher>(sp => sp.GetRequiredService<IEventDispatcher>()));
            // Agregamos el procesador de eventos por defecto
            services.AddSingleton<IEventProcessor, DefaultEventProcessor>();

            return services;
        }

    }
}
