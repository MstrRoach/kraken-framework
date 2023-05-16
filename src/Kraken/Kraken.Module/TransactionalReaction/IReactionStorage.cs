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
}
