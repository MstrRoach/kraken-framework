using AccessControl.Domain.Aggregates.AccountAggregate;
using AccessControl.Domain.Aggregates.ProfileAggregate;
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
    readonly IRepository<Profile> _profileRepository;
    public GetAllAccountsHandler(IRepository<Account> repository,
        IRepository<Profile> profileRepository)
    {
        _repository = repository;
        _profileRepository = profileRepository;
    }

    public async Task<List<SummaryAccount>> Handle(GetAllAccounts request, CancellationToken cancellationToken)
    {
        var accounts = await _repository.GetAll(AccountSpecification.GetAll());
        var profiles = await _profileRepository.GetAll(ProfileSpecification.GetAll());
        return accounts.Select(x => new SummaryAccount(x.Id, x.Name)).ToList();
    }
}

public record SummaryAccount(Guid Id, string Name);