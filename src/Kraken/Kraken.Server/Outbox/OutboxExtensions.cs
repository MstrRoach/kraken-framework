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
            return services;
        }
    }
}
