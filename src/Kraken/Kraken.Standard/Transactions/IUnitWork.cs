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
}

/// <summary>
/// Define la interface para especificar unidades de trabajo indivuales
/// por modulo
/// </summary>
/// <typeparam name="TModule"></typeparam>
public interface IUnitWork<TModule> : IUnitWork
{

}