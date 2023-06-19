using Dottex.Kalypso.Module.TransactionalOutbox;
using System.Collections.Concurrent;

namespace KrakenThreeApi.CommonServices;

public class InMemoryOutboxStorage : IOutboxStorage
{
    private static ConcurrentDictionary<Guid, OutboxRecord> outboxRecords = new ConcurrentDictionary<Guid, OutboxRecord>();

    public Task Save(OutboxRecord record)
    {
        if (!outboxRecords.ContainsKey(record.Id))
            outboxRecords.TryAdd(record.Id, record);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<OutboxRecord>> GetAll(Guid transaction)
    {
        var processables = outboxRecords
            .Where(x => x.Value.TransactionId == transaction)
            .Select(x => x.Value);
        return Task.FromResult(processables);
    }

    public Task Update(OutboxRecord record)
    {
        if (!outboxRecords.ContainsKey(record.Id))
        {
            outboxRecords.TryAdd(record.Id, record);
            return Task.CompletedTask;
        }
        outboxRecords[record.Id] = record;
        return Task.CompletedTask;
    }

    public Task Update(Guid id, OutboxRecordStatus status, DateTime? sentAt = null, string? notes = null)
    {
        if (!outboxRecords.ContainsKey(id))
            return Task.CompletedTask;
        outboxRecords[id] = outboxRecords[id] with
        {
            SentAt = sentAt,
            LastUpdatedAt = DateTime.UtcNow,
            Status = Enum.GetName<OutboxRecordStatus>(status),
            Notes = notes
        };
        return Task.CompletedTask;
    }

    public async Task UpdateAll(IEnumerable<OutboxRecord> updatedEvents)
    {
        foreach (var record in updatedEvents)
        {
            await Update(record);
        }
    }

    public async Task DeleteAll(Guid transaction)
    {
        foreach (var item in await GetAll(transaction))
        {
            outboxRecords.TryRemove(item.Id, out _);
        }
    }

    public IEnumerable<OutboxRecord> GetBy(OutboxFilter filter)
    {
        throw new NotImplementedException();
    }
}
