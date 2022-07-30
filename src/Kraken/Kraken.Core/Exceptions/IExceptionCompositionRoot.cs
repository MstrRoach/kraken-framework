using Kraken.Core.Exceptions;
using System;

namespace Kraken.Host.Exceptions;

/// <summary>
/// Interface para mapear las excepciones a errores 
/// mas faciles de identificar
/// </summary>
public interface IExceptionCompositionRoot
{
    /// <summary>
    /// Mapea una excepcion hacia una respuesta mas
    /// amigable
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    ExceptionResponse Map(Exception exception);
}