using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Common;

/// <summary>
/// Interface para configurar y usar los modulos
/// desde el server de kraken
/// </summary>
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
}
