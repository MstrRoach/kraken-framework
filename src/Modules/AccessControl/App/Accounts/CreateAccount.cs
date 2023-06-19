using AccessControl.Domain.Aggregates.AccountAggregate;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Accounts;

public class CreateAccountCommand : ICommand<AccountCreated>
{
    public string Name { get; set; }
    public string Email { get; set; }
}

internal class CreateAccountHandler : ICommandHandler<CreateAccountCommand, AccountCreated>
{
    readonly IRepository<Account> _accountRepository;
    public CreateAccountHandler(IRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }
    public async Task<AccountCreated> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = Account.Create(request.Name, request.Email);
        await _accountRepository.Create(account);
        return new AccountCreated
        {

        };
    }
}

public class AccountCreated
{

}