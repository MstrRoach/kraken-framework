using IdentityManagement.Domain.Aggregates.AccountAggregate.Events;
using Kraken.Core.EventBus;

namespace IdentityManagement.App.AccountManagement.AccountCreatedReactions;

internal class SendWelcomeEmailHandler : IDomainEventHandler<AccountCreated>
{
    public Task Handle(AccountCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("=============== SendWelcomeEmailHandler Evento de dominio consumido consumido ==============");
        return Task.CompletedTask;
    }
}
