using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Contexts;

public static class ContextExtensions
{
    /// <summary>
    /// Llave con la que se obtiene el encabezado para obtener
    /// la direccion ip del cliente
    /// </summary>
    private static string HeaderIpAddressKey = "x-forwarded-for";

    /// <summary>
    /// Agrega los servicios necesarios para la generacion del contexto
    /// </summary>
    /// <param name="services"></param>
    /// <param name="IdentityContextBuilderType"></param>
    public static void AddContext(this IServiceCollection services,
        IdentityContextProperties identityContextProperties)
    {
        services.AddSingleton<ContextProvider>();
        services.AddSingleton<DefaultIdentityContextBuilder>();
        services.AddSingleton(identityContextProperties);
        services.AddTransient(sp => sp.GetRequiredService<ContextProvider>().Context);
    }

    /// <summary>
    /// Agregar el contexto en la canalizacion de las capas de asp
    /// </summary>
    /// <param name="app"></param>
    public static void UseContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            // Obtener constructor de identidad
            var identityBuilder = ctx.RequestServices.GetRequiredService<DefaultIdentityContextBuilder>();
            // Obtener proveedor de contexto y setear el contexto
            ctx.RequestServices
            .GetRequiredService<ContextProvider>()
            .Context = new DefaultContext(ctx, identityBuilder);
            // Continuar con la operacion
            return next();
        });
    }

    /// <summary>
    /// Obtiene la ip del origen de la solicitud
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUserIpAddress(this HttpContext context)
    {
        if (context is null)
        {
            return string.Empty;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (context.Request.Headers.TryGetValue(HeaderIpAddressKey, out var forwardedFor))
        {
            var ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (ipAddresses.Any())
            {
                ipAddress = ipAddresses[0];
            }
        }

        return ipAddress ?? string.Empty;
    }
}
