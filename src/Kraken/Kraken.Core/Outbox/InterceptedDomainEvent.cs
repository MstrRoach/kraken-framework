using Kraken.Core.Mediator.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox;

/// <summary>
/// Evento que envuelve a los eventos de dominio para 
/// redistribuiurlos hacia la bandeja de salida transaccional
/// </summary>
public record InterceptedEvent : INotification
{
    /// <summary>
    /// Id del evento de intercepcion
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tipo del evento
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// El evento en si con la carga de datos
    /// </summary>
    public INotification Event { get; set; }

    /// <summary>
    /// Indica el modulo desde el cual se lanzo el evento
    /// </summary>
    public string SourceModule { get; set; }
}
