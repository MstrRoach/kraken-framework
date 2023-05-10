using Kraken.Standard.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Operation.Transaction;

/// <summary>
/// Fabrica para construir las unidades de trabajo para cada commando
/// </summary>
internal class DefaultUnitWorkFactory : IUnitWorkFactory
{
    /// <summary>
    /// Definicion de las unidades de trabajo asociadas
    /// a cada modulo
    /// </summary>
    private readonly UnitWorkRegistry _unitWorkRegistry;

    /// <summary>
    /// Proveedor de los servicios para obtener la unidad de 
    /// trabajo en el alcance
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public DefaultUnitWorkFactory(UnitWorkRegistry unitWorkRegistry,
            IServiceProvider serviceProvider)
    {
        _unitWorkRegistry = unitWorkRegistry;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Obtiene la unidad de trabajo registrada para el modulo al que
    /// pertenece el comando, en caso de no encontrar uno, construye una
    /// unidad de trabajo por defecto para las operaciones
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IUnitWork CreateUnitWork<T>()
    {
        // Obtenemos el modulo
        throw new NotImplementedException();
    }
}
