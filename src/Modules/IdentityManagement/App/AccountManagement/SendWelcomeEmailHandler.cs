using IdentityManagement.Domain.Aggregates.AccountAggregate;
using Kraken.Core.Internal.EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.App.AccountManagement;

internal class SendWelcomeEmailHandler : IDomainEventHandler<AccountCreated>
{
    public Task Handle(AccountCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("=============== SendWelcomeEmailHandler Evento de dominio consumido consumido ==============");
        return Task.CompletedTask;
    }
}

internal class NormalNotificationHandler : IDomainEventHandler<AccountCreated>
{
    public Task Handle(AccountCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("=============== NormalNotificationHandler Evento de dominio consumido ==============");
        return Task.CompletedTask;

    }
}
