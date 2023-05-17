
using Kraken.Module.Request.Mediator;

namespace AccessControl.Domain.Aggregates.Events;

public record AccountCreated : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid AccountId { get; set; }

    public string Name { get; set; }
}
