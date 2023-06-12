using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Logging;

public static class LoggingExtensions
{

    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// Agrega el middleware como parte de las etapas de la canalizacion de la
    /// solicitud
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
        => app.UseMiddleware<LoggingMiddleware>();
}
