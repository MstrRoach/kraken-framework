using Kraken.Core.Internal.EventBus;
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

        /// <summary>
        /// Fecha en la que ocurre el evento
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
