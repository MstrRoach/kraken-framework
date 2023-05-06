using System.Net;

namespace Kraken.Standard.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);