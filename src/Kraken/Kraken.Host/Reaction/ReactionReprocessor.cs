using Kraken.Core.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    /// <summary>
    /// Componente encargado de reprocesar los mensajes que cayeron en un error
    /// </summary>
    internal class ReactionReprocessor : IProcessor
    {
        /// <summary>
        /// Proceso prin
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task ProcessAsync(ProcessingContext context)
        {
            throw new Exception("Error de prueba");
        }
    }
}
