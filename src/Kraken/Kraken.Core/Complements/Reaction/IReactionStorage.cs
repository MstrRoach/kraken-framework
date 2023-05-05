using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Complements.Reaction
{
    /// <summary>
    /// Define las operaciones para el almacenaje de los
    /// registros de reacciones, para permitir el control
    /// sobre aquellas reacciones que no se ejecutan de forma
    /// correcta
    /// </summary>
    public interface IReactionStorage
    {
        /// <summary>
        /// Almacena el registro utilizando una implementacion del cliente
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task SaveAsync(StorageRecord record);

        /// <summary>
        /// Marca un registro como completado
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task MarkAsDone(Guid guid);

        /// <summary>
        /// Obtiene la lista de registros que no se han ejecutado
        /// </summary>
        /// <returns></returns>
        Task<List<StorageRecord>> GetUnprocessedRecords();
    }
}
