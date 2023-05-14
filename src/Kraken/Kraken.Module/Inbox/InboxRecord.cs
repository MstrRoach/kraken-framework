using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Inbox;

/// <summary>
/// Define el registro utilizado para guardar una reaccion
/// en el sistema de almacenamiento predeterminado
/// </summary>
public class InboxRecord
{
    /// <summary>
    /// Id deel handler
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id de correlacion parael handler
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Id del evento que generoel handler
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Tipo del evento asociado ael handler
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Nombre deel handler
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Typo deel handler
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Indica la fecha de creacion
    /// </summary>
    public DateTime CreateAt { get; set; }

    /// <summary>
    /// Indica la fecha de actualizacion
    /// </summary>
    public DateTime UpdateAt { get; set; }

    /// <summary>
    /// Si existe un error entonces aqui debe aparecer el
    /// problema para la evaluacion
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Indica el status de registro
    /// </summary>
    public InboxRecordStatus Status { get; set; } = InboxRecordStatus.Scheduled;
}

/// <summary>
/// Estados por los que puede pasar el mensaje
/// </summary>
public enum InboxRecordStatus { Scheduled, OnProcess, Processed, OnError }