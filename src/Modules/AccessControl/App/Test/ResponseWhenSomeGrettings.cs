using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

internal class ResponseWhenSomeGrettings : IDomainEventHandler<TestDomainEvent>
{
    public Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        var response = "Hello there";
        return Task.CompletedTask;
    }
}
