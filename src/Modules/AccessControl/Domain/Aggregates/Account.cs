using AccessControl.Domain.Aggregates.Events;
using Kraken.Domain.Core;
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

    public static Account Create(string Name)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = Name
        };
        account.AddDomainEvent(new AccountCreated
        {
            AccountId = account.Id,
            Name = account.Name
        });
        return account;
    }

    private Account() { }

}
