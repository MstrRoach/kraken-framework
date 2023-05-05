using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Request.Events;

/// <summary>
/// Handler para administrar los eventos de dominio
/// especificamente
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IDomainEventHandler<in TEvent> :
    INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
