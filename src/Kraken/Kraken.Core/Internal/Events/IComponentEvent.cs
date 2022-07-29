using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.Events
{
    /// <summary>
    /// Interface para definir los eventos de arquitectura, que permiten
    /// realizar un conjunto de operaciones de coordinacion entre componentes
    /// internos del sistema
    /// </summary>
    public interface IComponentEvent : INotification
    {
        /// <summary>
        /// Id del evento de arquitectura
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Nombre del componente que emite el evento
        /// </summary>
        string Component { get; }

    }
}
