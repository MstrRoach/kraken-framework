﻿using Dottex.Kalypso.Module.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Request.Mediator;

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
/// Clase abstracta para los eventos con contexto
/// </summary>
public abstract class EventBase : IEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Fecha que ocurre el evento
    /// </summary>
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Contexto del evento
    /// </summary>
    public IContext Context { get; set; }
}

/// <summary>
/// Interface para definir los eventos de dominio
/// </summary>
public interface IDomainEvent : IEvent
{

}

/// <summary>
/// Interface base para los eventos generados en un modulo y que esta disponible
/// para los demas modulos
/// </summary>
public interface IModuleEvent : IEvent
{

}


/// <summary>
/// Interface para definir los eventos de arquitectura, que permiten
/// realizar un conjunto de operaciones de coordinacion entre componentes
/// internos del sistema
/// </summary>
public interface IArchEvent : IEvent
{

    /// <summary>
    /// Nombre del componente que emite el evento
    /// </summary>
    string Component { get; }

}

