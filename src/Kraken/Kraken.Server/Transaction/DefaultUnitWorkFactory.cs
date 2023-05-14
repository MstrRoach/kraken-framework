using Kraken.Module.Server;
using Kraken.Module.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Transaction;

/// <summary>
/// Fabrica para construir las unidades de trabajo para cada commando
/// </summary>
internal class DefaultUnitWorkFactory : IUnitWorkFactory
{
    /// <summary>
    /// Definicion de las unidades de trabajo asociadas
    /// a cada modulo
    /// </summary>
    private readonly ModuleRegistry _moduleRegistry;

    /// <summary>
    /// Proveedor de los servicios para obtener la unidad de 
    /// trabajo en el alcance
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public DefaultUnitWorkFactory(ModuleRegistry moduleRegistry,
        IServiceProvider serviceProvider)
    {
        _moduleRegistry = moduleRegistry;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Obtiene la unidad de trabajo registrada para el modulo al que
    /// pertenece el comando, en caso de no encontrar uno, construye una
    /// unidad de trabajo por defecto para las operaciones
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IUnitWork CreateUnitWork<T>()
    {
        // Obtenemos el modulo
        var module = _moduleRegistry.Resolve<T>();
        // Creamos el tipo que debe recuperarse de los servicios
        var specificUnitWorkType = typeof(IUnitWork<>).MakeGenericType(module);
        // Obtenemos el servicio
        var specificUnitWork = _serviceProvider.GetService(specificUnitWorkType);
        // Si no es nulo lo regresamos
        if (specificUnitWork is not null)
            return specificUnitWork as IUnitWork;
        // Creamos el tipo cerrado para la unidad de trabajo por defecto
        var defaultUnitWorkType = typeof(DefaultUnitWork<>).MakeGenericType(module);
        // Obtenemos el servicio
        var defaultUnitWork = _serviceProvider.GetService(defaultUnitWorkType);
        // Lo regresamos como unidad de trabajo
        return defaultUnitWork as IUnitWork;
    }
}
