using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Domain.Aggregates.AccountAggregate
{
    internal interface IAccountRepository
    {
        Task Create(Account account);
    }
}
