using Kraken.Core.Commands;
using Kraken.Core.UnitWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.UnitWork
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
        public CommandTransactionMiddleware(IUnitWorkFactory unitWorkFactory, ILogger<CommandTransactionMiddleware<TRequest, TResponse>> logger, IServiceProvider serviceProvider)
        {
            _unitWorkFactory = unitWorkFactory;
            _logger = logger;
            var ddd = serviceProvider.GetServices<INotificationHandler<TransactionStarted>>();
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
            _logger.LogInformation("[Transaction] Get Unit of work from factory for tipe {type}. . .", request.GetType());
            var unitWork = _unitWorkFactory.CreateUnitWork<TRequest>();
            try
            {
                if (unitWork is null)
                    throw new InvalidOperationException("Can not buit a unit work from current command");
                _logger.LogInformation("[TRANSACTION] Starting the transaction");
                await unitWork.StartTransaction();
                _logger.LogInformation("[TRANSACTION] Executing request");
                var result = await next();
                _logger.LogInformation("[TRANSACTION] Request completed successfully. Confirming changes");
                await unitWork.Commit();
                _logger.LogInformation("[TRANSACTION] Transaction ending.");
                return result;
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

