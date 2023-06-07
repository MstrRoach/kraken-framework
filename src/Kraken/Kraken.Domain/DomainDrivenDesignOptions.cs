using Kraken.Domain.Storage;

namespace Kraken.Domain;

/// <summary>
/// Opciones paraa la configuracion del modulo de domain driven
/// </summary>
public class DomainDrivenDesignOptions
{
    /// <summary>
    /// Configuracion para los repositorios del modulo que usa DDD
    /// </summary>
    internal IRepositoryExtension RepositoryExtension;

    /// <summary>
    /// Registra la extension para el repositorio
    /// </summary>
    /// <param name="repositoryExtension"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RegisterRepository(IRepositoryExtension repositoryExtension)
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
