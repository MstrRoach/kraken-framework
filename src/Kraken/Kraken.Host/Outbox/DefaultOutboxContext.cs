using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    internal class DefaultOutboxContext : IOutboxContext
    {
        /// <summary>
        /// Id de la transaccion del contexto
        /// </summary>
        public Guid TransactionId { get; } = Guid.Empty;

        /// <summary>
        /// Indica el modulo al cual pertenece la transaccion. 
        /// Solo puede existir una transaccion por modulo.
        /// </summary>
        public string Module { get; } = "";

        /// <summary>
        /// Lista de eventos de dominio que deben de enviarse a procesamiento
        /// cuando se confirme la transaccion
        /// </summary>
        private List<IDomainEvent> domainEvents = new List<IDomainEvent>();

        /// <summary>
        /// Lista de solo lectura para obtener los eventos de domminio de
        /// la transaccion actual
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

        public DefaultOutboxContext(Guid transactionId)
        {
            this.TransactionId = transactionId;
        }

        /// <summary>
        /// Agrega los eventos a la lista de eventos
        /// </summary>
        /// <param name="event"></param>
        public void AddDomainEvent(IDomainEvent @event)
        {
            if (@event is null)
                return;
            domainEvents.Add(@event);
        }

        /// <summary>
        /// Limpia la lista de eventos
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Cleanup()
        {
            this.domainEvents.Clear();
        }
    }
}
