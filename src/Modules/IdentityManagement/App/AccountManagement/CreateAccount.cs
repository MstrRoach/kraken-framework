using IdentityManagement.Domain.Aggregates.AccountAggregate;
using IdentityManagement.Infrastructure.Services.KrakenServices;
using IdentityManagement.Persistence.Repositories;
using Kraken.Core;
using Kraken.Core.Internal.EventBus;
using Kraken.Core.Internal.Mediator;
using Kraken.Core.Internal.Storage;
using Kraken.Core.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ModuleEvents.IdentityManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.App.AccountManagement;

/// <summary>
/// Datos para registrar al usuario
/// </summary>
public class CreateAccountCommand : ICommand<AccountCreatedSuccessfull>
{
    public string Email { get; set; }
    public string Password { get; set; }

}

internal class CreateAccountHandler : ICommandHandler<CreateAccountCommand, AccountCreatedSuccessfull>
{
    /// <summary>
    /// Bus de eventos en memoria para la administracion de notificaciones
    /// </summary>
    private readonly IEventBus _eventBus;

    private readonly IRepository<Account> _repository;

    public CreateAccountHandler(IEventBus eventBus,
        IRepository<Account> repository)
    {
        _eventBus = eventBus;
        _repository = repository;
    }

    /// <summary>
    /// Realiza el registro de una cuenta en el sistema
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AccountCreatedSuccessfull> Handle(CreateAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = new Account("Jesus Antonio");
        await _repository.Create(account);
        var accountById = await _repository.Get(new AccountByIdAndName(account.Id, "Jesus"));

        //var accountId = Guid.NewGuid();
        //await _mediator.Publish(new NormalNotification { Message = "Prieba" });
        //await _eventBus.Publish(new AccountCreatedSuccessfull { AccountId = accountId });
        //await _eventBus.Publish(new AccountCreatedEvent { AccountId = accountId, Name = "Jesus Antonio" });

        //await _mediator.Send(new AccountCreatedSuccessfull { AccountId = accountId });
        return new AccountCreatedSuccessfull
        {
            AccountId = account.Id
        };
    }
}

public class AccountCreatedSuccessfull
{
    public Guid AccountId { get; set; }
}

public class NormalNotification : IModuleEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Modulo desde donde se lanza el evento
    /// </summary>
    public string Module { get; }

    /// <summary>
    /// Mensaje del evento
    /// </summary>
    public string Message { get; set; }

    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}