using Kraken.Module.Reaction;
using Kraken.Module.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Reaction;

internal class DefaultReactionStorage<TModule> : IReactionStorage<TModule>
    where TModule : IModule
{
    private static ConcurrentBag<ReactionRecord> reactionRecords = new ConcurrentBag<ReactionRecord>();

    public Task Save(ReactionRecord record)
    {
        if (!reactionRecords.Any(x => x.Id == record.Id)) 
            reactionRecords.Add(record);
        return Task.CompletedTask;
    }

    public Task SaveAll(List<ReactionRecord> records)
    {
        foreach (var record in records)
        {
            Save(record);
        }
        return Task.CompletedTask;
    }

    public Task Update(Guid id, ReactionRecordStatus status, string error = null)
    {
        var record = reactionRecords.FirstOrDefault(x => x.Id == id);
        if(record is null)
            return Task.CompletedTask;
        record.UpdateAt = DateTime.UtcNow; 
        record.Status = status;
        record.Error = error;
        return Task.CompletedTask;
    }

    public Task<List<ReactionRecord>> GetUnprocessed()
    {
        var processables = reactionRecords.Where(x => x.Status == ReactionRecordStatus.Scheduled).ToList();
        return Task.FromResult(processables);
    }

}
