using Humanizer;
using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    internal class DefaultOutboxBroker : IOutboxBroker
    {
        /// <summary>
        /// Logger del broker
        /// </summary>
        private readonly ILogger<DefaultOutboxBroker> _logger;

        /// <summary>
        /// Registro de las bandejas de salida de los modulos
        /// </summary>
        private readonly OutboxStoreRegistry _outboxRegistry;

        private readonly IServiceProvider _serviceProvider;

        public DefaultOutboxBroker(ILogger<DefaultOutboxBroker> logger, 
            OutboxStoreRegistry outboxRegistry, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _outboxRegistry = outboxRegistry;
            _serviceProvider = serviceProvider;
        }

        public async Task SendAsync<T>(T message)
            where T : IDomainEvent
        {
            _logger.LogInformation("[Event Broker] Processing event {event}, from module {module}", 
                message.GetType().Name.Underscore(),
                message.GetModuleName());
            // Obtenemos el tipo de almacenamiento
            var outboxStoreType = _outboxRegistry.Resolve(message);
            // Creamos el tipo cerrado
            var outboxType = typeof(DefaultOutbox<>).MakeGenericType(outboxStoreType);
            // Obtenemos la bandeja de ssalida
            var outbox = _serviceProvider.GetRequiredService(outboxType) as IOutbox;
            // Si es nula avisamos que no hay ninguna registrada
            if (outbox == null)
                throw new InvalidOperationException($"Outbox is not registered for module: '{message.GetModuleName()}'.");
            // Enviamos a la bandeja de salida
            await outbox.SaveAsync(message);
        }
    }
}
