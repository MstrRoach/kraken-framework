using IdentityManagement.Domain.Aggregates.AccountAggregate;
using Kraken.Core.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Persistence.Repositories
{
    /// <summary>
    /// Repositorio para las cuentas
    /// </summary>
    public class AccountRepository : IRepository<Account>
    {
        private static ConcurrentDictionary<Guid,Account> Accounts = new ConcurrentDictionary<Guid, Account>();

        /// <summary>
        /// Crea una cuenta en el repositorio
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task Create(Account account)
        {
            Accounts[account.Id] = account;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Elimina una cuenta del repositorio
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task Delete(Account account)
        {
            Accounts.TryRemove(account.Id, out _);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Obtiene una cuenta del repositorio que coincida con la especificacion
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public Task<Account> Get(ISpecification<Account> specification)
        {
            var account = Accounts.Values.Where(specification.IsSatisfiedBy).FirstOrDefault();
            return Task.FromResult(account);
        }

        /// <summary>
        /// Obtiene todas las cuentas que coincidan con la especificacion
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public Task<List<Account>> GetAll(ISpecification<Account> specification)
        {
            return Task.FromResult(Accounts.Values.Where(specification.IsSatisfiedBy).ToList());
        }

        /// <summary>
        /// Actualiza una cuenta en el repositorio
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task Update(Account account)
        {
            Accounts[account.Id] = account;
            return Task.CompletedTask;
        }

        public Task<bool> Exist(ISpecification<Account> specification)
            => Task.FromResult(Accounts.Values.Any(specification.IsSatisfiedBy));

        public Task<int> Count(ISpecification<Account>? specification = null)
        {
            var quantity = specification is not null ?
                Accounts.Values.Count(specification.IsSatisfiedBy) :
                Accounts.Values.Count;
            return Task.FromResult(quantity);
        }
    }

    /// <summary>
    /// Especificacion para obtener una cuenta por id
    /// </summary>
    public class AccountById : Specification<Account>
    {
        public AccountById(Guid accountId)
            :base(x => x.Id == accountId)
        {

        }
    }
    
    /// <summary>
    /// Especificacion para obtener cuentas por id y nombre
    /// </summary>
    public class AccountByIdAndName : Specification<Account>
    {
        public AccountByIdAndName(Guid accountId, string name) : 
            base(x => x.Id == accountId && x.Name.Contains(name))
        {
        }
    }
}
