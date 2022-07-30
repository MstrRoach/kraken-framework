﻿using Kraken.Core.Internal.Domain;
using Kraken.Core.Internal.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal.Domain;

/// <summary>
/// Clase base para almacenar las entidades de dominio y asegurarse de
/// distribuir los eventos internos
/// </summary>
/// <typeparam name="T"></typeparam>
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
    private readonly IEventBus _eventBus;

    /// <summary>
    /// Constructor para el decorador
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="eventBus"></param>
    public EventExtractorRepositoryDecorator(IRepository<T> inner, IEventBus eventBus)
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
    public async Task<T> Get()
    {
        return await _inner.Get();
    }

    /// <summary>
    /// Obtiene una lista dee entiddades que coincideen con 
    /// la especificacion
    /// </summary>
    /// <returns></returns>
    public async Task<List<T>> GetAll()
    {
        return await _inner.GetAll();
    }

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
        foreach (var @event in aggregate.DomainEvents)
        {
            _eventBus.Publish(@event).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
