using Kraken.Core.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    /// <summary>
    /// Commponente dedicado a la distribucion de los eventos en un hilo 
    /// de ejecucion y en orden de creacion y llegada, utilizando para
    /// esto, un transporte especificado, segun se activen o no los despachadores
    /// de eventos locales o las reacciones
    /// </summary>
    public interface IEventDispatcher : IProcessingServer
    {
        /// <summary>
        /// Agrega a la cola de ejecucion interna del evento.
        /// </summary>
        /// <param name="message"></param>
        void EnqueueToExecute(ProcessMessage message);

    }
}
