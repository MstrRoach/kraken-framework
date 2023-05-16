using Kraken.Module.Inbox;
using Kraken.Module.Request.Mediator;
using Kraken.Module.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

internal sealed class InboxTransactionMiddleware<TEvent, THandler> : IInboxMiddleware<TEvent, THandler> where TEvent : class, INotification
        where THandler : INotificationHandler<TEvent>
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
    /// Bus de eventos de kraken
    /// </summary>
    IEventPublisher _eventPublisher;

    /// <summary>
    /// Constructor de software
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitWorkFactory"></param>
    /// <param name="eventPublisher"></param>
    public InboxTransactionMiddleware(ILogger<InboxTransactionMiddleware<TEvent, THandler>> logger, IUnitWorkFactory unitWorkFactory, IEventPublisher eventPublisher)
    {
        _unitWorkFactory = unitWorkFactory;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(TEvent @event, InboxMessage processRecord, CancellationToken cancellationToken, EventHandlerDelegate next)
    {
        _logger.LogInformation("[TRANSACTION] >>>>>>>>>> Starting transaction process for {type}. . .", @event.GetType().Name.Replace("Command", string.Empty));
        _logger.LogInformation("[TRANSACTION] Getting unit work");
        var unitWork = _unitWorkFactory.CreateUnitWork<TEvent>();
        if (unitWork is null)
        {
            _logger.LogInformation("[TRANSACTION] Can not built a unit work from current command");
            throw new InvalidOperationException("Can not built a unit work from current command");
        }
        _logger.LogInformation("[TRANSACTION] Starting transaction");
        unitWork.StartTransaction();
        //await _eventPublisher.Publish(new TransactionStarted(unitWork.TransactionId));
        _logger.LogInformation("[TRANSACTION] Transaction started with id {id}", unitWork.TransactionId);
        try
        {
            _logger.LogInformation("[TRANSACTION] Running command handler");
            await next();
            _logger.LogInformation("[TRANSACTION] Command executed, confirming changes");
            await unitWork.Commit();
            //await _eventPublisher.Publish(new TransactionCommited(unitWork.TransactionId));
            _logger.LogInformation("[TRANSACTION] <<<<<<<<<< Confirmed changes, finish operation");
        }
        catch (Exception)
        {
            _logger.LogInformation("[TRANSACTION] Error in the execution of the command. Reverting changes");
            await unitWork.Rollback();
            //await _eventPublisher.Publish(new TransacctionFailed(unitWork.TransactionId));
            _logger.LogInformation("[TRANSACTION] <<<<<<<<<< Reverted changes, finishing operation");
            throw;
        }
    }
}
