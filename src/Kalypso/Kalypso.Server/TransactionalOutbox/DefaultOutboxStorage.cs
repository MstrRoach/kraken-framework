using Dapper;
using Dottex.Kalypso.Module.Audit;
using Dottex.Kalypso.Module.TransactionalOutbox;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

/// <summary>
/// Implementacion del almacenamiento por defecto
/// para los registros de bandeja de salida
/// </summary>
internal class DefaultOutboxStorage : IOutboxStorage
{
    private static ConcurrentDictionary<Guid, OutboxRecord> outboxRecords = new ConcurrentDictionary<Guid, OutboxRecord>();

    /// <summary>
    /// Propiedades para el acceso a la base de datos
    /// </summary>
    readonly ServerDatabaseProperties _properties;

    public DefaultOutboxStorage(ServerDatabaseProperties properties)
    {
        _properties = properties;
    }

    /// <summary>
    /// Busqueda de informacion en los registros
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public IEnumerable<OutboxRecord> GetBy(OutboxFilter filter)
    {
        var query = OutboxQueries.Search.Replace("$Where", filter.GetFilter());
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.Query<OutboxRecord>(query, new
        {
            filter.Id,
            filter.TransactionId,
            filter.Origin,
            filter.User,
            filter.Status,
            filter.From,
            filter.To,
            filter.PageSize,
            Offset = (filter.Page - 1) * filter.PageSize
        });
        connection.Close();
        return result;
    }

    /// <summary>
    /// Almacenaa el registro en la tabla de bandeja de salida
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public Task Save(OutboxRecord record)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var exist = connection.QuerySingle<bool>(OutboxQueries.Exist,new
        {
            record.Id
        });
        if (!exist)
        {
            var wasInserted = connection.Execute(OutboxQueries.Add, record);
        }
        connection.Close();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene la lista de registros asociada a una transaccion
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public Task<IEnumerable<OutboxRecord>> GetAll(Guid transaction)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var records = connection.Query<OutboxRecord>(OutboxQueries.GetByTransaction, new
        {
            TransactionId = transaction
        });
        connection.Close();
        return Task.FromResult(records);
    }

    /// <summary>
    /// Actualiza un registro
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public Task Update(OutboxRecord record)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var exist = connection.QuerySingle<bool>(OutboxQueries.Exist, new
        {
            record.Id
        });
        if (!exist)
        {
            Save(record);
        }
        var afected = connection.Execute(OutboxQueries.Update, record);
        connection.Close();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Actualiza el registro si existe
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <param name="sentAt"></param>
    /// <param name="notes"></param>
    /// <returns></returns>
    public Task Update(Guid id, OutboxRecordStatus status, DateTime? sentAt = null, string? notes = null)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var exist = connection.QuerySingle<bool>(OutboxQueries.Exist, new
        {
            Id = id
        });
        if (exist)
        {
            var afected = connection.Execute(OutboxQueries.PartialUpdate, new
            {
                Id = id,
                SentAt = sentAt,
                LastUpdatedAt = DateTime.UtcNow,
                Status = Enum.GetName<OutboxRecordStatus>(status),
                Notes = notes
            }) ;
        }
        connection.Close();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Actualiza todos los registros pasados
    /// </summary>
    /// <param name="updatedEvents"></param>
    /// <returns></returns>
    public async Task UpdateAll(IEnumerable<OutboxRecord> updatedEvents)
    {
        foreach (var record in updatedEvents)
        {
            await Update(record);
        }
    }

    /// <summary>
    /// Elimina todos los registros asociados a una transaccion
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public Task DeleteAll(Guid transaction)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var affected = connection.Execute(OutboxQueries.DeleteByTransaction,
            new
            {
                TransactionId = transaction
            });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obbtiene la cadena de conexion para la base de datos
    /// </summary>
    /// <returns></returns>
    private string GetConnectionString()
    {
        var database = "$DATABASE.db".Replace("$DATABASE", _properties.DatabaseName);
        var path = Path.Combine(
            _properties.ApplicationData,
            _properties.ApplicationName,
            database
            );
        return "Data Source=$DATABASE_PATH;".Replace("$DATABASE_PATH", path);
    }


}
