using Kraken.Module.TransactionalOutbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

internal class DefaultOutboxDispatcher : IOutboxDispatcher
{
    /// <summary>
    /// Almacen de los eventos despachados
    /// </summary>
    private readonly IOutboxStorage _storage;

    /// <summary>
    /// Proveedor de los servicios de aplicacion
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public DefaultOutboxDispatcher(IOutboxStorage storage, IServiceProvider serviceProvider)
    {
        _storage = storage;
        _serviceProvider = serviceProvider;
    }

    public async Task Process(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtenemos el evento 
            var @event = message.Event as INotification;
            // Instanciamos al mediator
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(message.Event);
            await _storage.Update(
                id: message.Id, 
                status: OutboxRecordStatus.Processed, 
                sentAt: DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            await _storage.Update(
                id: message.Id, 
                status: OutboxRecordStatus.Processed, 
                notes: ex.Message);
        }
    }
}
