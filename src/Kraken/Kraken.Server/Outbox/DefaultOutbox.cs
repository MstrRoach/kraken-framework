using Kraken.Standard.Outbox;
using Kraken.Standard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Outbox;

internal class DefaultOutbox<TModule> : IOutbox<TModule>
    where TModule : IModule
{
    public Task Cleanup(DateTime? to = null)
    {
        throw new NotImplementedException();
    }

    public Task<OutboxMessage> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task PublishUnsent()
    {
        throw new NotImplementedException();
    }

    public Task Save(OutboxMessage messages)
    {
        throw new NotImplementedException();
    }
}
