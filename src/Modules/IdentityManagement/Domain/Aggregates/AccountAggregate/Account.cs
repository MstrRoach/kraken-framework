using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Domain.Aggregates.AccountAggregate
{
    internal class Account
    {
        /// <summary>
        /// Nombre de la cuenta
        /// </summary>
        public string Name { get; private set; }
    }
}
