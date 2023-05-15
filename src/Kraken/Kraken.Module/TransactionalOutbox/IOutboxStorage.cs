using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.TransactionalOutbox;

/// <summary>
/// Define el sistema de almacenamiento para
/// la bandeja de salida
/// </summary>
public interface IOutboxStorage
{
    /// <summary>
    /// Almacena un registro
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    Task Save(OutboxRecord record);

    /// <summary>
    /// Actualiza los registros confirmando la transaccion y devolviendo todos
    /// los registros actualizados
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task<IEnumerable<OutboxRecord>> ConfirmTransaction(Guid transaction);

    /// <summary>
    /// Obtiene todos los registros que pertenecen a la transaccion
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task<IEnumerable<OutboxRecord>> GetAll(Guid transaction);

    /// <summary>
    /// Actuaaliza un registro
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    Task Update(OutboxRecord record);

    /// <summary>
    /// Elimina todos los registros que pertenecen a la transaccion
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task DeleteAll(Guid transaction);
}
