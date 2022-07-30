using Kraken.Core;
using Kraken.Core.Domain;
using Kraken.Core.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Storage
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Encargado de agregar el soporte para los repositorios, el decorador encargado de despachar los eventos
        /// automaticamente y de agregar las abtracciones con sus implementaciones
        /// </summary>
        /// <param name="services"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositorySupport(this IServiceCollection services, List<Assembly> modules)
        {
            // Buscamos todos los agregados
            var aggregates = modules
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsOpenGeneric())
                .Where(x => x.GetInterface(nameof(IAggregate)) is not null)
                .ToList();
            //.ToDictionary(key => key,element => default(Type));
            // Creamos el tipo abierto para buscar los repositorios
            var repositoryOpenType = typeof(IRepository<>);
            // Buscamos los repositorios que implementen la interface en los ensamblados
            var repositories = modules
                .SelectMany(x => x.GetTypes())
                //.Where(x => x.Name == "AccountRepository")
                .Where(x => !x.IsOpenGeneric())
                .Where(x => x.FindInterfacesThatClose(repositoryOpenType).Any())
                .ToDictionary(key => key.GetInterface(repositoryOpenType.Name).GetGenericArguments()[0].Name, element => element);
            // Recorremos la lista de agregados
            foreach (var aggregate in aggregates)
            {
                // Si no aparece el repositorio, es un error
                if (!repositories.TryGetValue(aggregate.Name, out var repository))
                {
                    throw new InvalidOperationException($"The implemented repository for the aggregate {aggregate.Name} was not found");
                }
                // Creamos el tipo para la interface
                var abstraction = repositoryOpenType.MakeGenericType(aggregate);
                // Lo registramos con alcance transitorio
                services.AddTransient(abstraction, repository);
            }
            // Indicamos que debe de decorar los repositorios con la clase para extraer eventos
            services.TryDecorate(typeof(IRepository<>), typeof(EventExtractorRepositoryDecorator<>));
            // Salimos
            return services;
        }

    }
}
