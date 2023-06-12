using Microsoft.Extensions.DependencyInjection;

namespace Dottex.Kalypso.Domain.Storage;

/// <summary>
/// Interface para la definicion de la configuracion para los repositorioss
/// </summary>
public interface IRepositoryExtension<TModule>
{
    /// <summary>
    /// Agrega los servicios para la configuracion del repositorio en turno
    /// </summary>
    /// <param name="services"></param>
    void AddServices(IServiceCollection services, List<Type> Aggregates);
}