using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.EventBus
{
    /// <summary>
    /// Interface para definir los eventos de dominio
    /// </summary>
    public interface IDomainEvent : IEvent
    {

    }
}
