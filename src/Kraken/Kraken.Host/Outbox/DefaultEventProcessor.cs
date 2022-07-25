using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    internal class DefaultEventProcessor : IEventProcessor
    {
        /// <summary>
        /// Proveedor de los servicios de la aplicacion
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        public DefaultEventProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Processa la distribucion de eventos.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ProcessAsync(ProcessMessage message, CancellationToken cancellationToken = default)
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
}
