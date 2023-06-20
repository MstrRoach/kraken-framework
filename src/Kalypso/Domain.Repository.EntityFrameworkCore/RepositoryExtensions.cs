using Dottex.Kalypso.Domain;
using Dottex.Kalypso.Module.Common;
using System.Security.AccessControl;

namespace Dottex.Domain.Repository.EntityFrameworkCore;

public static class DomainDrivenDesignOptionsExtensions
{
    /// <summary>
    /// Agrega los repositorios usando entity framework y configura todo
    /// lo necesario para habilitar el funcionamiento
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <param name="extensions"></param>
    /// <param name="entityFrameworkOptions"></param>
    /// <returns></returns>
    public static DomainDrivenDesignOptions<TModule> UseEntityFrameworkRepository<TModule>(this DomainDrivenDesignOptions<TModule> extensions, 
        Action<EntityFrameworkRepositoryOptions<TModule>> entityFrameworkOptions = null)
        where TModule : IModule
    {
        var options = new EntityFrameworkRepositoryOptions<TModule>();
        entityFrameworkOptions?.Invoke(options);
        extensions.RegisterRepository(new EntityFrameworkRepositoryExtensions<TModule>(options));
        return extensions;
    }
}
