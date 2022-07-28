using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Reaction
{
    /// <summary>
    /// clase que contiene la reaccion especificada adecuadamente para 
    /// el procesamiento y ejecucion de la misma
    /// </summary>
    public class ProcessRecord
    {
        /// <summary>
        /// Id de la reaccion que debe de ser unica
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Id de correlacion para la reaccion
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Id del evento que generó la reaccion
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Indica cual es el evento asociado a la reaccion
        /// </summary>
        public Type Event { get; set; }

        /// <summary>
        /// Objeto que contiene la reaccion que debe de ejecutarse
        /// </summary>
        public Type Reaction { get; set; }
    }
}
