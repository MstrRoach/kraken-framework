using Kraken.Domain;
using Kraken.Domain.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Domain.Repository.InMemory;

public static class DomainDrivenDesignOptionsExtensions
{
    public static DomainDrivenDesignOptions UseInMemoryRepository(this DomainDrivenDesignOptions extension)
    {
        extension.RegisterRepository(new RepositoryExtensions());
        return extension;
    }
}

/// <summary>
/// Configuracion para el registro de los repositorios
/// </summary>
public class RepositoryExtensions : IRepositoryExtension
{
    public void AddServices(IServiceCollection services, List<Type> Aggregates)
    {
        var repositoryOpenType = typeof(IRepository<>);
        var implementationRepositoryOpenType = typeof(DefaultRepository<,,>);
        // Recorremos cada tipo de agregado
        foreach (var aggregate in Aggregates)
        {
            // Obtenemos el tipo, el id
            var baseType = aggregate.BaseType;
            var repositoryType = repositoryOpenType.MakeGenericType(aggregate);
            var implementationRepositoryType = implementationRepositoryOpenType.MakeGenericType(
                aggregate, 
                baseType.GenericTypeArguments[0], 
                baseType.GenericTypeArguments[1]);
            // Registramos el repositorio
            services.AddScoped(repositoryType, implementationRepositoryType);
        }
    }
}