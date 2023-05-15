using Kraken.Module.Inbox;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

internal sealed class InboxLoggingMiddleware<TEvent, THandler> : IInboxMiddleware<TEvent, THandler>
        where TEvent : class, INotification
        where THandler : INotificationHandler<TEvent>
{
    private readonly ILogger _logger;

    public InboxLoggingMiddleware(ILogger<InboxLoggingMiddleware<TEvent, THandler>> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TEvent @event, InboxMessage processRecord, CancellationToken cancellationToken, EventHandlerDelegate next)
    {
        // Obtenemmos info de la solicitud
        var name = @event.GetType().Name.Replace("Command", string.Empty);
        try
        {
            _logger.LogInformation($"[START] {name}");
            _logger.LogInformation("[PROPS] {@Command}", @event);
            await next();
            _logger.LogInformation($"[END] {name}");
        }
        catch (Exception ex)
        {
            // Si falla logeamos el fallo explicitamente
            _logger.LogError(ex, "[ERROR] Command {Command} processing failed", name);
            throw;
        }
    }
}
