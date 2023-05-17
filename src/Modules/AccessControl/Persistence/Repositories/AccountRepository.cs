using AccessControl.Domain;
using AccessControl.Domain.Aggregates;
using Kraken.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Persistence.Repositories;

internal class AccountRepository : IRepository<Account, MongoSpecification<Account>>
{
    public Task Create(Account aggregate)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Account aggregate)
    {
        throw new NotImplementedException();
    }

    public Task<Account> Get(MongoSpecification<Account> specification)
    {
        throw new NotImplementedException();
    }

    public Task<List<Account>> GetAll(MongoSpecification<Account> specification)
    {
        throw new NotImplementedException();
    }

    public Task Update(Account aggregate)
    {
        throw new NotImplementedException();
    }
}
