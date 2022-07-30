using Kraken.Core.Internal.EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.App.Account;

internal class SendWelcomeEmailHandler : IDomainEventHandler<AccountCreatedSuccessfull>
{
    public Task Handle(AccountCreatedSuccessfull notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("=============== SendWelcomeEmailHandler Evento de dominio consumido consumido ==============");
        return Task.CompletedTask;
    }
}

internal class NormalNotificationHandler : IDomainEventHandler<AccountCreatedSuccessfull>
{
    public Task Handle(AccountCreatedSuccessfull notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("=============== NormalNotificationHandler Evento de dominio consumido ==============");
        return Task.CompletedTask;
    
    }
}
