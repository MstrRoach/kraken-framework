using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Middlewares.ErrorHandling;

/// <summary>
/// Middleware para la administracion de los errores
/// </summary>
internal sealed class ErrorHandlerMiddleware
{
    private readonly ExceptionCompositionRoot _exceptionCompositionRoot;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next,
        ILogger<ErrorHandlerMiddleware> logger,
        ExceptionCompositionRoot exceptionCompositionRoot)
    {
        _next = next;
        _logger = logger;
        _exceptionCompositionRoot = exceptionCompositionRoot;
    }

    /// <summary>
    /// Se invoca en la cadena de ejecucion
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    /// <summary>
    /// Realiza la administracion del error
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var errorResponse = _exceptionCompositionRoot.Map(exception);
        context.Response.StatusCode = (int)(errorResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        var response = errorResponse?.Response;
        if (response is null)
            return;
        await context.Response.WriteAsJsonAsync(response);
    }
}
