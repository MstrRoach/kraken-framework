using Domain.Repository.InMemory.MemoryStorable;
using Kraken.Domain.Core;
using Kraken.Domain.Storage;
using Kraken.Module.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository.InMemory;

/// <summary>
/// Repositorio por defecto para realizar en memoria
/// </summary>
/// <typeparam name="TAggregate"></typeparam>
/// <typeparam name="TType"></typeparam>
/// <typeparam name="TId"></typeparam>
public class DefaultRepository<TModule,TAggregate, TId> : IRepository<TAggregate>
    where TModule : IModule
    where TAggregate : Aggregate<TId>, IAggregate
    where TId : IComparable
{
    private static ConcurrentDictionary<TId, TAggregate> Aggregates = new();

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
        Aggregates.TryRemove(aggregate.Id, out _);
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
        var aggregate = Aggregates
            .Values
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
        var aggregates = Aggregates
           .Values
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
        Aggregates[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica la existencia para un filtro especificado
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<bool> Exist(ISpecification<TAggregate> specification)
            => Task.FromResult(Aggregates.Values.Any(specification.IsSatisfied));

    /// <summary>
    /// Cuenta la cantidad de elementos que coinciden con la busqueda
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<int> Count(ISpecification<TAggregate>? specification = null)
    {
        var quantity = specification is not null ?
            Aggregates.Values.Count(specification.IsSatisfied) :
            Aggregates.Values.Count;
        return Task.FromResult(quantity);
    }
}