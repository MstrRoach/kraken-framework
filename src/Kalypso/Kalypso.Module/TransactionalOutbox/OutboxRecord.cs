using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.TransactionalOutbox;

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
public record OutboxRecord
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Id de correlacion de la solicitud
    /// </summary>
    public Guid CorrelationId { get; init; }

    /// <summary>
    /// Id de transaccion
    /// </summary>
    public Guid TransactionId { get; init; }

    /// <summary>
    /// Identificador de trazado
    /// </summary>
    public string TraceId { get; init; }

    /// <summary>
    /// Modulo de origen
    /// </summary>
    public string Origin { get; init; }

    /// <summary>
    /// Id del usuario que desencadeno el evento
    /// </summary>
    public string User { get; init; }

    /// <summary>
    /// Nombre del usuario que desencadeno el evento
    /// </summary>
    public string Username { get; init; }

    /// <summary>
    /// Nombre del evento
    /// </summary>
    public string EventName { get; init; }

    /// <summary>
    /// Tipo del evento
    /// </summary>
    public string EventType { get; init; }

    /// <summary>
    /// Evento serializado
    /// </summary>
    public string Event { get; init; }

    /// <summary>
    /// Fecha de creacion del evento
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Fecha en la que la transaccion fue confirmada
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Fecha en la que el evento fue publicado
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Ultima fecha que se actualizo el registro
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }

    /// <summary>
    /// Ultima fecha de intento de ejecucion
    /// </summary>
    public DateTime LastAttemptAt { get; set; }

    /// <summary>
    /// Estado del registro
    /// </summary>
    public OutboxRecordStatus Status { get; set; }

    /// <summary>
    /// Informacion relevante acerca del registro
    /// </summary>
    public string Notes { get; set; }
}

/// <summary>
/// Estados por los que puede pasar el mensaje
/// </summary>
public enum OutboxRecordStatus { Pending, OnProcess, Processed, OnError };
