using Kraken.Standard.Request;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Operation.Request;

internal sealed class CommandLoggingMiddleware<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ICommand<TResponse>
{
    private readonly ILogger<CommandLoggingMiddleware<TRequest, TResponse>> _logger;

    public CommandLoggingMiddleware(ILogger<CommandLoggingMiddleware<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Obtenemmos info de la solicitud
        var name = request.GetType().Name.Replace("Command", string.Empty);
        try
        {
            _logger.LogInformation($"[START] {name}");
            _logger.LogInformation("[PROPS] {@Command}", request);
            var response = await next();
            _logger.LogInformation($"[END] {name}");
            return response;
        }
        catch (Exception ex)
        {
            // Si falla logeamos el fallo explicitamente
            _logger.LogError(ex, "[ERROR] Command {Command} processing failed", name);
            throw;
        }
    }
}
