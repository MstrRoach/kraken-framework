using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Events;

/// <summary>
/// Define el contrato base para todos los
/// eventos de kraken
/// </summary>
public interface IKrakenEvent : INotification
{

}

/// <summary>
/// Define un evento de dominio generado a partir 
/// de las modificaciones a un agregado y distribuido 
/// unicamente en el alcance local del modulo
/// </summary>
public class DomainEvent : IKrakenEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Indica la fecha en la que se ejecuto el evento
    /// </summary>
    public DateTime OcurredOn { get; set; } = DateTime.UtcNow;
}

///// <summary>
///// Marcador para un evento generado con alcance modular
///// y que debe de ser distribuido tanto localmente como
///// intermodularmente para aplicar cambios en todo el host. 
///// La definicion de los eventos de este tipo sera en un 
///// ensamblado al que todos los modulos hacen referencia
///// para aislar la ejecucion de los evento
///// </summary>
//public class ModuleEvent : IKrakenEvent
//{
//    /// <summary>
//    /// Id del evento
//    /// </summary>
//    public Guid Id { get; set; } = Guid.NewGuid();

//    /// <summary>
//    /// Indica la fecha en la que se ejecuto el evento
//    /// </summary>
//    public DateTime OcurredOn { get; set; } = DateTime.UtcNow;
//}

///// <summary>
///// Marcador para un evento generado con alcance fuera del host.
///// Estos eventos sirven para comunicarse con otros host. Para estos
///// es necesario utilizar un bus de eventos fisicos pues esta pensado
///// para que sirva como integracion entre multiples host. La definicion
///// de estos sera en un ensamblado diferente y que debe de compartirse 
///// con todos los host que se escuchan entre si.
///// </summary>
//public class IntegrationEvent : IKrakenEvent
//{
//    /// <summary>
//    /// Id del evento
//    /// </summary>
//    public Guid Id { get; set; } = Guid.NewGuid();

//    /// <summary>
//    /// Indica la fecha en la que se ejecuto el evento
//    /// </summary>
//    public DateTime OcurredOn { get; set; } = DateTime.UtcNow;
//}
