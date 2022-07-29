using IdentityManagement.Domain.Aggregates.AccountAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Persistence.Repositories
{
    internal class AccountRepository : IAccountRepository
    {

        public Task Create(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
