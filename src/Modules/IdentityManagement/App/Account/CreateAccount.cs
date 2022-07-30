﻿using IdentityManagement.Infrastructure.Services.KrakenServices;
using Kraken.Core;
using Kraken.Core.Internal.EventBus;
using Kraken.Core.Internal.Mediator;
using Kraken.Core.Outbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ModuleEvents.IdentityManagement;
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
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;

    public CreateAccountHandler(IEventBus eventBus, IServiceProvider serviceProvider/*, IRepository<Account> accountRepository*/)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
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
        //await _mediator.Publish(new NormalNotification { Message = "Prieba" });
        await _eventBus.Publish(new AccountCreatedSuccessfull { AccountId = accountId });
        await _eventBus.Publish(new AccountCreatedEvent { AccountId = accountId , Name = "Jesus Antonio" });

        //await _mediator.Send(new AccountCreatedSuccessfull { AccountId = accountId });
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

    public DateTime OccurredOn { get; } = DateTime.UtcNow;
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