using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kraken.Core.Modules;

public interface IModule
{
    /// <summary>
    /// Indica el nombre del modulo
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registra los servicios necesarios para el modulo
    /// dentro del contenedor de dependencias
    /// </summary>
    /// <param name="services"></param>
    void Register(IServiceCollection services);

    /// <summary>
    /// Agrega los servicios necesarios en el flujo del pipeline
    /// </summary>
    /// <param name="app"></param>
    void Use(IApplicationBuilder app);
}
