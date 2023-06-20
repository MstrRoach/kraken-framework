using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.Transaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dottex.Domain.Repository.EntityFrameworkCore;

public class EntityFrameworkRepositoryExtensions<TModule> : IRepositoryExtension<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Opciones de configuracion para el repositorio del modulo
    /// </summary>
    private EntityFrameworkRepositoryOptions<TModule> options;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options"></param>
    public EntityFrameworkRepositoryExtensions(EntityFrameworkRepositoryOptions<TModule> options)
    {
        this.options = options;
    }

    /// <summary>
    /// Registra los servicios necesarios para la operacion de los repositorios por
    /// entity framework
    /// </summary>
    /// <param name="services"></param>
    /// <param name="Aggregates"></param>
    public void AddServices(IServiceCollection services, List<Type> Aggregates)
    {
        services.AddSingleton(Options.Create(this));
        // Agregamos las opciones de configuracion
        services.AddSingleton(options);
        // Creamos los tipos abiertos
        var repositoryOpenType = typeof(IRepository<>);
        var implementationRepositoryOpenType = typeof(DefaultRepository<,,>);
        // Recorremos cada tipo de aggregado
        foreach (var aggregate in Aggregates)
        {
            var repositoryType = repositoryOpenType.MakeGenericType(aggregate);
            var implementationRepositoryType = implementationRepositoryOpenType.MakeGenericType(
                typeof(TModule),
                options.ModuleDbContext,
                aggregate);
            // Registramos el repositorio
            services.AddScoped(repositoryType, implementationRepositoryType);
        }
        // Creamos el tipo abierto
        var unitWorkOpenType = typeof(IUnitWork<>);
        var implementationUnitWorkOpenType = typeof(DefaultUnitWork<,>);
        // Creamos los tipos cerrados
        var unitWorkType = unitWorkOpenType.MakeGenericType(typeof(TModule));
        var implementationUnitWork = implementationUnitWorkOpenType.MakeGenericType(typeof(TModule), options.ModuleDbContext);
        // Registramos la unidad de trabajo
        services.AddScoped(unitWorkType, implementationUnitWork);
    }
}
