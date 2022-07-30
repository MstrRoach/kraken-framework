using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal.Mediator;

/// <summary>
/// Middleware que nos permite realizar la medicion de tiempos en el que un commando se
/// ejecuta. Es opcional
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
internal sealed class PerformanceMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    /// <summary>
    /// Proveedor de logeo de serilog
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Timer para medir los tiempos de ejecucion
    /// </summary>
    private readonly Stopwatch _timer;

    public PerformanceMiddleware(ILogger<PerformanceMiddleware<TRequest, TResponse>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        // Obtenemmos info de la solicitud
        var name = request.GetType().Name;
        // Creamos la variable de solicitud detallada
        var requestTitle = $"{name}";
        _timer.Start();
        var response = await next();
        _timer.Stop();
        if (_timer.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning("[PERFORMANCE] {name} Execution time={ElapsedMilliseconds} milliseconds", requestTitle, _timer.ElapsedMilliseconds);
        }
        return response;
    }
}
