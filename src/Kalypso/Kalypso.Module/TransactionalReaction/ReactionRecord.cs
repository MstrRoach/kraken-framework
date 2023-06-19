using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.TransactionalReaction;

public record ReactionRecord
{
    /// <summary>
    /// Id de la reaccion
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id de correlacion
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Id de seguimiento unico por solicitud
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// Id del usuario que ejecuto el reques
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Nombre del usuario que ejecuta el request
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Modulo desde el que viene el evento
    /// </summary>
    public string Origin { get; set; }

    /// <summary>
    /// Indica cual es el modulo que respondio al evento
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Id del evento que genero la reaccion
    /// </summary>
    public Guid EventId { get; set; }

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
    /// Estado del registro para administrar su operacion
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Informacion relevante acerca del registro
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// Indica el tipo de reaccion que responde al evento
    /// </summary>
    public string ReactionType { get; set; }
}

/// <summary>
/// Estados por los que puede pasar el mensaje
/// </summary>
public enum ReactionRecordStatus { OnProcess, Processed, OnError };