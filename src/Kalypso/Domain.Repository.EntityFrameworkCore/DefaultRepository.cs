using Dottex.Kalypso.Domain.Core;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Domain.Repository.EntityFrameworkCore;

/// <summary>
/// Repositorio generico basado en entity framework
/// </summary>
/// <typeparam name="TModule"></typeparam>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TAggregate"></typeparam>
internal class DefaultRepository<TModule, TContext, TAggregate> : IRepository<TAggregate>
    where TModule : IModule
    where TContext : DbContext
    where TAggregate : class, IAggregate
{
    readonly TContext _context;

    public DefaultRepository(TContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creacion del agregado
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Create(TAggregate aggregate)
    {
        await _context.Set<TAggregate>().AddAsync(aggregate);
        _context.SaveChanges();
    }

    /// <summary>
    /// Eliminacion del agregado
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Delete(TAggregate aggregate)
    {
        _context.Set<TAggregate>().Remove(aggregate);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Recuperacion de un agregado
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<TAggregate> Get(Expression<Func<TAggregate,bool>> criteria)
    {
        var aggregate = _context.Set<TAggregate>().Where(criteria).FirstOrDefault();
        return Task.FromResult(aggregate);
    }

    /// <summary>
    /// Recuperacion de varios agregados
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<List<TAggregate>> GetAll(Expression<Func<TAggregate, bool>> criteria)
    {
        var aggregate = _context.Set<TAggregate>().Where(criteria).ToList();
        return Task.FromResult(aggregate);
    }

    /// <summary>
    /// Actualizacion de un agregado
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Update(TAggregate aggregate)
    {
        _context.Set<TAggregate>().Update(aggregate);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Indica si existe un agregado
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<bool> Exist(Expression<Func<TAggregate, bool>> criteria)
            => Task.FromResult(_context.Set<TAggregate>().Any(criteria));

    /// <summary>
    /// Cuenta la cantidad de agregados
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<int> Count(Expression<Func<TAggregate, bool>>? criteria = null)
    {
        var quantity = criteria is not null ?
            _context.Set<TAggregate>().Where(criteria).Count() :
            _context.Set<TAggregate>().Count();
        return Task.FromResult(quantity);
    }
}
