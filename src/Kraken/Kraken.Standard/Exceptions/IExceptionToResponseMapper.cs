﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Exceptions;

/// <summary>
/// Define un mappeador para las excepciones basados en el tipo
/// de excepcion para una respuesta detallada
/// </summary>
public interface IExceptionToResponseMapper
{
    /// <summary>
    /// Mapea una excepcion a una respuesta detallada
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    ExceptionResponse Map(Exception exception);
}
