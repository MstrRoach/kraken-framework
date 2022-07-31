using Kraken.Core.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Logging
{
    /// <summary>
    /// Extensiones para la configuracion y el uso del loggin dentro
    /// de la aplicacion
    /// </summary>
    public static class LoggingExtensions
    {

        /// <summary>
        /// Agrega el middleware de logging a la canalizacion de 
        /// las solicitudes
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLogging(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                // Obtenemos el logger
                var logger = ctx.RequestServices.GetRequiredService<ILogger<IContext>>();
                // Obtenemos el contexto de la aplicacion
                var context = ctx.RequestServices.GetRequiredService<IContext>();
                // Loggeamos el inicio de la solicitud
                logger.LogInformation("Started processing a request [Request ID: '{RequestId}', Correlation ID: '{CorrelationId}', Trace ID: '{TraceId}', User ID: '{UserId}']...",
                    context.RequestId, context.CorrelationId, context.TraceId, context.Identity.IsAuthenticated ? context.Identity.Id : string.Empty);
                // Ejecutamos el pipeline
                await next();
                // Logeamos el fin de la solicitud
                logger.LogInformation("Finished processing a request with status code: {StatusCode} [Request ID: '{RequestId}', Correlation ID: '{CorrelationId}', Trace ID: '{TraceId}', User ID: '{UserId}']",
                    ctx.Response.StatusCode, context.RequestId, context.CorrelationId, context.TraceId, context.Identity.IsAuthenticated ? context.Identity.Id : string.Empty);
            });
            return app;
        }
    }
}
