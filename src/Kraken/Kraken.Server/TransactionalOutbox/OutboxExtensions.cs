using Kraken.Module.Processing;
using Kraken.Module.Request.Mediator;
using Kraken.Module.TransactionalOutbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

internal static class OutboxExtensions
{
    public static IServiceCollection AddTransactionalOutbox(this IServiceCollection services, ServiceDescriptor? outboxStorageDescriptor)
    {
        // Registramos el proveedor de contexto
        services.AddScoped<ContextProvider>();
        // Decoramos el bus de eventos con el de la bandeja de salida
        services.TryDecorate<IEventPublisher, OutboxEventPublisher>();
        // Agregamos el servicio de bandeja de salidaa
        services.AddScoped<Outbox>();
        // Aggregamos el servicio de almacenamiento por defecto
        services.Add(outboxStorageDescriptor ?? ServiceDescriptor.Describe(
            typeof(IOutboxStorage),
            typeof(DefaultOutboxStorage),
            ServiceLifetime.Singleton
            ));
        // Registramos el broker
        services.AddSingleton<OutboxBroker>();
        // Registramos el elemento a la lista de servicios de procesamiento
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingService, OutboxBroker>(sp => sp.GetRequiredService<OutboxBroker>()));
        // Registramos el despachador por defecto
        services.AddSingleton<OutboxProcessor>();
        // Registramos el despachador de eventos
        services.AddSingleton<IOutboxDispatcher, DefaultOutboxDispatcher>();
        return services;
    }
}
