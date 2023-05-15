using Kraken.Module.Inbox;
using Kraken.Module.TransactionalOutbox;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

/// <summary>
/// Implementacion del almacenamiento por defecto
/// para los registros de bandeja de salida
/// </summary>
internal class DefaultOutboxStorage : IOutboxStorage
{
    private static ConcurrentDictionary<Guid,OutboxRecord> outboxRecords = new ConcurrentDictionary<Guid, OutboxRecord>();

    public Task Save(OutboxRecord record)
    {
        if (!outboxRecords.ContainsKey(record.Id))
            outboxRecords.TryAdd(record.Id, record);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<OutboxRecord>> ConfirmTransaction(Guid transaction)
    {
        var processables = outboxRecords
            .Where(x => x.Value.TransactionId == transaction)
            .Select(x => x.Value).ToList();
        processables.ForEach(x =>
        {
            x.
        })
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

    public async Task DeleteAll(Guid transaction)
    {
        foreach (var item  in await GetAll(transaction))
        {
            outboxRecords.TryRemove(item.Id, out _);
        }
    }

    
}
