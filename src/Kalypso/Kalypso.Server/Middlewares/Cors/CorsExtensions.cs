using Dottex.Kalypso.Server.Middlewares.Cors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Cors;

public static class CorsExtensions
{

    /// <summary>
    /// Agrega los servicios de cors a los servicios disponibles
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services,
        Action<CorsOptions> setup)
    {
        var options = new CorsOptions();
        setup(options);
        services.AddSingleton(options);
        services.AddCors(cors =>
        {
            var allowedHeaders = options.allowedHeaders ?? Enumerable.Empty<string>();
            var allowedMethods = options.allowedMethods ?? Enumerable.Empty<string>();
            var allowedOrigins = options.allowedOrigins ?? Enumerable.Empty<string>();
            var exposedHeaders = options.exposedHeaders ?? Enumerable.Empty<string>();
            cors.AddPolicy(options.Name, corsBuilder =>
            {
                var origins = allowedOrigins.ToArray();
                if (options.allowCredentials && origins.FirstOrDefault() != "*")
                {
                    corsBuilder.AllowCredentials();
                }
                else
                {
                    corsBuilder.DisallowCredentials();
                }

                corsBuilder.WithHeaders(allowedHeaders.ToArray())
                    .WithMethods(allowedMethods.ToArray())
                    .WithOrigins(origins.ToArray())
                    .WithExposedHeaders(exposedHeaders.ToArray());
            });
        });
        return services;
    }

    /// <summary>
    /// Configura las cors en el pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<CorsOptions>();
        if(options is null)
            return app;
        app.UseCors(options.Name);
        return app;
    }

    /// <summary>
    /// Agrega la politica de cors al descriptor de la aplicacion
    /// </summary>
    /// <param name="Dottex.KalypsoOptions"></param>
    /// <param name="setup"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static ServerDescriptor AddCorsPolicy(this ServerDescriptor server,
        Action<CorsOptions> setup)
    {
        if (server.Cors is not null)
            throw new InvalidOperationException("Cors already been configured. See your kalypso host builder");
        // Si las opciones de cors son nulas lanzamos excepcion
        if (setup is null)
            throw new ArgumentNullException(nameof(setup));
        // Creamos las configuraciones por defecto
        var options = new CorsOptions();
        // Ejecutamos las configuraciones
        setup(options);
        // Setteammos la configuracion en las opciones de Dottex.Kalypso
        server.Cors = new CorsFeature(options);
        // Devolvemos las opciones de Dottex.Kalypso
        return server;
    }
}
