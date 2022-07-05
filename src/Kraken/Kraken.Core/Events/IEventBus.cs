using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Events
{
    /// <summary>
    /// Componente que permite enviar eventos de kraken, a traves
    /// de la distribucion de eventos del mediador en turno
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publica un evento dentro de
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task PublishAsync(IKrakenEvent @event);
    }
}
