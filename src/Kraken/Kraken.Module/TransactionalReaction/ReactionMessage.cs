using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.TransactionalReaction;

public class ReactionMessage
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
    /// Indica el tipo del evento
    /// </summary>
    public object Event { get; set; }

    /// <summary>
    /// Indica quien responde al evento
    /// </summary>
    public Type Reaction { get; set; }

}
