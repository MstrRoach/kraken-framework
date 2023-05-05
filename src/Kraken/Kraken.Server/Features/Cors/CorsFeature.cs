using Kraken.Standard.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Features.Cors
{
    /// <summary>
    /// Configura las politicas del Cors
    /// </summary>
    internal class CorsFeature : IFeature
    {
        /// <summary>
        /// Opciones para la caracteristica del host
        /// </summary>
        private readonly CorsOptions corsOptions;

        /// <summary>
        /// Constructor de la caracteristica
        /// </summary>
        /// <param name="setup"></param>
        public CorsFeature(CorsOptions options)
        {
            corsOptions = options;
        }

        /// <summary>
        /// Configura los servicios para permitir los cors segun la
        /// configuracion pasada por constructor del feature
        /// </summary>
        /// <param name="services"></param>
        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton(corsOptions);
            services.AddCors(cors =>
            {
                var allowedHeaders = corsOptions.allowedHeaders ?? Enumerable.Empty<string>();
                var allowedMethods = corsOptions.allowedMethods ?? Enumerable.Empty<string>();
                var allowedOrigins = corsOptions.allowedOrigins ?? Enumerable.Empty<string>();
                var exposedHeaders = corsOptions.exposedHeaders ?? Enumerable.Empty<string>();
                cors.AddPolicy("krakencors", corsBuilder =>
                {
                    var origins = allowedOrigins.ToArray();
                    if (corsOptions.allowCredentials && origins.FirstOrDefault() != "*")
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
            app.UseCors("krakencors");
        }
    }
}
