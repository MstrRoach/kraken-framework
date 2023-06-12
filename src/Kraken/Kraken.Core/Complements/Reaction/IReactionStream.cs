using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Complements.Reaction
{
    public interface IReactionStream
    {
        /// <summary>
        /// Proceso y guarda la reaccion envuelta en el objeto 
        /// de registro de proceso
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task SaveAsync(ProcessRecord record);

        /// <summary>
        /// Marca una reaccion como lista para no procesarlas de nuevo
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task MarkReactionAsDone(Guid guid);

        /// <summary>
        /// Obtiene todas las reacciones no procesadas
        /// </summary>
        /// <returns></returns>
        Task<List<ProcessRecord>> GetUnprocessedRecords();
    }
}
