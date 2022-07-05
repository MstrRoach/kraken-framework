using Inflow.Shared.Infrastructure.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kraken.Host.Contexts;

public static class ContextExtensions
{
    /// <summary>
    /// Agrega el accesor de contexto a los servicios disponibles
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.AddSingleton<ContextAccessor>();
        services.AddTransient(sp => sp.GetRequiredService<ContextAccessor>().Context);

        return services;
    }

    /// <summary>
    /// Agrega la obtencion del contexto en la canalizacion de la solicitud
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            ctx.RequestServices.GetRequiredService<ContextAccessor>().Context = new Context(ctx); ;

            return next();
        });

        return app;
    }
}