using IdentityManagement.Domain.Aggregates.AccountAggregate;
using IdentityManagement.Persistence.Repositories;
using Kraken.Core.EventBus;
using Kraken.Core.Mediator;
using Kraken.Core.Storage;
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
        if (await _repository.Exist(new AccountById(account.Id)))
            return new AccountCreatedSuccessfull();
        await _repository.Create(account);
        var elements = await _repository.Count();
        var accountById = await _repository.Get(new AccountByIdAndName(account.Id, "Jesus"));
        await _eventBus.Publish(new AccountCreatedEvent { AccountId = account.Id });
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