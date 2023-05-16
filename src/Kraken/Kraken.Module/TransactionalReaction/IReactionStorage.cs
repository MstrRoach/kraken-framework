using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.TransactionalReaction;

public interface IReactionStorage
{
    /// <summary>
    /// Guarda todas las reacciones de una sola vez
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    Task SaveAll(List<ReactionRecord> records);

    /// <summary>
    /// Actualiza un registro sin recuperarlo
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <param name="sentAt"></param>
    /// <param name="notes"></param>
    /// <returns></returns>
    Task Update(Guid id, ReactionRecordStatus status, DateTime? sentAt = null, string? notes = null);
}
