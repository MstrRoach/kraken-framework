using Kraken.Core.Internal.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleEvents.IdentityManagement
{
    public class AccountCreatedEvent : IModuleEvent
    {
        /// <summary>
        /// Id del evento
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Id de la cuenta creada
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// Nombre de la cuenta
        /// </summary>
        public string Name { get; set; }

    }
}
