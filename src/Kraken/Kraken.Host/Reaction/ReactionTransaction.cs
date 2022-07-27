using Kraken.Core.Reaction;
using Kraken.Core.UnitWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    internal sealed class ReactionTransaction<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction>
        where TEvent : class, INotification
        where TReaction : INotificationHandler<TEvent>
    {
        /// <summary>
        /// Contiene la unidad de trabajo del modulo, este es provisto
        /// por el implementador, ya sea a traves de la inyeccion de dependencias
        /// o a traves de una fabrica
        /// </summary>
        private readonly IUnitWorkFactory _unitWorkFactory;

        /// <summary>
        /// Logger del middleware de transaccionalidad
        /// </summary>
        private readonly ILogger<ReactionTransaction<TEvent, TReaction>> _logger;

        /// <summary>
        /// Stream para la reaccion actual
        /// </summary>
        private readonly IReactionStream _reactionStream;

        public ReactionTransaction(IUnitWorkFactory unitWorkFactory,
            ILogger<ReactionTransaction<TEvent, TReaction>> logger, 
            IReactionStreamFactory reactionStreamFactory, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _unitWorkFactory = unitWorkFactory;
            _reactionStream = reactionStreamFactory.CreateReactionStream<TReaction>(serviceProvider);
        }

        /// <summary>
        /// Administra la transaccionalidad de los eventos
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task Handle(TEvent @event, ProcessRecord record, CancellationToken cancellationToken, EventHandlerDelegate next)
        {
            _logger.LogInformation("[Transaction] Get Unit of work from factory for tipe {type}. . .", typeof(TReaction));
            var unitWork = _unitWorkFactory.CreateUnitWork<TReaction>();
            try
            {
                if (unitWork is null)
                    throw new InvalidOperationException("Can not buit a unit work from current reaction");
                _logger.LogInformation("[TRANSACTION] Starting the transaction");
                await unitWork.StartTransaction();
                _logger.LogInformation("[TRANSACTION] Executing request");
                await next();
                _logger.LogInformation("[TRANSACTION] Update reaction record on stream");
                await _reactionStream.MarkReactionAsDone(record.Id);
                _logger.LogInformation("[TRANSACTION] Request completed successfully. Confirming changes");
                await unitWork.Commit();
                _logger.LogInformation("[TRANSACTION] Transaction ending.");
            }
            catch (InvalidOperationException ex)
            {
                // No podemos hacer nada y no se inicio operacion, entonces, devolvemos el error mas arriba
                throw;
            }
            catch (Exception ex)
            {
                // Si falla revertimos los cambios
                await unitWork.Rollback();
                // Loggeamos lo que sucede
                _logger.LogError("[ERROR] Error in transaction. Reverting changes due to: {error}", ex.StackTrace);
                // Lanzamos el error
                throw;
            }
        }
    }
}
