using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Request;

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

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Obtenemmos info de la solicitud
        var name = request.GetType().Name.Replace("Command", string.Empty);
        // Creamos la variable de solicitud detallada
        _timer.Start();
        var response = await next();
        _timer.Stop();
        if (_timer.ElapsedMilliseconds > 500)
        {
            _logger.LogWarning("[PERFORMANCE] {name} Execution time={ElapsedMilliseconds} milliseconds", name, _timer.ElapsedMilliseconds);
        }
        return response;
    }
}
