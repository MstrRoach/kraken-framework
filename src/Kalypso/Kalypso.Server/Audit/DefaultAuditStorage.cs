using Dapper;
using Dottex.Kalypso.Module.Audit;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Audit;

/// <summary>
/// Contiene la implementacion por defecto para
/// el almacenamiento de auditoria
/// </summary>
internal class DefaultAuditStorage : IAuditStorage
{
    /// <summary>
    /// Propiedades para la configuracion de la base de datos
    /// </summary>
    readonly ServerDatabaseOptions _properties;

    /// <summary>
    /// Constructor del almacen por defecto
    /// </summary>
    /// <param name="properties"></param>
    public DefaultAuditStorage(ServerDatabaseOptions properties)
    {
        _properties = properties;
    }

    /// <summary>
    /// Query para realizar las busquedass en los registros de auditoria
    /// </summary>
    private const string SearchQuery = $@"
    SELECT Id,Module,EntityId,Entity,Operation,Delta,User,UpdatedAt
    FROM Audit
    $Where";

    /// <summary>
    /// Devuelve la lista de registros de auditoria
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public List<AuditLog> GetAll(AuditFilter filter)
    {
        var query = SearchQuery.Replace("$Where", filter.GetFilter());
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.Query<AuditLog>(query, new
        {
            filter.Id,
            filter.Module,
            filter.EntityId,
            filter.Entity,
            filter.Operation,
            filter.User,
            filter.From,
            filter.To,
            filter.PageSize,
            Offset = (filter.Page - 1) * filter.PageSize
        });
        connection.Close();
        return result.ToList();
    }

    /// <summary>
    /// Query para agregar un registro dentro del almacen
    /// de logs
    /// </summary>
    private const string AddRecord = $@"
    INSERT INTO Audit(Module,EntityId,Entity,Operation,Delta,User,UpdatedAt)
    VALUES(@Module,@EntityId,@Entity,@Operation,@Delta,@User,@UpdatedAt);
    SELECT LAST_INSERT_ROWID();";

    /// <summary>
    /// Almacena un registro en la basse de datos en
    /// memoria
    /// </summary>
    /// <param name="record"></param>
    public void Save(AuditLog record)
    {
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.ExecuteScalar<int>(AddRecord, new
        {
            record.Module,
            record.EntityId,
            record.Entity,
            record.Operation,
            record.Delta,
            record.User,
            record.UpdatedAt
        });
        connection.Close();
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
