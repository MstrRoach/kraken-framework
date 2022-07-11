using Kraken.Core.Commands;
using Kraken.Core.Events;
using Kraken.Core.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.App.Account;

/// <summary>
/// Datos para registrar al usuario
/// </summary>
public class CreateAccountCommand : ICommand<AccountCreated>
{
    public string Email { get; set; }
    public string Password { get; set; }

}

internal class CreateAccountHandler : ICommandHandler<CreateAccountCommand, AccountCreated>
{
    /// <summary>
    /// Bus de eventos en memoria para la administracion de notificaciones
    /// </summary>
    private readonly IMediator _mediator;

    public CreateAccountHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Realiza el registro de una cuenta en el sistema
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AccountCreated> Handle(CreateAccountCommand command, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        var accountId = Guid.NewGuid();
        await _mediator.Publish(new AccountCreatedSuccessfull { AccountId = accountId });
        return new AccountCreated
        {
            AccountId = accountId
        };
    }
}

/// <summary>
/// Indica que el comando fue ejecutado con exito
/// </summary>
public class AccountCreated
{
    public Guid AccountId { get; set; }
}

public class AccountCreatedSuccessfull : IDomainEvent
{
    public Guid AccountId { get; set; }

    public Guid Id { get; } = Guid.NewGuid();
}