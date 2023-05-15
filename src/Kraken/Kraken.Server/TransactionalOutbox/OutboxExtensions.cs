using Kraken.Module.Request.Mediator;
using Kraken.Module.TransactionalOutbox;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

internal static class OutboxExtensions
{
    public static IServiceCollection AddTransactionalOutbox(this IServiceCollection services)
    {
        // Registramos el proveedor de contexto
        services.AddScoped<ContextProvider>();
        // Decoramos el bus de eventos con el de la bandeja de salida
        services.TryDecorate<IEventPublisher, OutboxEventPublisher>();
        // Agregamos el servicio de bandeja de salidaa
        services.AddScoped<Outbox>();
        // Aggregamos el servicio de almacenamiento por defecto
        services.AddScoped<IOutboxStorage, DefaultOutboxStorage>();
        return services;
    }
}
