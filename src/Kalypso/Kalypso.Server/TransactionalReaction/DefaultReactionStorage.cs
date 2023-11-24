using Dapper;
using Dottex.Kalypso.Module.TransactionalReaction;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

/// <summary>
/// Componente encargado de almacenar las reacciones
/// utilizando una immplementacion por defecto
/// </summary>
internal class DefaultReactionStorage : IReactionStorage
{
    private static ConcurrentDictionary<Guid, ReactionRecord> reactionRecords = new ConcurrentDictionary<Guid, ReactionRecord>();

    /// <summary>
    /// Propiedades para el acceso a la base de datos
    /// </summary>
    readonly ServerDatabaseOptions _properties;

    public DefaultReactionStorage(ServerDatabaseOptions properties)
    {
        _properties = properties;
    }

    /// <summary>
    /// Busquedda de registros por tipo de filtro
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ReactionRecord> GetBy(ReactionFilter filter)
    {
        var query = ReactionQueries.Search.Replace("$Where", filter.GetFilter());
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.Query<ReactionRecord>(query, new
        {
            filter.Id,
            filter.TransactionId,
            filter.Origin,
            filter.Target,
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
    /// Almacena un registro de reaccion en la base de datos
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public Task Save(ReactionRecord record)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var exist = connection.QuerySingle<bool>(ReactionQueries.Exist,
            new
            {
                record.Id
            });
        if (!exist)
        {
            var wasInserted = connection.Execute(ReactionQueries.Add, record);
        }
        connection.Close();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Guarda todos los registros al mismo tiempo
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    public async Task SaveAll(List<ReactionRecord> records)
    {
        foreach (var record in records)
        {
            await Save(record);
        }
    }


    /// <summary>
    /// Actualiza un registro especifico
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <param name="sentAt"></param>
    /// <param name="notes"></param>
    /// <returns></returns>
    public Task Update(Guid id, ReactionRecordStatus status, DateTime? sentAt = null, string? notes = null)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var affected = connection.Execute(ReactionQueries.PartialUpdate, 
            new
            {
                Id = id,
                SentAt = sentAt,
                LastUpdatedAt = DateTime.UtcNow,
                Status = Enum.GetName<ReactionRecordStatus>(status),
                Notes = notes
            });
        connection.Close();
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
