using AccessControl.Domain.Aggregates.Events;
using Kraken.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Domain.Aggregates;

public class Account : Aggregate<Guid>
{
    /// <summary>
    /// Nombre de la cuenta
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Correo del usuario
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Fecha de creacion del registro 
    /// </summary>
    //public DateTime CreatedAt { get; private set; }

    public static Account Create(string name, string email)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = Email.Create(email),
            //CreatedAt = DateTime.UtcNow,
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

public record Email
{
    /// <summary>
    /// Indica la direccion del correo electronico
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// Almacena el correo normalizado en mayusculas
    /// </summary>
    public string Normalized { get; private set; }

    public static Email Create(string address)
    {
        return new Email
        {
            Address = address,
            Normalized = address.ToUpper()
        };
    }

    /// <summary>
    /// Constructor privado vacio
    /// </summary>
    private Email() { }
}
