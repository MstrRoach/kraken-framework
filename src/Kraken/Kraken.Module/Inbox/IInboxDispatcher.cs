using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Inbox;

/// <summary>
/// Componente encargado de distribuir
/// </summary>
public interface IInboxDispatcher
{
    /// <summary>
    /// Procesa un mensaje
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ProcessAsync(InboxMessage message, INotification @event, CancellationToken cancellationToken = default);
}
