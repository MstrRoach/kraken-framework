using Dapper;
using Dottex.Kalypso.Module.Processing;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

/// <summary>
/// Inicializador del almacen en memoria
/// </summary>
internal class ReactionInitializer : IInitializer
{
    /// <summary>
    /// Configuraciones para la base de datos
    /// </summary>
    readonly ServerDatabaseProperties _databaseProperties;

    public ReactionInitializer(ServerDatabaseProperties databaseProperties)
    {
        _databaseProperties = databaseProperties;
    }

    /// <summary>
    /// Ejecuta la inicializacion de las reacciones
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
    /// Revisa que la tabla de reacciones exista
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
        var result = connection.Execute(ReactionQueries.Create);
        connection.Close();
    }
}

public class ReactionQueries
{
    /// <summary>
    /// Cadena para crear una tabla
    /// </summary>
    public static string Create = $@"
    CREATE TABLE IF NOT EXISTS Reaction(
        Id TEXT PRIMARY KEY,
        CorrelationId TEXT NOT NULL,
        TraceId TEXT NOT NULL,
        User TEXT NOT NULL,
        Username TEXT NOT NULL,
        Origin TEXT NOT NULL,
        Target TEXT NOT NULL,
        EventId TEXT NOT NULL,
        EventName TEXT NOT NULL,
        EventType TEXT NOT NULL,
        Event TEXT NOT NULL,
        CreatedAt TEXT NOT NULL,
        SentAt TEXT NULL,
        LastUpdatedAt TEXT NOT NULL,
        LastAttemptAt TEXT NOT NULL,
        Status TEXT NOT NULL,
        Notes TEXT NULL,
        ReactionType TEXT NULL);";

    public static string Exist = $@"
    SELECT COALESCE(
    (SELECT 1 EXIST FROM Reaction WHERE Id = @Id),
    0) Exist;";

    public static string Add = $@"
    INSERT INTO Reaction(Id,
        CorrelationId,
        TraceId,
        User,
        Username,
        Origin,
        Target,
        EventId,
        EventName,
        EventType,
        Event,
        CreatedAt,
        SentAt,
        LastUpdatedAt,
        LastAttemptAt,
        Status,
        Notes,
        ReactionType)
    VALUES(@Id,
        @CorrelationId,
        @TraceId,
        @User,
        @Username,
        @Origin,
        @Target,
        @EventId,
        @EventName,
        @EventType,
        @Event,
        @CreatedAt,
        @SentAt,
        @LastUpdatedAt,
        @LastAttemptAt,
        @Status,
        @Notes,
        @ReactionType)";

    public static string PartialUpdate = $@"
    UPDATE Reaction
    SET SentAt = @SentAt,
    LastUpdatedAt = @LastUpdatedAt,
    Status = @Status,
    Notes = @Notes
    WHERE Id = @Id;";

    /// <summary>
    /// Query para busqueda
    /// </summary>
    public static string Search = $@"
    SELECT Id,
        CorrelationId,
        TraceId,
        User,
        Username,
        Origin,
        Target,
        EventId,
        EventName,
        EventType,
        Event,
        CreatedAt,
        SentAt,
        LastUpdatedAt,
        LastAttemptAt,
        Status,
        Notes,
        ReactionType
    FROM Reaction
    $Where;";
}
