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
    /// Actualiza un registro sin recuperarlo
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <param name="notes"></param>
    /// <returns></returns>
    Task Update(Guid id, OutboxRecordStatus status, DateTime? sentAt = null, string? notes = null);

    /// <summary>
    /// Actualiza todos los registros pasados de una sola vez
    /// </summary>
    /// <param name="updatedEvents"></param>
    /// <returns></returns>
    Task UpdateAll(IEnumerable<OutboxRecord> updatedEvents);

    /// <summary>
    /// Elimina todos los registros que pertenecen a la transaccion
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    Task DeleteAll(Guid transaction);


}
