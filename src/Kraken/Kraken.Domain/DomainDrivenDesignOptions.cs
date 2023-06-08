using Kraken.Domain.Storage;
using Kraken.Module.Common;

namespace Kraken.Domain;

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
    internal IRepositoryExtension<TModule> RepositoryExtension;

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
    /// Valida que la informacion necesaria este registrada
    /// </summary>
    internal void Validate()
    {
        if(RepositoryExtension is null)
            throw new ArgumentNullException(nameof(RepositoryExtension));
    }
}
