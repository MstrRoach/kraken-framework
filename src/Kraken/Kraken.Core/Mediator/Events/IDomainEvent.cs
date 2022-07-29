﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Mediator.Events
{
    /// <summary>
    /// Interface para definir los eventos de dominio
    /// </summary>
    public interface IDomainEvent : INotification
    {
        /// <summary>
        /// Id del evento de dominio
        /// </summary>
        Guid Id { get; }
    }
}
