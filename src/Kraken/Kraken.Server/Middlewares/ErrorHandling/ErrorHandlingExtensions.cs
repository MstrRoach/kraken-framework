using Kraken.Module.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Middlewares.ErrorHandling;

public static class ErrorHandlingExtensions
{

    /// <summary>
    /// Agrega la administracion de errores a los servicios disponibles
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
        => services.AddSingleton<IExceptionToResponseMapper, DefaultExceptionToResponseMapper>()
            .AddSingleton<ExceptionCompositionRoot>();

    /// <summary>
    /// Usa la administracion en la canalizacion especificada
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ErrorHandlerMiddleware>();
}
