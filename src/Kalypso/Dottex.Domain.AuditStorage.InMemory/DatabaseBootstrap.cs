using Dapper;
using Dottex.Kalypso.Module.Common;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Domain.AuditStorage.InMemory;

public class DatabaseBootstrap<TModule> where TModule : IModule
{
    /// <summary>
    /// Opciones  de configuracion para la extension
    /// </summary>
    readonly InMemoryAuditStorageOptions<TModule> _options;

    /// <summary>
    /// Contructor del arrancador
    /// </summary>
    /// <param name="options"></param>
    public DatabaseBootstrap(InMemoryAuditStorageOptions<TModule> options)
    {
        _options = options;
    }

    /// <summary>
    /// Inicializa los necessario para
    /// habilitar el almacen en memoria
    /// </summary>
    public void Initialize()
    {
        EnsureDatabase();
        EnsureAuditStorageTable();
    }

    /// <summary>
    /// Asegura que el archivo de base de datos exista
    /// </summary>
    private void EnsureDatabase()
    {
        var database = $"{_options.DatabaseName}.db";
        var databasePath = Path.Combine(
            _options.ApplicationData,
            _options.ApplicationName,
            database);
        var databaseFolder = Path.Combine(
            _options.ApplicationData,
            _options.ApplicationName);
        if (File.Exists(databasePath))
            return;
        if (!Directory.Exists(databaseFolder))
            Directory.CreateDirectory(databaseFolder);
        File.Create(path: databasePath);
        return;
    }

    /// <summary>
    /// Cadena para crear una tabla
    /// </summary>
    public const string CreateTable = $@"
CREATE TABLE IF NOT EXISTS AUDIT(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EntityId TEXT NOT NULL,
    Entity TEXT NOT NULL,
    Operation TEXT NOT NULL,
    Delta TEXT NOT NULL,
    User TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);";

    /// <summary>
    /// Template para la cadena de conexion
    /// </summary>
    public const string ConnectionString = "Data Source=$DATABASE_PATH;";

    /// <summary>
    /// Revisa que la tabla de auditoria exista
    /// </summary>
    private void EnsureAuditStorageTable()
    {
        var database = "$DATABASE.db".Replace("$DATABASE", _options.DatabaseName);
        var path = Path.Combine(
            _options.ApplicationData,
            _options.ApplicationName,
            database
            );
        var connectionString = ConnectionString.Replace("$DATABASE_PATH", path);
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var result = connection.Execute(CreateTable);
        connection.Close();
    }
}
