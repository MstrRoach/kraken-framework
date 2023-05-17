using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kraken.Domain;

/// <summary>
/// Clase base que implementa la entidad y una raiza agregada
/// </summary>
/// <typeparam name="Type"></typeparam>
public abstract class Aggregate<TType, TId> : Entity<TType, TId>, IAggregate
    where TId : IComparable
{
    /// <summary>
    /// Obtiene el tipo del agregado
    /// </summary>
    public string AggregateRootType => GetType().FullName;

    /// <summary>
    /// Lista de eventos de dominio de tipo notificaciones
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    /// <summary>
    /// Devuelve la lista de eventos de dominio que lanza este agregado
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    /// <summary>
    /// Permite limpiar todos los eventos de dominio
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Agrega eventos de dominio a la lista de eventos del agregado
    /// </summary>
    /// <param name="event"></param>
    public void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    /// <summary>
    /// Remueve un evento de dominio de la lista de eventos del agregado
    /// </summary>
    /// <param name="event"></param>
    public void RemoveDomainEvent(IDomainEvent @event)
    {
        _domainEvents?.Remove(@event);
    }
}
