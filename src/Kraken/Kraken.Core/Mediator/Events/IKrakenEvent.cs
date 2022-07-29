using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Mediator.Events
{
    /// <summary>
    /// Interface para los eventos internos
    /// </summary>
    public interface IKrakenEvent : INotification
    {
    }
}
