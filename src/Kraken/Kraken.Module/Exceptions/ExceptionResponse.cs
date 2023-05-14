using System.Net;

namespace Kraken.Module.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);