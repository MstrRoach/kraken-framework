using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    public interface IOutboxStorage
    {
        /// <summary>
        /// Almacena un mensaje en el store de base de datos
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SaveAsync(StorageMessage message);

        /// <summary>
        /// Obtiene los mensajes no enviados
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<StorageMessage>> GetUnsentAsync();

        /// <summary>
        /// Obtiene un evento por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StorageMessage> Get(Guid id);

        /// <summary>
        /// Realiza una limpieza de los eventos processados
        /// </summary>
        /// <returns></returns>
        Task Cleanup();
    }
}
