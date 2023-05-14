using Kraken.Module.Request.Mediator;
using Kraken.Module.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

public class SayHelloWorldCommand : ContextCommand<HelloWorldSaid>
{
}

public class SayHelloWorldHandler : ICommandHandler<SayHelloWorldCommand, HelloWorldSaid>
{
    private readonly IEventPublisher _publisher;
    public SayHelloWorldHandler(IEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<HelloWorldSaid> Handle(SayHelloWorldCommand request, CancellationToken cancellationToken)
    {
        var name = request.Context?.Identity?.Name;
        await _publisher.Publish(new TestDomainEvent("Hola mundo"));
        await Task.CompletedTask;
        return new HelloWorldSaid
        {
            Message = $"Hola {name ?? "desconocido"}"
        };
    }
}

public record TestDomainEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

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
