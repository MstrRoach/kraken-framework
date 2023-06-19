using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

public class OutboxQueries
{
    /// <summary>
    /// Query para saber si existe un registro por id
    /// </summary>
    public static string Exist = $@"
    SELECT COALESCE(
    (SELECT 1 EXIST FROM Outbox WHERE Id = @Id),
    0) Exist;";

    /// <summary>
    /// Query para agregar un registro
    /// </summary>
    public static string Add = $@"
    INSERT INTO Outbox(Id,
        CorrelationId,
        TransactionId,
        TraceId,
        Origin,
        User,
        Username,
        EventName,
        EventType,
        Event,
        CreatedAt,
        ConfirmedAt,
        SentAt,
        LastUpdatedAt,
        LastAttemptAt,
        Status,
        Notes)
    VALUES(@Id,
        @CorrelationId,
        @TransactionId,
        @TraceId,
        @Origin,
        @User,
        @Username,
        @EventName,
        @EventType,
        @Event,
        @CreatedAt,
        @ConfirmedAt,
        @SentAt,
        @LastUpdatedAt,
        @LastAttemptAt,
        @Status,
        @Notes);";

    /// <summary>
    /// Query para obtener registros por transaccion
    /// </summary>
    public static string GetByTransaction = $@"
    SELECT Id,
        CorrelationId,
        TransactionId,
        TraceId,
        Origin,
        User,
        Username,
        EventName,
        EventType,
        Event,
        CreatedAt,
        ConfirmedAt,
        SentAt,
        LastUpdatedAt,
        LastAttemptAt,
        Status,
        Notes
    FROM Outbox 
    WHERE TransactionId = @TransactionId;";

    /// <summary>
    /// Query para actualizar un registro
    /// </summary>
    public static string Update = $@"
    UPDATE Outbox
    SET ConfirmedAt = @ConfirmedAt,
    SentAt = @SentAt,
    LastUpdatedAt = @LastUpdatedAt,
    LastAttemptAt = @LastAttemptAt,
    Status = @Status,
    Notes = @Notes
    WHERE Id = @Id;";

    /// <summary>
    /// Actualizacion parcial del registro
    /// </summary>
    public static string PartialUpdate = $@"
    UPDATE Outbox
    SET SentAt = @SentAt,
    LastUpdatedAt = @LastUpdatedAt,
    Status = @Status,
    Notes = @Notes
    WHERE Id = @Id;";

    public static string DeleteByTransaction = $@"
    DELETE FROM Outbox WHERE TransactionId = @TransactionId;";
}
