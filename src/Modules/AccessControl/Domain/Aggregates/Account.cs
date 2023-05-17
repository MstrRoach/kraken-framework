using AccessControl.Domain.Aggregates.Events;
using Kraken.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Domain.Aggregates;

public class Account : Aggregate<Account, Guid>
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
        this.AddDomainEvent(new AccountCreated
        {
            AccountId = this.Id,
            Name = Name
        });
    }
}
