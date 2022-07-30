using System;

namespace Kraken.Core.Exceptions;

public interface IExceptionToResponseMapper
{
    ExceptionResponse Map(Exception exception);
}