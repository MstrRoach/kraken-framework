using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    /// <summary>
    /// Define el contrato para un contexto de bandeja de
    /// salida transsaccional. Esto para concentrar los eventos
    /// de dominio que deben de ser distribuidos solo cuando 
    /// la transaccion ha finalizado
    /// </summary>
    public interface IOutboxContext
    {
        /// <summary>
        /// Id de la transaccion que engloba
        /// </summary>
        Guid TransactionId { get; }

        /// <summary>
        /// Indica el modulo al cual pertenece la
        /// transaccion
        /// </summary>
        string Module { get; }

        /// <summary>
        /// Lista de solo lectura de los eventos de dominio
        /// </summary>
        IReadOnlyCollection<ProcessMessage> Events { get; }

        /// <summary>
        /// Almacena un evento de dominio dentro del
        /// contexto de la bandeja de salida
        /// </summary>
        /// <param name="event"></param>
        void AddProcessMessage(ProcessMessage @event);

        /// <summary>
        /// Limpia la lista de eventos
        /// </summary>
        void Cleanup();
    }
}
