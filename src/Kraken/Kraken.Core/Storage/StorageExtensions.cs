using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Storage
{
    /// <summary>
    /// Extensiones para la seccion de storage
    /// </summary>
    public static class StorageExtensions
    {
        /// <summary>
        /// Agrega y configura el acceso a los datos relacionales de un modulo especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static IServiceCollection AddRelationalData<T>(this IServiceCollection services, Action<RelationalOptions<T>> setup)
            where T : IModule
        {
            // Registramos el default, como singlenton
            services.AddSingleton<IRelationalData<T>, DefaultRelationalData<T>>();
            // Configuramos las opciones relacionales
            services.Configure(setup);
            // Salimos
            return services;
        }
    }
}
