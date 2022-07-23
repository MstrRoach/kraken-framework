using Kraken.Core.Mediator;
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
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Almacena un evento de dominio dentro del
        /// contexto de la bandeja de salida
        /// </summary>
        /// <param name="event"></param>
        void AddDomainEvent(IDomainEvent @event);

        /// <summary>
        /// Limpia la lista de eventos
        /// </summary>
        void Cleanup();
    }
}
