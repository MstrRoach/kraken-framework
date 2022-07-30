using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.Events
{
    /// <summary>
    /// Interface base para los eventos dentro generados dentro de
    /// de kraken.
    /// </summary>
    public interface IModuleEvent : INotification
    {
        /// <summary>
        /// Id del evento
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Fecha en la que ocurrio el evento
        /// </summary>
        DateTime OccurredOn { get; }
    }

}
