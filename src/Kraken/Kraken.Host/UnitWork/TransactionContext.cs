using Kraken.Core.Mediator;
using Kraken.Core.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.UnitWork
{
    internal class TransactionContext : ITransactionContext
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
        private List<IDomainEvent> DomainEvents = new List<IDomainEvent>();



    }
}
