﻿using Kraken.Core.Internal.Domain;
using Kraken.Core.Internal.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Domain.Aggregates.AccountAggregate;

public class Account : Aggregate<Account,Guid>
{
    /// <summary>
    /// Nombre de la cuenta
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Constructor de una cuenta
    /// </summary>
    /// <param name="name"></param>
    public Account(string name)
    {
        this.Id = Guid.NewGuid();
        Name = name;
        this.AddDomainEvent(new AccountCreated { 
            AccountId = this.Id,
            Name = this.Name
        });
    }
}

public record AccountCreated : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid AccountId { get; set; }

    public string Name { get; set;}
}
