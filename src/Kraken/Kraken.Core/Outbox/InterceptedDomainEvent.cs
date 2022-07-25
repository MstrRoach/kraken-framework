using Kraken.Core.Mediator;
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
public record InterceptedDomainEvent : INotification
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
    public IDomainEvent Event { get; set; }
}
