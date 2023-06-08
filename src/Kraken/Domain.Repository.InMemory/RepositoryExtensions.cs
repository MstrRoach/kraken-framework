using Kraken.Domain;
using Kraken.Domain.Core;
using Kraken.Module.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Domain.Repository.InMemory;

public static class DomainDrivenDesignOptionsExtensions
{
    /// <summary>
    /// Agrega los repositorios en memoria y configura todo lo necesario
    /// para el almacenamiento de estos elementos utilizando sqlite
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <param name="extension"></param>
    /// <param name="inMemoryOptions"></param>
    /// <returns></returns>
    public static DomainDrivenDesignOptions<TModule> UseInMemoryRepository<TModule>(
        this DomainDrivenDesignOptions<TModule> extension, Action<InMemoryRepositoryOptions<TModule>> inMemoryOptions = null)
        where TModule : IModule
    {
        var options = new InMemoryRepositoryOptions<TModule>();
        inMemoryOptions?.Invoke(options);
        extension.RegisterRepository(new InMemoryRepositoryExtensions<TModule>(options));
        return extension;
    }
}