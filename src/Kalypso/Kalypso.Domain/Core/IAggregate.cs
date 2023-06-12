using Dottex.Kalypso.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dottex.Kalypso.Domain.Core;

/// <summary>
/// Interface para marcar a los agregados
/// </summary>
public interface IAggregate
{
    /// <summary>
    /// Nombre del tipo de agregado immplementa actualmente la interface
    /// </summary>
    string AggregateRootType { get; }

    /// <summary>
    /// Accesor a la lista de dominios
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Agrega un evento de dominio a la lista de eventos
    /// </summary>
    /// <param name="event"></param>
    void AddDomainEvent(IDomainEvent @event);

    /// <summary>
    /// Remueve de la lista de eventos el evento pasado por parametro
    /// </summary>
    /// <param name="event"></param>
    void RemoveDomainEvent(IDomainEvent @event);

    /// <summary>
    /// Limpia la lista de eventos de dominio
    /// </summary>
    void ClearDomainEvents();
}
