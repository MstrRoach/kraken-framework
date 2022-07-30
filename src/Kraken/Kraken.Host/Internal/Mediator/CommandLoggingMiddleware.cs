using Kraken.Core.Internal.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal.Mediator
{
    /// <summary>
    /// Middleware que nos permite registrar cada comando a traves del logger 
    /// configurado para el sistema
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal sealed class CommandLoggingMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICommand<TResponse>
    {

        private readonly ILogger<CommandLoggingMiddleware<TRequest, TResponse>> _logger;

        public CommandLoggingMiddleware(ILogger<CommandLoggingMiddleware<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // Obtenemmos info de la solicitud
            var name = request.GetType().Name;
            // Creamos la variable de solicitud detallada
            var requestTitle = $"{name}";
            try
            {
                _logger.LogInformation($"[START] {requestTitle}");
                _logger.LogInformation("[PROPS] {@Command}", request);
                var response = await next();
                _logger.LogInformation($"[END] {requestTitle}");
                return response;
            }
            catch (Exception ex)
            {
                // Si falla logeamos el fallo explicitamente
                _logger.LogError(ex, "[ERROR] Command {Command} processing failed", requestTitle);
                throw;
            }
        }
    }
}
