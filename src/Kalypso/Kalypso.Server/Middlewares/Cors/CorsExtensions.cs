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
}
