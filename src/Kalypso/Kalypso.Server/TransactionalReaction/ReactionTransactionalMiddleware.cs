using Dottex.Kalypso.Module;
using Dottex.Kalypso.Module.Request.Mediator;
using Dottex.Kalypso.Module.Transaction;
using Dottex.Kalypso.Module.TransactionalReaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

internal sealed class ReactionTransactionalMiddleware<TEvent, TReaction> : IReactionMiddleware<TEvent, TReaction> where TEvent : class, INotification
        where TReaction : INotificationHandler<TEvent>
{
    /// <summary>
    /// Logger del middleware
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Contiene la fabrica paara construir la unidad de trabajo
    /// especificaa para cada evento
    /// </summary>
    private readonly IUnitWorkFactory _unitWorkFactory;

    /// <summary>
    /// Bus de eventos de Kalypso
    /// </summary>
    IEventPublisher _eventPublisher;

    public ReactionTransactionalMiddleware(ILogger<ReactionTransactionalMiddleware<TEvent, TReaction>> logger,
        IUnitWorkFactory unitWorkFactory,
        IEventPublisher eventPublisher)
    {
        _unitWorkFactory = unitWorkFactory;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(TEvent @event, ReactionMessage processRecord, CancellationToken cancellationToken, EventHandlerDelegate next)
    {
        _logger.LogInformation("[REACTOR-TRANSACTION] >>>>>>>>>> Starting transaction process for {type}. . .", @event.GetType().Name.Replace("Command", string.Empty));
        _logger.LogInformation("[REACTOR-TRANSACTION] Getting unit work");
        var unitWork = _unitWorkFactory.CreateUnitWork<TEvent>();
        if (unitWork is null)
        {
            _logger.LogInformation("[REACTOR-TRANSACTION] Can not built a unit work from current command");
            throw new InvalidOperationException("Can not built a unit work from current command");
        }
        _logger.LogInformation("[REACTOR-TRANSACTION] Starting transaction");
        unitWork.StartTransaction();
        await _eventPublisher.Publish(new TransactionStarted(
            unitWork.TransactionId,
            processRecord.Reaction.GetModuleName()
            )
        );
        _logger.LogInformation("[REACTOR-TRANSACTION] Transaction started with id {id}", unitWork.TransactionId);
        try
        {
            _logger.LogInformation("[REACTOR-TRANSACTION] Running command handler");
            await next();
            _logger.LogInformation("[REACTOR-TRANSACTION] Command executed, confirming changes");
            await unitWork.Commit();
            await _eventPublisher.Publish(new TransactionCommited(
                unitWork.TransactionId,
                processRecord.Reaction.GetModuleName()
                )
            );
            _logger.LogInformation("[REACTOR-TRANSACTION] <<<<<<<<<< Confirmed changes, finish operation");
        }
        catch (Exception)
        {
            _logger.LogInformation("[REACTOR-TRANSACTION] Error in the execution of the command. Reverting changes");
            await unitWork.Rollback();
            await _eventPublisher.Publish(new TransacctionFailed(
                unitWork.TransactionId,
                processRecord.Reaction.GetModuleName()
                )
            );
            _logger.LogInformation("[REACTOR-TRANSACTION] <<<<<<<<<< Reverted changes, finishing operation");
            throw;
        }
    }
}
