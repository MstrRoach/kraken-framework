using Kraken.Core.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProfileManagement.Infrastructure.Services.KrakenServices;
using ProfileManagement.Persistence;

namespace ProfileManagement
{
    public class ProfileModule : IModule
    {
        /// <summary>
        /// Nombre del modulo
        /// </summary>
        public string Name => "ProfileModule";

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
            services.AddSingleton<IOptions<ProfileModule>>(Options.Create(this));
            services.AddScoped<ProfileUnitWork>();
            services.AddScoped<ProfileOutboxStore>();
            //services.AddScoped<ProfileReactionStorage>();

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