using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.EventBus;

/// <summary>
/// Evento base para derivar todos las mismas
/// propiedades de administracion y procesamiento
/// </summary>
public interface IEvent : INotification
{
    /// <summary>
    /// Id del evento de dominio
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Fecha en la que ocurrio el evento
    /// </summary>
    DateTime OccurredOn { get; }
}

/// <summary>
/// Interface para definir los eventos de dominio
/// </summary>
public interface IDomainEvent : IEvent
{

}

/// <summary>
/// Interface base para los eventos dentro generados dentro de
/// de kraken.
/// </summary>
public interface IModuleEvent : IEvent
{
    
}

/// <summary>
/// Interface para definir los eventos de arquitectura, que permiten
/// realizar un conjunto de operaciones de coordinacion entre componentes
/// internos del sistema
/// </summary>
public interface IComponentEvent : IEvent
{
    
    /// <summary>
    /// Nombre del componente que emite el evento
    /// </summary>
    string Component { get; }

}
