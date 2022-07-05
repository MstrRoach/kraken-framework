using Kraken.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Events
{
    internal class DefaultEventDispatcher : IEventDispatcher
    {



        /// <summary>
        /// Inicia el procesamiento de la operacion
        /// </summary>
        /// <param name="stoppingToken"></param>
        public void Start(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Agrega un evento a la lista de espera
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task AddToWaitingList(IKrakenEvent @event)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Operacion realizada cuando una transaccion se ha completado 
        /// exitosamente
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public Task TransactionCompleted(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operacion realizada cuando una transaccion ha fallado
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public Task TransactionFailed(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Libera los recursos al terminar la operacion
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
