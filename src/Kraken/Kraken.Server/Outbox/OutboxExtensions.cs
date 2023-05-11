using Kraken.Standard.Outbox;
using Kraken.Standard.Request;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Outbox
{
    internal static class OutboxExtensions
    {
        public static IServiceCollection AddTransactionalOutbox(this IServiceCollection services)
        {
            // Registramos el proveedor de contexto para bandeja de salida
            services.AddScoped<OutboxContextProvider>();
            // Decoramos el bus de eventos con el de la bandeja de salida
            services.TryDecorate<IEventPublisher, OutboxEventPublisher>();
            // Registramos la fabrica para la bandeja de salida
            services.AddScoped<DefaultOutboxFactory>();
            // Registramos el tipo abierto para la bandeja de salida de tipo abierto
            services.AddScoped(typeof(DefaultOutbox<>));
            // Registramos la fabrica para la construccion
            services.AddScoped<DefaultOutboxStorageFactory>();
            // Registramos la implementacion por defecto y de tipo abierta para el storage
            services.AddScoped(typeof(DefaultOutboxStorage<>));
            return services;
        }
    }
}
