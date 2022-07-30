using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    /// <summary>
    /// Define el procesamiento de los eventos, de forma
    /// definida por quien implementa la operacion
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Processa un mensaje y los distribuye para realizar las operaciones
        /// pendientes para el evento
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ProcessAsync(ProcessMessage message, CancellationToken cancellationToken = default);
    }
}