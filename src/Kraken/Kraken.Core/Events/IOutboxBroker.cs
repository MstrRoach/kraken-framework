using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Events
{
    /// <summary>
    /// Componente encargado de administrar las bandejas de
    /// salida para cada modulo registrado. Es quien decide 
    /// a donde registrar cada mensaje segun el modulo proveniente.
    /// Esto en caso de
    /// </summary>
    public interface IOutboxBroker
    {
        /// <summary>
        /// Envia un evento a la bandeja de salida correspondiente
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task SendAsync(IKrakenEvent @event);
    }
}
