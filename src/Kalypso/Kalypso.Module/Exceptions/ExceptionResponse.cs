using System.Net;

namespace Dottex.Kalypso.Module.Exceptions;

public record ExceptionResponse(object Response, HttpStatusCode StatusCode);