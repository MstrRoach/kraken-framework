using IdentityManagement.Infrastructure.Services.KrakenServices;
using IdentityManagement.Persistence;
using Kraken.Core.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityManagement
{
    public class IdentityModule : IModule
    {
        /// <summary>
        /// Nombre del modulo
        /// </summary>
        public string Name => "IdentityModule";

        /// <summary>
        /// Cadena de conexion para el modulo
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Registramos los servicios especificos del modulo
        /// </summary>
        /// <param name="services"></param>
        public void Register(IServiceCollection services)
        {
            // Configura las opciones para el modulo
            services.AddSingleton<IOptions<IdentityModule>>(Options.Create(this));
            services.AddScoped<IdentityUnitWork>();
            services.AddScoped<IdentityOutboxStore>();
            services.AddScoped<IdentityReactionStorage>();
            //services.Configure<IdentityModule>(options => options = this);
        }

        /// <summary>
        /// Incluimos los servicios en las canalizaciones de solicitud
        /// </summary>
        /// <param name="app"></param>
        public void Use(IApplicationBuilder app)
        {
            
        }
    }
}