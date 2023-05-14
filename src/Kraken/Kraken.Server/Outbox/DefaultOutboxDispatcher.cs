using Kraken.Module.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Outbox;

internal class DefaultOutboxDispatcher : IOutboxDispatcher
{
    /// <summary>
    /// Proveedor de los servicios de la aplicacion
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public DefaultOutboxDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Processa la distribucion de eventos.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ProcessAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtenemos el evento 
            var @event = message.Event as INotification;
            // Instanciamos al mediator
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(message.Event);
        }
        catch (Exception ex)
        {
            // Loggeamos el error del evento
        }
    }
}
