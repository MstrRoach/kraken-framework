using Dottex.Kalypso.Domain.Audit;
using Dottex.Kalypso.Domain.Core;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module;
using Dottex.Kalypso.Module.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain;

public static class DomainExtensions
{
    /// <summary>
    /// Registra y habilita los mecanismos para utilizar domain driven design, incluyendo el uso
    /// del interceptor para obtener los eventos de los agregados y distribuirlos
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomainDrivenDesign<TModule>(
        this IServiceCollection services,
        Action<DomainDrivenDesignOptions<TModule>>? builder)
        where TModule : IModule
    {
        // Validamos que las opciones tengan lo obligatorio
        var options = new DomainDrivenDesignOptions<TModule>();
        // Las precargamos
        builder.Invoke(options);
        // Registramos las configuraciones
        services.Configure(builder);
        // Lista de ensamblados
        List<Assembly> assemblies = [
            typeof(TModule).Assembly, 
            ..options.Nested.Select(x => x.Assembly).ToList()];
        var moduleAssembly = typeof(TModule).Assembly;
        // Extraemos todos los agregados
        var callerAggregates = assemblies.SelectMany(x => x.GetTypes())
            .Where(x => !x.IsOpenGeneric())
            .Where(x => !x.IsAbstract)
            .Where(x => x.GetInterface(nameof(IAggregate)) is not null)
            .ToList();
        // Registramos los repositorios
        options.RepositoryExtension?.AddServices(services, callerAggregates);
        // Registramos el almacen de auditoria
        services.AddSingleton(typeof(Flattener<>));
        services.AddSingleton<ChangeExtractor>();
        services.AddSingleton<DifferenceExtractor>();
        // Registramos el decorador
        services.TryDecorate(typeof(IRepository<>), 
            typeof(EventExtractorRepositoryDecorator<>));
        services.TryDecorate(typeof(IRepository<>), 
            typeof(AuditorRepositoryExtractor<>));
        // Salimos
        return services;
    }

}
