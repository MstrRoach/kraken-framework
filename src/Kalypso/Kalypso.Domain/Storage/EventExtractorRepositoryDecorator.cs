﻿using Dottex.Kalypso.Domain.Core;
using Dottex.Kalypso.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Storage;

public sealed class EventExtractorRepositoryDecorator<T> : IRepository<T>
    where T : IAggregate
{
    /// <summary>
    /// Repositorio interno que contiene las lecturas
    /// y escrituras
    /// </summary>
    private readonly IRepository<T> _inner;

    /// <summary>
    /// Bus de eventos para distribuir los eventos generados por
    /// las entidades de dominio
    /// </summary>
    private readonly IEventPublisher _eventBus;

    public EventExtractorRepositoryDecorator(IRepository<T> inner, IEventPublisher eventBus)
    {
        _inner = inner;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Ejecuta la creacion y despues extraeee los eventos para
    /// distribuirlos a traves deel bus dee eevntos
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Create(T aggregate)
    {
        await _inner.Create(aggregate);
        DispatchEvents(aggregate);
    }

    /// <summary>
    /// Elimina el elemento y distribuye los eventos
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Delete(T aggregate)
    {
        await _inner.Delete(aggregate);
        DispatchEvents(aggregate);
    }

    /// <summary>
    /// Obtiene un eelemento que concida con la especificacion
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<T> Get(Expression<Func<T, bool>> criteria)
        => await _inner.Get(criteria);


    /// <summary>
    /// Obtiene una lista dee entiddades que coincideen con 
    /// la especificacion
    /// </summary>
    /// <returns></returns>
    public async Task<List<T>> GetAll(Expression<Func<T, bool>> criteria)
        => await _inner.GetAll(criteria);

    /// <summary>
    /// Indica si existe algun registro que coincida con la especificacion
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<bool> Exist(Expression<Func<T, bool>> criteria)
        => _inner.Exist(criteria);

    /// <summary>
    /// Realiza el conteo de los registros que coincidan con la
    /// especificacion pasada por parametro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<int> Count(Expression<Func<T, bool>>? criteria = null)
        => _inner.Count(criteria);


    /// <summary>
    /// Actualiza un reeegistro deel agregado dentro
    /// del reepositorio y distribuye los eventos
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public async Task Update(T aggregate)
    {
        await _inner.Update(aggregate);
        DispatchEvents(aggregate);
    }

    /// <summary>
    /// Distribuye todos los eveentos dee domminio que esten dentro
    /// de la eentidad
    /// </summary>
    /// <param name="aggregate"></param>
    private void DispatchEvents(T aggregate)
    {
        // Distribuimos los eventos
        foreach (var @event in aggregate.DomainEvents)
        {
            _eventBus.Publish(@event).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        // Los eliminamos
        aggregate.ClearDomainEvents();
    }
}
