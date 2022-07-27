using Kraken.Core.Mediator;
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
        return Task.CompletedTask;
    }
}

internal class NormalNotificationHandler : IDomainEventHandler<AccountCreatedSuccessfull>
{
    public Task Handle(AccountCreatedSuccessfull notification, CancellationToken cancellationToken)
    {
        Console.WriteLine(notification.AccountId);
        return Task.CompletedTask;
    
    }
}
