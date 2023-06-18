using Dottex.Kalypso.Module.TransactionalReaction;
using System.Collections.Concurrent;

namespace KrakenThreeApi.CommonServices;

public class InMemoryReactionStorage : IReactionStorage
{
    private static ConcurrentDictionary<Guid, ReactionRecord> reactionRecords = new ConcurrentDictionary<Guid, ReactionRecord>();

    public Task Save(ReactionRecord record)
    {
        if (!reactionRecords.ContainsKey(record.Id))
            reactionRecords.TryAdd(record.Id, record);
        return Task.CompletedTask;
    }

    public async Task SaveAll(List<ReactionRecord> records)
    {
        foreach (var record in records)
        {
            await Save(record);
        }
    }

    public Task Update(Guid id, ReactionRecordStatus status, DateTime? sentAt = null, string? notes = null)
    {
        if (!reactionRecords.ContainsKey(id))
            return Task.CompletedTask;
        reactionRecords[id] = reactionRecords[id] with
        {
            SentAt = sentAt,
            LastUpdatedAt = DateTime.UtcNow,
            Status = status,
            Notes = notes
        };
        return Task.CompletedTask;
    }
}
