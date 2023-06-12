using Dottex.Kalypso.Module.Request.Mediator;

namespace AccessControl.Domain.Aggregates.AccountAggregate.Events;

public record AccountCreated : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid AccountId { get; set; }

    public string Name { get; set; }
}
