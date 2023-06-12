using Dottex.Kalypso.Module.TransactionalReaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

/// <summary>
/// Middleware que agrega una capa de logeo para la trazabilidad
/// </summary>
internal sealed class ReactionLoggingMiddleware<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction>
    where TEvent : class, INotification
        where TReaction : INotificationHandler<TEvent>
{
    private readonly ILogger _logger;

    public ReactionLoggingMiddleware(ILogger<ReactionLoggingMiddleware<TEvent, TReaction>> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TEvent @event, ReactionMessage processRecord, CancellationToken cancellationToken, EventHandlerDelegate next)
    {
        // Obtenemmos info de la solicitud
        var name = @event.GetType().Name.Replace("Event", string.Empty);
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
