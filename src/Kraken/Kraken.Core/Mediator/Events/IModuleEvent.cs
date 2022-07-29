﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Mediator.Events
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
    }

}
