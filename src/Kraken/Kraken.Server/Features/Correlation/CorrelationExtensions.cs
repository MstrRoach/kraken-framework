using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Features.Correlation;

public static class CorrelationExtensions
{
    /// <summary>
    /// Llave con la que identificamos al id de correlacion
    /// </summary>
    private const string CorrelationIdKey = "correlation-id";

    /// <summary>
    /// Agrega el id de correlacion en la solicitud
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.Use((ctx, next) =>
        {
            ctx.Items.Add(CorrelationIdKey, Guid.NewGuid());
            return next();
        });

    /// <summary>
    /// Intenta recuperar el id de correlacion de la solicitud
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Guid? TryGetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(CorrelationIdKey, out var id) ? (Guid)id : null;
}
