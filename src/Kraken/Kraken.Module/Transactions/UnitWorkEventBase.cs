using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Transactions;

/// <summary>
/// Evento base para los eventos de la unidad de trabajo
/// </summary>
public record UnitWorkEventBase : IArchEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nombre del componente que genera el evento
    /// </summary>
    public string Component => "UnitWork";

    /// <summary>
    /// Indica la fecha en la que ocurrio el evento
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
