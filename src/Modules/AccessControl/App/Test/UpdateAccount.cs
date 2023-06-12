using AccessControl.Domain.Aggregates.AccountAggregate;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

public class UpdateAccountCommand : ICommand<AccountUpdated>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

internal class UpdateAccountHandler : ICommandHandler<UpdateAccountCommand, AccountUpdated>
{
    readonly IRepository<Account> _repository;
    public UpdateAccountHandler(IRepository<Account> repository)
    {
        _repository = repository;
    }
    public async Task<AccountUpdated> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var storedAccount = await _repository.Get(AccountSpecification.GetById(request.Id));
        var email = Email.Create(request.Email);
        storedAccount.Update(request.Name, email);
        await _repository.Update(storedAccount);
        return new AccountUpdated { };
    }
}

public class AccountUpdated
{

}