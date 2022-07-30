using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox
{
    public interface IOutboxContextRegistry
    {
        /// <summary>
        /// Agrega una nueva transaccion al registro de contextos vivos
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="context"></param>
        void SetTransaction(IOutboxContext context);
    }
}
