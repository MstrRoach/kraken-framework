using Kraken.Module.Inbox;
using Kraken.Module.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

internal class DefaultInboxStorage<TModule> : IInboxStorage<TModule>
    where TModule : IModule
{
    private static ConcurrentBag<InboxRecord> inboxRecords = new ConcurrentBag<InboxRecord>();

    public Task Save(InboxRecord record)
    {
        if (!inboxRecords.Any(x => x.Id == record.Id))
            inboxRecords.Add(record);
        return Task.CompletedTask;
    }

    public Task SaveAll(List<InboxRecord> records)
    {
        foreach (var record in records)
        {
            Save(record);
        }
        return Task.CompletedTask;
    }

    public Task Update(Guid id, InboxRecordStatus status, string error = null)
    {
        var record = inboxRecords.FirstOrDefault(x => x.Id == id);
        if (record is null)
            return Task.CompletedTask;
        record.UpdateAt = DateTime.UtcNow;
        record.Status = status;
        record.Error = error;
        return Task.CompletedTask;
    }

    public Task<List<InboxRecord>> GetUnprocessed()
    {
        var processables = inboxRecords.Where(x => x.Status == InboxRecordStatus.Scheduled).ToList();
        return Task.FromResult(processables);
    }

}
