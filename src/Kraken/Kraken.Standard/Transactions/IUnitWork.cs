using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Transactions;

/// <summary>
/// Define la unidad de trabajo para las
/// operaciones de transaccionalidad
/// </summary>
public interface IUnitWork
{
    /// <summary>
    /// Accesor al id de la transaccion
    /// </summary>
    Guid TransactionId { get; }

    /// <summary>
    /// Inicia una transaccion
    /// </summary>
    void StartTransaction();

    /// <summary>
    /// Confirma la transsaccion
    /// </summary>
    /// <returns></returns>
    Task Commit();

    /// <summary>
    /// Revierte la transaccion
    /// </summary>
    /// <returns></returns>
    Task Rollback();

    /// <summary>
    /// Ejecuta la operacion con la transaccionalidad administrada
    /// por la unidad de trabajo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    Task ExecuteAsync(Func<Task> action);

    /// <summary>
    /// Ejecuta una operacion con respuesta, administrada
    /// por la unidad de trabajo y envuelva en los procesos
    /// de transaccionalidad
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
}

/// <summary>
/// Define la interface para especificar unidades de trabajo indivuales
/// por modulo
/// </summary>
/// <typeparam name="TModule"></typeparam>
public interface IUnitWork<TModule> : IUnitWork
{

}