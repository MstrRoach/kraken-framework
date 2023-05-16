using Kraken.Module.TransactionalReaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalReaction;

/// <summary>
/// Componente encargado de almacenar las reacciones
/// utilizando una immplementacion por defecto
/// </summary>
internal class DefaultReactionStorage : IReactionStorage
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
}
