using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Reaction
{
    /// <summary>
    /// Define el contrato para enviar las reacciones ejecutadas
    /// y controlar su estado, y se encarga de dirigir cada una de 
    /// las reacciones al modulo al que pertenecen
    /// </summary>
    public interface IReactionBroker
    {
        /// <summary>
        /// Almacena el registro de reaccion en el storage
        /// especifico, calculando el storage a partir del tipo
        /// </summary>
        /// <param name="processRecord"></param>
        /// <returns></returns>
        Task SaveAsync(ProcessRecord processRecord);

        /// <summary>
        /// Actualiza el registro para marcarlo como procesado
        /// </summary>
        /// <param name="processRecord"></param>
        /// <returns></returns>
        Task UpdateToDone(Guid reactionId);
    }
}
