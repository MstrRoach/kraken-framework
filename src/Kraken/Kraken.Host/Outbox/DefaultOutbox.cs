using Kraken.Core.Mediator;
using Kraken.Core.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    /// <summary>
    /// Laa bandeja de salida estara disponible a nivel
    /// de alcance, para poder tener el almacenamiento
    /// conectado a la misma transaccion en caso de estarse
    /// ejecutando una.
    /// </summary>
    internal class DefaultOutbox<TStore> : IOutbox
        where TStore : IOutboxStore
    {
        /// <summary>
        /// Contiene el contenedor 
        /// </summary>
        private readonly TStore _outboxStore;

        public DefaultOutbox(TStore outboxStore)
        {
            _outboxStore = outboxStore;
        }

        public Task SaveAsync<T>(T messages) where T : IDomainEvent
        {
            throw new NotImplementedException();
        }

        public Task PublishUnsentAsync()
        {
            throw new NotImplementedException();
        }

        public Task CleanupAsync(DateTime? to = null)
        {
            throw new NotImplementedException();
        }


    }
}
