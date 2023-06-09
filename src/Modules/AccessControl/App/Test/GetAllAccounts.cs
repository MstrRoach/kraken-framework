using AccessControl.Domain.Aggregates.AccountAggregate;
using Kraken.Domain.Storage;
using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.App.Test;

public class GetAllAccounts : IQuery<List<SummaryAccount>>
{
}

internal class GetAllAccountsHandler : IQueryHandler<GetAllAccounts, List<SummaryAccount>>
{
    readonly IRepository<Account> _repository;
    public GetAllAccountsHandler(IRepository<Account> repository)
    {
        _repository = repository;
    }

    public async Task<List<SummaryAccount>> Handle(GetAllAccounts request, CancellationToken cancellationToken)
    {
        var accounts = await _repository.GetAll(AccountSpecification.GetAll());
        return accounts.Select(x => new SummaryAccount(x.Id, x.Name)).ToList();
    }
}

public record SummaryAccount(Guid Id, string Name);