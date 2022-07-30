using Kraken.Core.Internal.Transaction;
using Kraken.Host.Internal.Mediator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.UnitWork
{
    public static class UnitWorkExtensions
    {

        /// <summary>
        /// Agrega el soporte para las unidades de trabajo y el
        /// middleware necesario para envolver los comandos en 
        /// la transaccionalidad 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitWorks(this IServiceCollection services, List<Assembly> assemblies)
        {
            // Registramos la fabrica para las unidades de trabajo
            services.AddScoped<IUnitWorkFactory, UnitWorkFactory>();
            // Buscamos todas las unidades de trabajo 
            var units = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterface(nameof(IUnitWork)) is not null)
                .ToList();
            // Creamos el registro de unidades de trabajo
            var unitWorkRegistry = new UnitWorkRegistry();
            // Recorremos las unidades encontradas
            foreach (var unit in units)
            {
                // Agregamos el servicio de la unidad de trabajo
                services.AddScoped<IUnitWork>(x => (IUnitWork)x.GetService(unit));
                unitWorkRegistry.Register(unit);
                // Omitimos el registro de una unidad de trabajo como una
                // dependencia independiente, se deja a criterio del usuario
                // el registro o la omision de la unidad de trabajo para cada
                // modulo
            }
            // Registramos la instancia del registro de unidad de trabajo
            services.AddSingleton(unitWorkRegistry);
            // Las registramos dentro de nuestro registro de unidades de trabajo
            return services;
        }
    }
}
