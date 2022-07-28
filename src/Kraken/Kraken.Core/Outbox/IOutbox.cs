using Kraken.Core.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    public interface IOutbox
    {
        /// <summary>
        /// Guarda un mensaje en la base de datos
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        Task SaveAsync(ProcessMessage messages);

        /// <summary>
        /// Publica los mensajes que no han sido publicados
        /// </summary>
        /// <returns></returns>
        Task PublishUnsentAsync();

        /// <summary>
        /// Limpia la bandeja de salida para tener mas 
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        Task CleanupAsync(DateTime? to = null);

        /// <summary>
        /// Obtiene un mensaje de processamiento que coincida con el
        /// id especificado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProcessMessage> Get(Guid id);
    }
}
