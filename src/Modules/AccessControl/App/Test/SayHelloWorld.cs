using Dottex.Kalypso.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dottex.Kalypso.Module.Context;
using AccessControl.Domain.Aggregates.AccountAggregate;
using AccessControl.Domain.Aggregates.ProfileAggregate;
using Dottex.Kalypso.Domain.Storage;

namespace AccessControl.App.Test;

public class SayHelloWorldCommand : ContextCommand<HelloWorldSaid>
{
}

public class SayHelloWorldHandler : ICommandHandler<SayHelloWorldCommand, HelloWorldSaid>
{
    private readonly IEventPublisher _publisher;
    private readonly IRepository<Profile> _repository;
    private readonly IRepository<Account> _accountRepository;
    public SayHelloWorldHandler(IEventPublisher publisher, IRepository<Profile> repository, IRepository<Account> accountRepository)
    {
        _publisher = publisher;
        _repository = repository;
        _accountRepository = accountRepository;
    }

    public async Task<HelloWorldSaid> Handle(SayHelloWorldCommand request, CancellationToken cancellationToken)
    {
        var account = Account.Create("Chuy", "imct.jesus.antonio@gmail.com");
        var profile = Profile.Create("Jesus Antonio", new DateTime(1995,06,09));
        //await _repository.Create(profile);
        await _accountRepository.Create(account);
        var name = request.Context?.Identity?.Name;
        await _publisher.Publish(new TestDomainEvent("Hola mundo"));
        await Task.CompletedTask;
        //var accountStored = await _repository.Get(AccountSpecification.GetById(account.Id));
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
