using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
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
            // Agregamos como singlenton el accesor para el contexto
            services.AddScoped<OutboxContextAccesor>();
            // El outbox hub se registra automaticamente cuando se analizan los modulos
            // Registramos el broker de las bandejas de salida
            services.AddScoped<IOutboxBroker, DefaultOutboxBroker>();
            // Buscamos todas las bandejas de salida
            var stores = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterface(nameof(IOutboxStore)) is not null)
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
                services.AddScoped<IOutboxStore>(sp => sp.GetService(store) as IOutboxStore);
                outboxRegistry.Register(store);
                // El registro de la bandeja de salida de modulo como dependencia independeiente se deja
                // a criterio del usuario el registro o la omision de la uniddad de trabajo para cada
                // modulo
            }
            // Registramos la instancia del registro de bandejas de salida
            services.AddSingleton(outboxRegistry);

            return services;
        }

    }
}
