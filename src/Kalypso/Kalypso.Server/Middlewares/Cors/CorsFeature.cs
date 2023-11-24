using Dottex.Kalypso.Module.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Cors;

/// <summary>
/// Configura las politicas del Cors
/// </summary>
public class CorsFeature : IFeature
{
    /// <summary>
    /// Opciones para la caracteristica del host
    /// </summary>
    private readonly CorsOptions _options;

    /// <summary>
    /// Constructor de la caracteristica
    /// </summary>
    /// <param name="setup"></param>
    public CorsFeature(CorsOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Construye el feature a partir de una accion
    /// </summary>
    /// <param name="setup"></param>
    public CorsFeature(Action<CorsOptions> setup)
    {
        _options = new CorsOptions();
        setup(_options);
    }

    /// <summary>
    /// Configura los servicios para permitir los cors segun la
    /// configuracion pasada por constructor del feature
    /// </summary>
    /// <param name="services"></param>
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton(_options);
        services.AddCors(cors =>
        {
            var allowedHeaders = _options.allowedHeaders ?? Enumerable.Empty<string>();
            var allowedMethods = _options.allowedMethods ?? Enumerable.Empty<string>();
            var allowedOrigins = _options.allowedOrigins ?? Enumerable.Empty<string>();
            var exposedHeaders = _options.exposedHeaders ?? Enumerable.Empty<string>();
            cors.AddPolicy("kalypsocors", corsBuilder =>
            {
                var origins = allowedOrigins.ToArray();
                if (_options.allowCredentials && origins.FirstOrDefault() != "*")
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
    }

    /// <summary>
    /// Configura los servicios en la canalizacion de solicitudes
    /// </summary>
    /// <param name="app"></param>
    public void UseServices(IApplicationBuilder app)
    {
        app.UseCors("kalypsocors");
    }
}
