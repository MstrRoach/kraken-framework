using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Complements.Outbox
{
    /// <summary>
    /// Define un mensaje utilizado para el procesamiento
    /// de los eventos que envuelven. Esto, para conservar y transaportar
    /// detalles del contexto sin la necesidad de extrapolar un mensaje de
    /// almacenamiento fuera de su campo de accion
    /// </summary>
    public class ProcessMessage
    {
        /// <summary>
        /// Id del mensaje
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Id de correlacion del evento
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Usuario que desencadeno el evento
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Id de trazabilidad
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// Tipo del evento
        /// </summary>
        public object Event { get; set; }

    }
}
