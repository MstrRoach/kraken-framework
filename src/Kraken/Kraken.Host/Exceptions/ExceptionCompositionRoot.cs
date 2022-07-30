using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Kraken.Core.Exceptions;

namespace Kraken.Host.Exceptions;

/// <summary>
/// Mapeador de excepciones hacia respuestas
/// </summary>
internal sealed class ExceptionCompositionRoot : IExceptionCompositionRoot
{
    private readonly IServiceProvider _serviceProvider;

    public ExceptionCompositionRoot(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Realiza el mapeo entre las excepciones capturadas a una respuesta
    /// especifica para ciertos tipos
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public ExceptionResponse Map(Exception exception)
    {
        using var scope = _serviceProvider.CreateScope();
        var mappers = scope.ServiceProvider.GetServices<IExceptionToResponseMapper>().ToArray();
        var nonDefaultMappers = mappers.Where(x => x is not ExceptionToResponseMapper);
        var result = nonDefaultMappers
            .Select(x => x.Map(exception))
            .SingleOrDefault(x => x is not null);

        if (result is not null)
        {
            return result;
        }

        var defaultMapper = mappers.SingleOrDefault(x => x is ExceptionToResponseMapper);

        return defaultMapper?.Map(exception);
    }
}