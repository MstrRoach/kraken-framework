using Kraken.Core.Reaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    internal class ReactionMarkingMiddleware<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction>
        where TEvent : class, INotification
        where TReaction : INotificationHandler<TEvent>
    {
        /// <summary>
        /// Stream para la reaccion actual
        /// </summary>
        private readonly IReactionStream _reactionStream;

        /// <summary>
        /// Logger del middleware
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reactionStreamFactory"></param>
        /// <param name="serviceProvider"></param>
        public ReactionMarkingMiddleware(IReactionStreamFactory reactionStreamFactory, 
            IServiceProvider serviceProvider, 
            ILogger<ReactionMarkingMiddleware<TEvent, TReaction>> logger)
        {
            _reactionStream = reactionStreamFactory.CreateReactionStream<TReaction>(serviceProvider);
            _logger = logger;
        }

        /// <summary>
        /// Administrador para marcar las reacciones como processadas
        /// </summary>
        /// <param name="event"></param>
        /// <param name="processRecord"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Handle(TEvent @event, ProcessRecord record, CancellationToken cancellationToken, EventHandlerDelegate next)
        {
            _logger.LogInformation("[REACTION MARKER] Start main process");
            await next();
            _logger.LogInformation("[REACTION MARKER] Main process done. Marking reaction as done");
            await _reactionStream.MarkReactionAsDone(record.Id);
            _logger.LogInformation("[REACTION MARKER] Reaction has marked. Well done!!");
        }
    }
}
