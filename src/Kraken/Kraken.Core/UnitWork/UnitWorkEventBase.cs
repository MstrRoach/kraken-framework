using Kraken.Core.Internal.Events;

namespace Kraken.Core.UnitWork;

/// <summary>
/// Evento base para los eventos de la unidad de trabajo
/// </summary>
public record UnitWorkEventBase : IComponentEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nombre del componente que genera el evento
    /// </summary>
    public string Component => "UnitWork";
}
