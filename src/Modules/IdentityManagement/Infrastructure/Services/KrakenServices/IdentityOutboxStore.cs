using Kraken.Core.Outbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Infrastructure.Services.KrakenServices
{
    internal class IdentityOutboxStore : IOutboxStorage
    {
        private static ConcurrentBag<StorageMessage> outboxMessages = new ConcurrentBag<StorageMessage>();

        public IdentityOutboxStore()
        {

        }
        

        public Task SaveAsync(StorageMessage message)
        {
            outboxMessages.Add(message);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<StorageMessage>> GetUnsentAsync()
        {
            var unsent = outboxMessages.Where(x => x.SentAt is null);
            return Task.FromResult(unsent);
        }

        public Task Cleanup()
        {
            throw new NotImplementedException();
        }
    }
}
