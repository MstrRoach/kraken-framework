using IdentityManagement.Domain.Aggregates.AccountAggregate.Events;
using Kraken.Core.EventBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.App.AccountManagement.AccountCreatedReactions;

internal class UpdateAccountStatusNotificationHandler : IDomainEventHandler<AccountCreated>
{
    public Task Handle(AccountCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("=============== NormalNotificationHandler Evento de dominio consumido ==============");
        return Task.CompletedTask;

    }
}
