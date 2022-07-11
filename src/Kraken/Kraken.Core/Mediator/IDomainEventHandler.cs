using Kraken.Core.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Mediator
{
    /// <summary>
    /// Handler para administrar los eventos de dominio
    /// especificamente
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventHandler<in TEvent> : 
        INotificationHandler<IDomainEvent> 
        where TEvent : IDomainEvent 
    {
    }
}
