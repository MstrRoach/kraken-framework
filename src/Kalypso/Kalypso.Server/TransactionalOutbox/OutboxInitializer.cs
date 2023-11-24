using Dapper;
using Dottex.Kalypso.Module.Processing;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

/// <summary>
/// Encargado de inicializar los servicios para la bandeja de 
/// salida transaccional
/// </summary>
internal class OutboxInitializer : IInitializer
{
    /// <summary>
    /// Configuraciones para la base de datos
    /// </summary>
    readonly ServerDatabaseOptions _databaseProperties;

    public OutboxInitializer(ServerDatabaseOptions databaseProperties)
    {
        _databaseProperties = databaseProperties;
    }

    /// <summary>
    /// Ejecuta la inicializacion de la bandeja de salida
    /// </summary>
    /// <returns></returns>
    public Task Run()
    {
        EnsureDatabase();
        EnsureAuditStorageTable();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Template para la cadena de conexion
    /// </summary>
    public const string ConnectionString = "Data Source=$DATABASE_PATH;";

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
    CREATE TABLE IF NOT EXISTS Outbox(
        Id TEXT PRIMARY KEY,
        CorrelationId TEXT NOT NULL,
        TransactionId TEXT NOT NULL,
        TraceId TEXT NOT NULL,
        Origin TEXT NOT NULL,
        User TEXT NOT NULL,
        Username TEXT NOT NULL,
        EventName TEXT NOT NULL,
        EventType TEXT NOT NULL,
        Event TEXT NOT NULL,
        CreatedAt TEXT NOT NULL,
        ConfirmedAt TEXT NULL,
        SentAt TEXT NULL,
        LastUpdatedAt TEXT NOT NULL,
        LastAttemptAt TEXT NOT NULL,
        Status TEXT NOT NULL,
        Notes TEXT NULL
    );";

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
