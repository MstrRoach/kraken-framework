using Kraken.Core.Internal.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Mediator
{
    public interface IComponentEventHandler<in TEvent> :
        INotificationHandler<TEvent>
        where TEvent : IComponentEvent
    {
    }
}
