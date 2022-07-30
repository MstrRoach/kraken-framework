using Kraken.Core.Internal.EventBus;
using Kraken.Core.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.Transaction;

/// <summary>
/// Unidad de trabajo integrada con el bus de eventos disponibles dentro
/// de la aplicacion
/// </summary>
public abstract class UnitWork : IUnitWork
{
    /// <summary>
    /// Bus de eventos para los eventos de infrastructura
    /// </summary>
    private readonly IEventBus _eventBus;

    /// <summary>
    /// Constructor para la unidad de trabajo base
    /// </summary>
    /// <param name="mediator"></param>
    public UnitWork(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Permite acceder a la transaccion actual
    /// de la unidad de trabajo
    /// </summary>
    /// <returns></returns>
    public abstract Guid TransactionId { get; }


    /// <summary>
    /// Administra el proceso genericamente para ejecutar una
    /// accion con la transacciondad administrada por dentro
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task ExecuteAsync(Func<Task> action)
    {
        // Iniciamos la transaccion
        StartTransaction();
        // Publicamos el evento
        await _eventBus.Publish(new TransactionStarted { TransactionId = TransactionId });
        try
        {
            // Ejecutamos el proceso
            await action();
            // Confirmamos los cammbios
            await Commit();
            // Publicamos el exito de la transaccion
            await _eventBus.Publish(new TransactionCommited { TransactionId = TransactionId });
        }
        catch (Exception)
        {
            // Si falla, entonces, deshacemos los cambiios
            await Rollback();
            // Publicamos el evento de fallo
            await _eventBus.Publish(new TransacctionFailed { TransactionId = TransactionId });
            // Lanzamos el error mas arriba
            throw;
        }
    }

    /// <summary>
    /// Ejecuta una operacion con respuesta, envolviendo el proceso
    /// en la transaccionalidad de la unidad de trrabajo y bajo el
    /// flujo de los eventos que emite la unidad de trabajo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        // Iniciamos la transaccion
        StartTransaction();
        // Publicamos el evento
        await _eventBus.Publish(new TransactionStarted { TransactionId = TransactionId });
        try
        {
            // Ejecutamos el proceso
            var result = await action();
            // Confirmamos los cammbios
            await Commit();
            // Publicamos el exito de la transaccion
            await _eventBus.Publish(new TransactionCommited { TransactionId = TransactionId });
            // Devolvemos el resultado
            return result;
        }
        catch (Exception)
        {
            // Si falla, entonces, deshacemos los cambiios
            await Rollback();
            // Publicamos el evento de fallo
            await _eventBus.Publish(new TransacctionFailed { TransactionId = TransactionId });
            // Lanzamos el error mas arriba
            throw;
        }
    }

    /// <summary>
    /// Inicia la transaccion
    /// </summary>
    public abstract void StartTransaction();

    /// <summary>
    /// Confirma la transaccion actual
    /// </summary>
    /// <returns></returns>
    public abstract Task Commit();

    /// <summary>
    /// Revierta la transaccion en el elemento base
    /// </summary>
    /// <returns></returns>
    public abstract Task Rollback();
}
