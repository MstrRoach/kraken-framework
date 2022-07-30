using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kraken.Host.Exceptions;

/// <summary>
/// Middleware para la administracion de los errores
/// </summary>
internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    private readonly IExceptionCompositionRoot _exceptionCompositionRoot;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(IExceptionCompositionRoot exceptionCompositionRoot,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _exceptionCompositionRoot = exceptionCompositionRoot;
        _logger = logger;
    }

    /// <summary>
    /// Se invoca en la cadena de ejecucion
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
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
        {
            return;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}