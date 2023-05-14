﻿using Kraken.Module.Inbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infrastructure;

internal class CommonReactionStorage : IInboxStorage<AccessControlModule>
{
    private static ConcurrentBag<InboxRecord> reactionRecords = new ConcurrentBag<InboxRecord>();

    public Task Save(InboxRecord record)
    {
        if (!reactionRecords.Any(x => x.Id == record.Id))
            reactionRecords.Add(record);
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
        var record = reactionRecords.FirstOrDefault(x => x.Id == id);
        if (record is null)
            return Task.CompletedTask;
        record.UpdateAt = DateTime.UtcNow;
        record.Status = status;
        record.Error = error;
        return Task.CompletedTask;
    }

    public Task<List<InboxRecord>> GetUnprocessed()
    {
        var processables = reactionRecords.Where(x => x.Status == InboxRecordStatus.Scheduled).ToList();
        return Task.FromResult(processables);
    }
}
