using AccessControl.Domain.Aggregates.AccountAggregate;
using Kraken.Domain.Storage;
using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

public class DeleteAccountCommand : ICommand<AccountDeleted>
{
    public Guid Id { get; set; }
}

internal class DeleteAccountHandler : ICommandHandler<DeleteAccountCommand, AccountDeleted>
{
    readonly IRepository<Account> _repository;
    public DeleteAccountHandler(IRepository<Account> repository)
    {
        _repository = repository;
    }
    public async Task<AccountDeleted> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var storedAccount = await _repository.Get(AccountSpecification.GetById(request.Id));
        await _repository.Delete(storedAccount);
        return new AccountDeleted { };
    }
}

public class AccountDeleted
{

}
