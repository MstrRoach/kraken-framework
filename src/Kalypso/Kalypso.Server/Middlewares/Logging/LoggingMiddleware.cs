using Dottex.Kalypso.Module.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Logging;

/// <summary>
/// Middleware que registra informacion relevante para cada una
/// de las solicitudes que lleguen
/// </summary>
internal sealed class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    private string startTemplate = "=====> Started processing a request [Request ID: '{RequestId}', Correlation ID: '{CorrelationId}', Trace ID: '{TraceId}', User ID: '{UserId}']";
    private string finishTemplate = "<===== Finished processing a request with status code: {StatusCode} [Request ID: '{RequestId}', Correlation ID: '{CorrelationId}', Trace ID: '{TraceId}', User ID: '{UserId}']";

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Etapa de procesamiento del middleware
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext, IContext context,
        ILogger<LoggingMiddleware> logger)
    {
        // Loggeamos el inicio de la solicitud
        logger.LogInformation(startTemplate,
            context.RequestId,
            context.CorrelationId,
            context.TraceId,
            context.Identity.IsAuthenticated ? context.Identity.Id : string.Empty);
        // Ejecutamos el pipeline
        await _next(httpContext);
        // Logeamos el fin de la solicitud
        logger.LogInformation(finishTemplate,
            httpContext.Response.StatusCode,
            context.RequestId,
            context.CorrelationId,
            context.TraceId,
            context.Identity.IsAuthenticated ? context.Identity.Id : string.Empty);
    }
}
