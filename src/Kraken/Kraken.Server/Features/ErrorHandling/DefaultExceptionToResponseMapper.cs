﻿using Kraken.Standard.Exceptions;
using Kraken.Standard.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Humanizer;

namespace Kraken.Server.Features.ErrorHandling;

internal sealed class DefaultExceptionToResponseMapper : IExceptionToResponseMapper
{
    private string InternalServerMessage = "Lo sentimos, ha ocurrido un error interno del servidor. Nuestro equipo de desarrollo está trabajando para resolver este problema lo antes posible. Por favor, inténtalo de nuevo más tarde. Si el problema persiste, no dudes en contactarnos para que podamos ayudarte a resolverlo. Gracias por tu paciencia y comprensión.";

    /// <summary>
    /// Mapeador por defecto
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            Exception ex => new ExceptionResponse(
                new Errors(new Error("InternalServer", InternalServerMessage)),
                HttpStatusCode.InternalServerError)
        };
}
