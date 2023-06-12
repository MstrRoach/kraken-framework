using Dottex.Kalypso.Module.TransactionalReaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

internal sealed class ReactionConfirmationMiddleware<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction> where TEvent : class, INotification
        where TReaction : INotificationHandler<TEvent>
{
    /// <summary>
    /// Logger del middleware
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Administra la informacion de las reacciones
    /// </summary>
    private readonly IReactionStorage _storage;

    public ReactionConfirmationMiddleware(ILogger<ReactionConfirmationMiddleware<TEvent, TReaction>> logger,
        IReactionStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public async Task Handle(TEvent @event, ReactionMessage message, CancellationToken cancellationToken, EventHandlerDelegate next)
    {
        try
        {
            _logger.LogInformation("[REACTION MARKER] Start main process");
            await next();
            _logger.LogInformation("[REACTION MARKER] Main process done. Marking reaction as done");
            await _storage.Update(
                id: message.Id,
                status: ReactionRecordStatus.Processed,
                sentAt: DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("[REACTION MARKER] Error to process event");
            await _storage.Update(
                id: message.Id,
                status: ReactionRecordStatus.OnError,
                notes: ex.Message
                );
            throw;
        }
    }
}
