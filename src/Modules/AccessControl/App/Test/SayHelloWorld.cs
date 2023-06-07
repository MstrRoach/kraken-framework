using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kraken.Module.Context;
using Kraken.Domain.Storage;
using AccessControl.Domain.Aggregates;

namespace AccessControl.App.Test;

public class SayHelloWorldCommand : ContextCommand<HelloWorldSaid>
{
}

public class SayHelloWorldHandler : ICommandHandler<SayHelloWorldCommand, HelloWorldSaid>
{
    private readonly IEventPublisher _publisher;
    private readonly IRepository<Account> _repository;
    public SayHelloWorldHandler(IEventPublisher publisher, IRepository<Account> repository)
    {
        _publisher = publisher;
        _repository = repository;
    }

    public async Task<HelloWorldSaid> Handle(SayHelloWorldCommand request, CancellationToken cancellationToken)
    {
        var account = Account.Create("Chuy");
        await _repository.Create(account);
        var name = request.Context?.Identity?.Name;
        await _publisher.Publish(new TestDomainEvent("Hola mundo"));
        await Task.CompletedTask;
        var accountStored = await _repository.Get(AccountSpecification.GetById(account.Id));
        return new HelloWorldSaid
        {
            Message = $"Hola {name ?? "desconocido"}"
        };
    }
}

public record TestDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public IContext Context { get; set; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string Message { get; set; }
    

    public TestDomainEvent(string message)
    {
        Message = message;
    }
}

public class HelloWorldSaid
{
    public string Message { get; set; }
}
