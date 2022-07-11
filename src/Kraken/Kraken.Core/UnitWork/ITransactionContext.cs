using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.UnitWork
{
    /// <summary>
    /// Contiene la informacion de la transaccion actual 
    /// </summary>
    public interface ITransactionContext
    {
        /// <summary>
        /// Contiene el id de la transaccion
        /// </summary>
        Guid TransactionId { get; }

        /// <summary>
        /// Indica el modulo al cual pertenece la
        /// transaccion
        /// </summary>
        string Module { get; }


    }
}
