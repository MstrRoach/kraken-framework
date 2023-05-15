using Kraken.Module;
using Kraken.Module.Request.Mediator;
using Kraken.Module.Transactions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Transaction;

internal sealed class TransactionMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class, ICommand<TResponse>
{
    /// <summary>
    /// Contiene la fabrica para construir la unidad de trabajo especifica para
    /// cada uno de los comandos que entran en el host
    /// </summary>
    private readonly IUnitWorkFactory _unitWorkFactory;

    /// <summary>
    /// logger para el middleware
    /// </summary>
    private readonly ILogger<TransactionMiddleware<TRequest, TResponse>> _logger;

    /// <summary>
    /// Bus de eventos de kraken
    /// </summary>
    IEventPublisher _eventPublisher;

    /// <summary>
    /// Constructor del middleware
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitWorkFactory"></param>
    public TransactionMiddleware(ILogger<TransactionMiddleware<TRequest, TResponse>> logger, IUnitWorkFactory unitWorkFactory, IEventPublisher eventPublisher)
    {
        _unitWorkFactory = unitWorkFactory;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[TRANSACTION] >>>>>>>>>> Starting transaction process for {type}. . .", request.GetType().Name.Replace("Command", string.Empty));
        _logger.LogInformation("[TRANSACTION] Getting unit work");
        var unitWork = _unitWorkFactory.CreateUnitWork<TRequest>();
        if (unitWork is null)
        {
            _logger.LogInformation("[TRANSACTION] Can not built a unit work from current command");
            throw new InvalidOperationException("Can not built a unit work from current command");
        }
        _logger.LogInformation("[TRANSACTION] Starting transaction");
        unitWork.StartTransaction();
        await _eventPublisher.Publish(new TransactionStarted(unitWork.TransactionId, request.GetModuleName()));
        _logger.LogInformation("[TRANSACTION] Transaction started with id {id}", unitWork.TransactionId);
        try
        {
            _logger.LogInformation("[TRANSACTION] Running command handler");
            var response = await next();
            _logger.LogInformation("[TRANSACTION] Command executed, confirming changes");
            await unitWork.Commit();
            await _eventPublisher.Publish(new TransactionCommited(unitWork.TransactionId, request.GetModuleName()));
            _logger.LogInformation("[TRANSACTION] <<<<<<<<<< Confirmed changes, finish operation");
            return response;
        }
        catch (Exception)
        {
            _logger.LogInformation("[TRANSACTION] Error in the execution of the command. Reverting changes");
            await unitWork.Rollback();
            await _eventPublisher.Publish(new TransacctionFailed(unitWork.TransactionId, request.GetModuleName()));
            _logger.LogInformation("[TRANSACTION] <<<<<<<<<< Reverted changes, finishing operation");
            throw;
        }
    }
}
