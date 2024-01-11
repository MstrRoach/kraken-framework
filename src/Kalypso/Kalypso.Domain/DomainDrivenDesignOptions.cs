using Dottex.Kalypso.Domain.Audit;
using Dottex.Kalypso.Domain.Storage;
using Dottex.Kalypso.Module.Common;

namespace Dottex.Kalypso.Domain;

/// <summary>
/// Opciones paraa la configuracion del modulo de domain driven
/// </summary>
public class DomainDrivenDesignOptions<TModule> where TModule : IModule
{
    /// <summary>
    /// Nombre del modulo que configura DDD
    /// </summary>
    public string Module => nameof(TModule);

    /// <summary>
    /// Configuracion para los repositorios del modulo que usa DDD
    /// </summary>
    internal IRepositoryExtension<TModule>? RepositoryExtension;

    /// <summary>
    /// Lista de modulos internos donde existen agregados
    /// </summary>
    internal List<Type> Nested = new();

    /// <summary>
    /// Registra la extension para el repositorio
    /// </summary>
    /// <param name="repositoryExtension"></param>
    public void RegisterRepository(IRepositoryExtension<TModule> repositoryExtension)
    {
        if (repositoryExtension == null)
            throw new ArgumentNullException(nameof(repositoryExtension));
        RepositoryExtension = repositoryExtension;
    }

    /// <summary>
    /// Agrega el modulo a los tipos desde los cuales se pueden extraer
    /// agregados
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    public void IncludeAggregatesFrom<TModule>() where TModule : IModule
    {
        this.Nested.Add(typeof(TModule));
    }
}
