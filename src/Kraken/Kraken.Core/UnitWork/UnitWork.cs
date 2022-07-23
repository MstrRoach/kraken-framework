using Kraken.Core.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.UnitWork;

/// <summary>
/// Unidad de trabajo integrada con el bus de eventos disponibles dentro
/// de la aplicacion
/// </summary>
public abstract class UnitWork : IUnitWork
{
    /// <summary>
    /// Bus de eventos para los eventos de infrastructura
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor para la unidad de trabajo base
    /// </summary>
    /// <param name="mediator"></param>
    public UnitWork(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Permite acceder a la transaccion actual
    /// de la unidad de trabajo
    /// </summary>
    /// <returns></returns>
    public abstract Guid TransactionId { get; }

    /// <summary>
    /// Inicia una transaccion y envia el evento despues de invocar al metodo interno
    /// </summary>
    async Task IUnitWork.StartTransaction()
    {
        this.StartTransaction();
        await this._mediator.Publish(new TransactionStarted { TransactionId = TransactionId });
    }

    /// <summary>
    /// Inicia la transaccion
    /// </summary>
    public abstract void StartTransaction();

    async Task IUnitWork.Commit()
    {
        await this.Commit();
        await this._mediator.Publish(new TransactionCommited { TransactionId = TransactionId });
    }

    /// <summary>
    /// Confirma la transaccion actual
    /// </summary>
    /// <returns></returns>
    public abstract Task Commit();

    /// <summary>
    /// Revierte una transaccion y desecha los cambios
    /// </summary>
    /// <returns></returns>
    async Task IUnitWork.Rollback()
    {
        await this.Rollback();
        await _mediator.Publish(new TransacctionFailed { TransactionId = TransactionId });
    }

    /// <summary>
    /// Revierta la transaccion en el elemento base
    /// </summary>
    /// <returns></returns>
    public abstract Task Rollback();

}

/// <summary>
/// Evento base para los eventos de la unidad de trabajo
/// </summary>
public record UnitWorkEventBase : IComponentEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nombre del componente que genera el evento
    /// </summary>
    public string Component => "UnitWork";
}

/// <summary>
/// Evento para indicar que una transaccion ha sido iniciada
/// </summary>
public record TransactionStarted : UnitWorkEventBase
{
    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; set; }
}

/// <summary>
/// Indica que una transaccion fue finalizada
/// correctammente
/// </summary>
public record TransactionCommited : UnitWorkEventBase
{

    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; set; }
}

/// <summary>
/// Indica que una transaccion fallo y que se revirtio
/// </summary>
public record TransacctionFailed : UnitWorkEventBase
{
    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; set; }
}
