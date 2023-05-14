using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Reaction;

/// <summary>
/// Registro con el evento y la reaccion que debe procesar
/// </summary>
public class ReactionMessage
{
    /// <summary>
    /// Id de la reaccion que debe de ser unica
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id de correlacion para la reaccion
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Id del evento que generó la reaccion
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Indica cual es el evento asociado a la reaccion
    /// </summary>
    public Type Event { get; set; }

    /// <summary>
    /// Objeto que contiene la reaccion que debe de ejecutarse
    /// </summary>
    public Type Reaction { get; set; }
}
