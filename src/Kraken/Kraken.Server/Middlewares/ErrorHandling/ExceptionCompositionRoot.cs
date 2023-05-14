using Kraken.Module.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Middlewares.ErrorHandling;

internal sealed class ExceptionCompositionRoot
{
    /// <summary>
    /// Lista de mapeadores
    /// </summary>
    readonly List<IExceptionToResponseMapper> _mappers;
    public ExceptionCompositionRoot(IEnumerable<IExceptionToResponseMapper> mappers)
    {
        _mappers = mappers.ToList();
    }

    /// <summary>
    /// Realiza el mapeo entre las excepciones capturadas a una respuesta
    /// especifica para ciertos tipos
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public ExceptionResponse Map(Exception exception)
    {
        // Obtenemos los mapeadores personalizados
        var nonDefaultMappers = _mappers.Where(x => x is not DefaultExceptionToResponseMapper);
        // Obtenemos un resultado de los mapeadores personalizados
        var result = nonDefaultMappers
            .Select(x => x.Map(exception))
            .SingleOrDefault(x => x is not null);
        // Si existe devolvemos el resultado
        if (result is not null)
            return result;
        // Si no, obtenemos el mapeador por defecto
        var defaultMapper = _mappers.SingleOrDefault(x => x is DefaultExceptionToResponseMapper);
        // Devolvemos el valor mapeado
        return defaultMapper?.Map(exception);
    }
}
