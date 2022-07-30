using Kraken.Core.Internal.EventBus;
using Kraken.Core.Internal.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal.Transaction
{
    internal class DefaultUnitWork : Core.Internal.Transaction.UnitWork
    {
        /// <summary>
        /// Logger para la unidad de trabajo por defecto
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Id de la transaccion
        /// </summary>
        private Guid _transactionId;

        /// <summary>
        /// Accessor para el id de transaccion
        /// </summary>
        public override Guid TransactionId => _transactionId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public DefaultUnitWork(ILogger<DefaultUnitWork> logger, IEventBus eventBus)
            : base(eventBus)
        {
            _logger = logger;
        }

        /// <summary>
        /// Para iniciar la transaccion solo vamos a crear un guid
        /// </summary>
        public override void StartTransaction()
        {
            // Solo creamos el id
            this._transactionId = Guid.NewGuid();
        }

        /// <summary>
        /// El commmit no applica por que no tenemos nada que confirmar
        /// </summary>
        /// <returns></returns>
        public override Task Commit()
        {
            // No hacemos nadda
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tammpoc hay como revertir las operaciones
        /// </summary>
        /// <returns></returns>
        public override Task Rollback()
        {
            // No hacemmos nada
            return Task.CompletedTask;
        }

        
    }
}
