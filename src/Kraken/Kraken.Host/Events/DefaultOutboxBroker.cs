using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Events.Outbox
{
    internal class DefaultOutboxBroker : IOutboxBroker
    {
        public ILogger<DefaultOutboxBroker> _logger;

        public DefaultOutboxBroker(ILogger<DefaultOutboxBroker> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(IKrakenEvent @event)
        {
            if (@event is DomainEvent)
                _logger.LogInformation("Event is domain event");
            await Task.CompletedTask;
        }
    }
}
