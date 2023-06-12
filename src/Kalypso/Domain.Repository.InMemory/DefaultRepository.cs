using Dottex.Domain.Repository.InMemory.MemoryStorable;
using Dottex.Kalypso.Domain.Core;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Domain.Repository.InMemory;

/// <summary>
/// Repositorio por defecto para realizar en memoria
/// </summary>
/// <typeparam name="TAggregate"></typeparam>
/// <typeparam name="TType"></typeparam>
/// <typeparam name="TId"></typeparam>
public class DefaultRepository<TModule, TAggregate, TId> : IRepository<TAggregate>
    where TModule : IModule
    where TAggregate : Aggregate<TId>, IAggregate
    where TId : IComparable
{

    /// <summary>
    /// Accesor al almacen de base de datos en memoria
    /// </summary>
    private readonly DefaultMemoryStorable<TModule, TAggregate> _storage;

    public DefaultRepository(DefaultMemoryStorable<TModule, TAggregate> storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// Agrega un nuevo registro del aggregado a la lista
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Create(TAggregate aggregate)
    {
        _storage.Add(aggregate);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Elimina una instancia del agregado
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Delete(TAggregate aggregate)
    {
        _storage.Delete(aggregate);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene un agregado a partir de una especificacion de
    /// filtro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<TAggregate> Get(ISpecification<TAggregate> specification)
    {
        var aggregate = _storage
            .GetAll()
            .Where(specification.IsSatisfied)
            .FirstOrDefault();
        return Task.FromResult(aggregate);
    }

    /// <summary>
    /// Obtiene una lista de registros que coinciden con la especificacion
    /// de filtro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<List<TAggregate>> GetAll(ISpecification<TAggregate> specification)
    {
        var aggregates = _storage
            .GetAll()
           .Where(specification.IsSatisfied)
           .ToList();
        return Task.FromResult(aggregates);
    }

    /// <summary>
    /// Actualiza una entidad
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Update(TAggregate aggregate)
    {
        _storage.Update(aggregate);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica la existencia para un filtro especificado
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<bool> Exist(ISpecification<TAggregate> specification)
            => Task.FromResult(_storage.GetAll().Any(specification.IsSatisfied));

    /// <summary>
    /// Cuenta la cantidad de elementos que coinciden con la busqueda
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<int> Count(ISpecification<TAggregate>? specification = null)
    {
        var quantity = specification is not null ?
            _storage.GetAll().Where(specification.IsSatisfied).Count() :
            _storage.GetAll().Count();
        return Task.FromResult(quantity);
    }
}