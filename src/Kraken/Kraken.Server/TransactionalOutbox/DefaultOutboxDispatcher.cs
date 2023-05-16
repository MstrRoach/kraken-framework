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
    /// Proveedor de los servicios
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    public DefaultOutboxDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Process(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        // Instanciamos al mediator
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Publish(message.Event);
    }
}
