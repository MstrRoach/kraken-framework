using AccessControl.Domain.Aggregates.AccountAggregate.Events;
using Dottex.Kalypso.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

internal class ResponseWhenSomeGrettings : IDomainEventHandler<AccountCreated>
{
    public async Task Handle(AccountCreated notification, CancellationToken cancellationToken)
    {
        var response = $"Hello {notification.Name}";
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine(response);
    }
}
