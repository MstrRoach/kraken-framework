using Kraken.Module.Inbox;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

internal sealed class InboxProcessingMiddleware<TEvent, THandler> : IInboxMiddleware<TEvent, THandler> where TEvent : class, INotification
        where THandler : INotificationHandler<TEvent>
{
    /// <summary>
    /// Logger del middleware
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Acceso al almacenamiento de los mensajes de entrada
    /// </summary>
    private readonly DefaultInboxStorageAccessor _storageAccessor;

    public InboxProcessingMiddleware(ILogger<InboxProcessingMiddleware<TEvent, THandler>> logger, DefaultInboxStorageAccessor storageAccessor)
    {
        _logger = logger;
        _storageAccessor = storageAccessor;
    }

    public async Task Handle(TEvent @event, InboxMessage processRecord, CancellationToken cancellationToken, EventHandlerDelegate next)
    {
        try
        {

        }catch (Exception ex)
        {
            //await _storageAccessor.Update
            throw;
        }
        throw new NotImplementedException();
    }
}
