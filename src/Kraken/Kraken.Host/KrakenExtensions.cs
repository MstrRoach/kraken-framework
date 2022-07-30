using Kraken.Core.Internal;
using Kraken.Core.Internal.Serializer;
using Kraken.Core.UnitWork;
using Kraken.Host.Contexts;
using Kraken.Host.Internal;
using Kraken.Host.Internal.Mediator;
using Kraken.Host.Internal.Serializer;
using Kraken.Host.Modules;
using Kraken.Host.Outbox;
using Kraken.Host.Reaction;
using Kraken.Host.UnitWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Kraken.Host;

public static class KrakenExtensions
{
    /// <summary>
    /// Llave con la que identificamos al id de correlacion
    /// </summary>
    private const string CorrelationIdKey = "correlation-id";

    /// <summary>
    /// Agrega todos los servicios de kraken y los modulos de kraken
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IServiceCollection AddKraken(this IServiceCollection services, IConfiguration configuration ,Action<KrakenOptions> setup)
    {
        // Creamos las configuraciones
        KrakenOptions krakenOptions = new();
        setup(krakenOptions);
        // Las registramos como singlenton
        services.AddSingleton(krakenOptions);
        // ------------------------- Configuracion de las partes centrales de kraken
        services.AddInternalKernel(krakenOptions.assemblies);
        //services.AddMediator(krakenOptions.assemblies);
        services.AddModuleInfo(krakenOptions.modules);
        services.AddContext();
        // ------------------------- Configuracion de las partes opcionales de kraken
        // Agrega las operaciones de transaccionalidad
        services.AddUnitWorks(krakenOptions.assemblies);
        // Agrega soporte de bandeja de salida para los eventos asincronos
        services.AddOutbox(krakenOptions.assemblies);
        // Agrega soporte para las reacciones
        //services.AddReactions(krakenOptions.assemblies);
        // ------------------------- Configuracion de los modulos
        // Obtenemos las configuraciones de todos los modulos
        krakenOptions.modules.ForEach(module => configuration.GetSection(module.Name).Bind(module));
        // Registramos los modulos
        krakenOptions.modules.ForEach(module => module.Register(services));
        // Configuramos el arrancador
        services.AddSingleton<KrakenBootstrapper>()
            .AddSingleton<IBootstrapper>(x => x.GetRequiredService<KrakenBootstrapper>())
            .AddHostedService(x => x.GetRequiredService<KrakenBootstrapper>());
        // Devolvemos los servicios
        return services;
    }

    /// <summary>
    /// Agrega los servicios a la canalizacion de solicitudes del host
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseKraken(this IApplicationBuilder app)
    {
        // Agregamos los componentes para la canalizacion de solicitudes

        // Agrega el id de correlacion al componente principal
        app.UseCorrelationId();

        // Agrega la extraccion de contextos
        app.UseContext();

        // Agregamos el middleware para el routeo de la aplicacion
        app.UseRouting();

        // Agregamos las configuraciones de los modulos al pipeline
        var krakenOptions = app.ApplicationServices.GetRequiredService<KrakenOptions>();
        krakenOptions?.modules.ForEach(module => module.Use(app));
        krakenOptions?.Clear();

        // Agregamos el mapeo de endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", context => context.Response.WriteAsync("Kraken Example API"));
            endpoints.MapModuleInfo();
        });

        // Devolvemos la applicacion
        return app;
    }

    

    /// <summary>
    /// Intenta recuperar el id de correlacion de la solicitud
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Guid? TryGetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(CorrelationIdKey, out var id) ? (Guid)id : null;

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
        if (context.Request.Headers.TryGetValue("x-forwarded-for", out var forwardedFor))
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
