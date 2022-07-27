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
    internal sealed class ReactionLogging<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction>
        where TEvent : class, INotification
        where TReaction : INotificationHandler<TEvent>
    {
        /// <summary>
        /// Logger del middleware
        /// </summary>
        private readonly ILogger<ReactionLogging<TEvent, TReaction>> _logger;

        /// <summary>
        /// Constructor del middleware
        /// </summary>
        /// <param name="logger"></param>
        public ReactionLogging(ILogger<ReactionLogging<TEvent, TReaction>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Ejecuta el middleware
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Handle(TEvent @event, CancellationToken cancellationToken, EventHandlerDelegate next)
        {
            // Obtenemmos info de la solicitud
            var name = typeof(TReaction).Name;
            // Creamos la variable de solicitud detallada
            var eventTitle = $"{name}";
            try
            {
                _logger.LogInformation($"[REACTION] Publishing and handler event with reaction {name}");
                _logger.LogInformation($"[START] {eventTitle}");
                _logger.LogInformation("[PROPS] {@Event}", @event);
                await next();
                _logger.LogInformation($"[END] {eventTitle}");
            }
            catch (Exception ex)
            {
                // Si falla logeamos el fallo explicitamente
                _logger.LogError(ex, "[ERROR] Event {Event} processing failed", eventTitle);
                throw;
            }
        }
    }
}
