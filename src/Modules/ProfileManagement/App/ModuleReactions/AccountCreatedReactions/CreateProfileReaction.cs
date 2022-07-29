using Kraken.Core.Mediator;
using MediatR;
using ModuleEvents.IdentityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileManagement.App.ModuleReactions.AccountCreatedReactions
{
    public class CreateProfileReaction : IModuleEventHandler<AccountCreatedEvent>
    {
        public Task Handle(AccountCreatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("=============== Evento modular consumido ==============");
            return Task.CompletedTask;
        }
    }
}
