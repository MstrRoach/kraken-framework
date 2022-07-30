using System.Net;

namespace Kraken.Core.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);