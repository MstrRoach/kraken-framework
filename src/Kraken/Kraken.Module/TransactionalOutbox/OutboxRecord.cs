using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.TransactionalOutbox;

/// <summary>
/// Registro para guardar un mensaje de la bandeja de salida
/// </summary>
/// <param name="Id"></param>
/// <param name="CorrelationId"></param>
/// <param name="TransactionId"></param>
/// <param name="TraceId"></param>
/// <param name="Origin"></param>
/// <param name="User"></param>
/// <param name="Username"></param>
/// <param name="EventName"></param>
/// <param name="EventType"></param>
/// <param name="Event"></param>
/// <param name="CreatedAt"></param>
/// <param name="SentAt"></param>
/// <param name="Status"></param>
/// <param name="Error"></param>
public record OutboxRecord(
    Guid Id,
    Guid CorrelationId,
    Guid TransactionId,
    string TraceId,
    string Origin,
    string User,
    string Username,
    string EventName,
    string EventType,
    string Event,
    DateTime CreatedAt,
    DateTime? ConfirmedAt,
    DateTime? SentAt,
    DateTime LastUpdatedAt,
    OutboxRecordStatus Status,
    string Error
);

/// <summary>
/// Estados por los que puede pasar el mensaje
/// </summary>
public enum OutboxRecordStatus { Pending, OnProcess, Processed, OnError };
