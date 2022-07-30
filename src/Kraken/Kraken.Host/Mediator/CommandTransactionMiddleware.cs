using Kraken.Core.Mediator;
using Kraken.Core.Transaction;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Mediator
{
    internal sealed class CommandTransactionMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICommand<TResponse>
    {
        /// <summary>
        /// Contiene la unidad de trabajo del modulo, este es provisto
        /// por el implementador, ya sea a traves de la inyeccion de dependencias
        /// o a traves de una fabrica
        /// </summary>
        private readonly IUnitWorkFactory _unitWorkFactory;

        /// <summary>
        /// logger para el middleware
        /// </summary>
        private readonly ILogger<CommandTransactionMiddleware<TRequest, TResponse>> _logger;

        /// <summary>
        /// Constuctor del 
        /// </summary>
        /// <param name="unitWork"></param>
        public CommandTransactionMiddleware(IUnitWorkFactory unitWorkFactory,
            ILogger<CommandTransactionMiddleware<TRequest, TResponse>> logger)
        {
            _unitWorkFactory = unitWorkFactory;
            _logger = logger;
        }

        /// <summary>
        /// Administra la transaccionalidad
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("[TRANSACTION] Start transaction processing for {type}. . .", request.GetType());
            var unitWork = _unitWorkFactory.CreateUnitWork<TRequest>();
            if (unitWork is null)
            {
                _logger.LogInformation("[TRANSACTION] Can not built a unit work from current command");
                throw new InvalidOperationException("Can not built a unit work from current command");
            }
            var result = await unitWork.ExecuteAsync(async () => await next());
            _logger.LogInformation("[TRANSACTION] Transaction process ending.");
            return result;
        }
    }
}

