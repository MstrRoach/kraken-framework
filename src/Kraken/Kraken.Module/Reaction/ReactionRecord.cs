using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Reaction;

/// <summary>
/// Define el registro utilizado para guardar una reaccion
/// en el sistema de almacenamiento predeterminado
/// </summary>
public class ReactionRecord
{
    /// <summary>
    /// Id de la reaccion
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id de correlacion para la reaccion
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Id del evento que genero la reaccion
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Tipo del evento asociado a la reaccion
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Nombre de la reaccion
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Typo de la reaccion
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
    public ReactionRecordStatus Status { get; set; } = ReactionRecordStatus.Scheduled;
}

/// <summary>
/// Estados por los que puede pasar el mensaje
/// </summary>
public enum ReactionRecordStatus { Scheduled, OnProcess, Processed, OnError }