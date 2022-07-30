using System;
using System.Collections.Concurrent;
using System.Net;
using Humanizer;
using Kraken.Core.Exceptions;

namespace Kraken.Host.Exceptions;

/// <summary>
/// Realiza el mapeo de las excepciones a respuestas
/// </summary>
internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    /// <summary>
    /// Diccionario de codigos
    /// </summary>
    private static readonly ConcurrentDictionary<Type, string> Codes = new();

    /// <summary>
    /// Mapea la excepcion a una respuesta
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            KrakenException ex => new ExceptionResponse(new ErrorsResponse(new Error(GetErrorCode(ex), ex.Message))
                , HttpStatusCode.BadRequest),
            _ => new ExceptionResponse(new ErrorsResponse(new Error("error", "There was an error.")),
                HttpStatusCode.InternalServerError)
        };

    private record Error(string Code, string Message);

    private record ErrorsResponse(params Error[] Errors);

    private static string GetErrorCode(object exception)
    {
        var type = exception.GetType();
        return Codes.GetOrAdd(type, type.Name.Underscore().Replace("_exception", string.Empty));
    }
}