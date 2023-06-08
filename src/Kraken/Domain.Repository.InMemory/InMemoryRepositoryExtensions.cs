using Domain.Repository.InMemory.MemoryStorable;
using Kraken.Domain.Storage;
using Kraken.Module.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Repository.InMemory;

/// <summary>
/// Configuracion para el registro de los repositorios
/// </summary>
public class InMemoryRepositoryExtensions<TModule> : IRepositoryExtension<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Opciones configuraddass para el repositorio del modulo
    /// </summary>
    private InMemoryRepositoryOptions<TModule> options;

    /// <summary>
    /// Constructor con los valores de configuracion
    /// </summary>
    /// <param name="options"></param>
    public InMemoryRepositoryExtensions(InMemoryRepositoryOptions<TModule> options)
    {
        this.options = options;
    }

    /// <summary>
    /// Agrega los servicios de los repositorios en memoria a los servicios
    /// de la aplicacion
    /// </summary>
    /// <param name="services"></param>
    /// <param name="Aggregates"></param>
    public void AddServices(IServiceCollection services, List<Type> Aggregates)
    {
        // Agregamos las opciones de configuracion
        services.AddSingleton(options);
        // Creammos los tipos abiertos
        var repositoryOpenType = typeof(IRepository<>);
        var implementationRepositoryOpenType = typeof(DefaultRepository<,,>);
        var memoryStorableType = typeof(IMemoryStorable<TModule>);
        var implementationMemoryStorableOpenType = typeof(DefaultMemoryStorable<,>);
        // Recorremos cada tipo de agregado
        foreach (var aggregate in Aggregates)
        {
            // Obtenemos el tipo, el id
            var baseType = aggregate.BaseType;
            var repositoryType = repositoryOpenType.MakeGenericType(aggregate);
            var implementationRepositoryType = implementationRepositoryOpenType.MakeGenericType(
                typeof(TModule),
                aggregate, 
                baseType.GenericTypeArguments[0]);
            // Registramos el repositorio
            services.AddScoped(repositoryType, implementationRepositoryType);
            // Creacion de la implementacion para el storable
            var implementationMemoryStorableType = implementationMemoryStorableOpenType.MakeGenericType(
                typeof(TModule),
                aggregate);
            // Registramos el accesor al storage de memoria como servicio especifico
            services.AddSingleton(implementationMemoryStorableType);
            // Registramos el acceso independiente
            services.AddSingleton(memoryStorableType, 
                x => x.GetRequiredService(implementationMemoryStorableType));
        }
        // Agregamos el arrancador de la base de datos
        services.AddSingleton<DatabaseBootstrap<TModule>>();
        // Inicializacion de la base de datos en sqllite
        services.BuildServiceProvider()
            .GetService<DatabaseBootstrap<TModule>>()!
            .Initialize()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
