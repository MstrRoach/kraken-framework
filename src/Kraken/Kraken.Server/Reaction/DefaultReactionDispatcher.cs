
using Kraken.Module.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Reaction;

internal class DefaultReactionDispatcher : IOutboxDispatcher
{
    public Task ProcessAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
