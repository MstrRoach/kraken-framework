using Dapper;
using Dottex.Kalypso.Domain.Audit;
using Dottex.Kalypso.Module.Common;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dottex.Domain.AuditStorage.InMemory;

public class DefaultAuditStorage<TModule> : IAuditStorage<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Contiene las opciones de configuracion
    /// </summary>
    readonly InMemoryAuditStorageOptions<TModule> _options;
    public DefaultAuditStorage(InMemoryAuditStorageOptions<TModule> options)
    {
        _options = options;
    }

    private const string AddRecord = $@"
    INSERT INTO AUDIT(EntityId,Entity,Operation,Delta,User,UpdatedAt)
    VALUES(@EntityId,@Entity,@Operation,@Delta,@User,@UpdatedAt);
    SELECT LAST_INSERT_ROWID();";

    /// <summary>
    /// Guarda un registro en la base de datos en memoria
    /// </summary>
    /// <param name="record"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Save(Change record)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.ExecuteScalar<int>(AddRecord, new
        {
            record.EntityId,
            record.Entity,
            Operation = record.Operation.ToString(),
            Delta = JsonSerializer.Serialize(record.Delta),
            record.User,
            record.UpdatedAt,
        });
        connection.Close();
    }


    /// <summary>
    /// Obbtiene la cadena de conexion para la base de datos
    /// </summary>
    /// <returns></returns>
    private string GetConnectionString()
    {
        var database = "$DATABASE.db".Replace("$DATABASE", _options.DatabaseName);
        var path = Path.Combine(
            _options.ApplicationData,
            _options.ApplicationName,
            database
            );
        return "Data Source=$DATABASE_PATH;".Replace("$DATABASE_PATH", path);
    }
}
