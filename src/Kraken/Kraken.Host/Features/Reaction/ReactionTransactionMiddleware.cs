using Kraken.Core.Reaction;
using Kraken.Core.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    internal sealed class ReactionTransactionMiddleware<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction>
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
        private readonly ILogger<ReactionTransactionMiddleware<TEvent, TReaction>> _logger;

        /// <summary>
        /// Stream para la reaccion actual
        /// </summary>
        private readonly IReactionStream _reactionStream;

        public ReactionTransactionMiddleware(IUnitWorkFactory unitWorkFactory,
            ILogger<ReactionTransactionMiddleware<TEvent, TReaction>> logger, 
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
            if (unitWork is null)
            {
                _logger.LogInformation("[TRANSACTION] Can not built a unit work from current command");
                throw new InvalidOperationException("Can not built a unit work from current command");
            }
            // La llamada de la funcion debe de ser hecha con el await interno, para que el sistema
            // no piense que se debe de generar en otro hilo aparte, sino mas bien debe de generarlo
            // dentro del mismo hilo de ejecucion para contener los servicios de ambito creados a
            // partir de la construccion del event handler
            await unitWork.ExecuteAsync(async () => await next());
            _logger.LogInformation("[TRANSACTION] Transaction process ending.");

        }
    }
}
