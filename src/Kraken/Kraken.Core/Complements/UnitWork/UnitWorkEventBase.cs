using Kraken.Core.EventBus;

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

    /// <summary>
    /// Indica la fecha en la que ocurrio el evento
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
