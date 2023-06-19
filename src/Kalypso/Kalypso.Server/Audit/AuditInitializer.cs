using Dapper;
using Dottex.Kalypso.Module.Processing;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Audit;

/// <summary>
/// Arrancador de las configuraciones para el almacenamiento
/// de los registros de auditoria de kalypso
/// </summary>
internal class AuditInitializer : IInitializer
{
    /// <summary>
    /// Configuraciones para la base de datos
    /// </summary>
    readonly ServerDatabaseProperties _databaseProperties;

    public AuditInitializer(ServerDatabaseProperties databaseProperties)
    {
        _databaseProperties = databaseProperties;
    }

    /// <summary>
    /// Ejecuta la inicializacion de la auditoria
    /// </summary>
    /// <returns></returns>
    public Task Run()
    {
        EnsureDatabase();
        EnsureAuditStorageTable();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asegura que el archivo de base de datos exista
    /// </summary>
    private void EnsureDatabase()
    {
        var database = $"{_databaseProperties.DatabaseName}.db";
        var databasePath = Path.Combine(
            _databaseProperties.ApplicationData,
            _databaseProperties.ApplicationName,
            database);
        var databaseFolder = Path.Combine(
            _databaseProperties.ApplicationData,
            _databaseProperties.ApplicationName);
        if (File.Exists(databasePath))
            return;
        if (!Directory.Exists(databaseFolder))
            Directory.CreateDirectory(databaseFolder);

        var connectionString = ConnectionString.Replace("$DATABASE_PATH", databasePath);
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        connection.Execute("PRAGMA journal_mode=DELETE;");
        connection.Close();
        return;
    }

    /// <summary>
    /// Cadena para crear una tabla
    /// </summary>
    public const string CreateTable = $@"
    CREATE TABLE IF NOT EXISTS Audit(
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Module TEXT NOT NULL,
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
        var database = "$DATABASE.db".Replace("$DATABASE", _databaseProperties.DatabaseName);
        var path = Path.Combine(
            _databaseProperties.ApplicationData,
            _databaseProperties.ApplicationName,
            database
            );
        var connectionString = ConnectionString.Replace("$DATABASE_PATH", path);
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        connection.Execute("PRAGMA journal_mode=DELETE;");
        var result = connection.Execute(CreateTable);
        connection.Close();
    }
}
