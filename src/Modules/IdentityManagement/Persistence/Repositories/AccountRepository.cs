using IdentityManagement.Domain.Aggregates.AccountAggregate;
using Kraken.Core.Internal.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Persistence.Repositories
{
    public class AccountRepository : IRepository<Account>
    {
        private static ConcurrentDictionary<Guid,Account> Accounts = new ConcurrentDictionary<Guid, Account>();

        public Task Create(Account account)
        {
            Accounts[account.Id] = account;
            return Task.CompletedTask;
        }

        public Task Delete(Account account)
        {
            Accounts.TryRemove(account.Id, out _);
            return Task.CompletedTask;
        }

        public Task<Account> Get(ISpecification<Account> specification)
        {
            var account = Accounts.Values.Where(specification.IsSatisfiedBy).FirstOrDefault();
            return Task.FromResult(account);
        }

        public Task<List<Account>> GetAll(ISpecification<Account> specification)
        {
            return Task.FromResult(Accounts.Values.Where(specification.IsSatisfiedBy).ToList());
        }

        public Task Update(Account account)
        {
            Accounts[account.Id] = account;
            return Task.CompletedTask;
        }
    }

    public class AccountById : Specification<Account>
    {
        public AccountById(Guid accountId)
            :base(x => x.Id == accountId)
        {

        }
    }

    public class AccountByIdAndName : Specification<Account>
    {
        public AccountByIdAndName(Guid accountId, string name) : 
            base(x => x.Id == accountId && x.Name.Contains(name))
        {
        }
    }
}
