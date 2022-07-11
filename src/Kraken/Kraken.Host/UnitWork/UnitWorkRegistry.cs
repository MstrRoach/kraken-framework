using Kraken.Core.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.UnitWork;

/// <summary>
/// Define un registro de las unidades de
/// trabajo asociadas al modulo del que provienen
/// </summary>
internal class UnitWorkRegistry
{
    /// <summary>
    /// Contiene la relacion entre los modulos y las unidades de
    /// trabajo que corresponden a cada modulo
    /// </summary>
    private readonly Dictionary<string, Type> _moduleUnitWorks = new();

    /// <summary>
    /// Registra un tipo de unidad de trabjo dentro del diccionario para conservr
    /// la relacion entre modulo y unidad de trabajo
    /// </summary>
    /// <param name="unitWork"></param>
    public void Register(Type unitWork) => _moduleUnitWorks[GetKey(unitWork)] = unitWork;

    /// <summary>
    /// Devuelve el tipo de la unidad de trabajo segun el tipo que se pasa por
    /// parametro para construir la unidad de trabajo asociada el modulo del tipo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Type Resolve<T>() => _moduleUnitWorks.TryGetValue(GetKey<T>(), out var type) ? type : null;

    /// <summary>
    /// Obtiene el nombre del modulo a partir del tipo especificado
    /// </summary>
    /// <param name="unitWork"></param>
    /// <returns></returns>
    private static string GetKey(Type unitWork) => $"{unitWork.GetModuleName()}";

    /// <summary>
    /// Devuelve una llave a partir de un tipo generico pasado por parametro 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string GetKey<T>() => $"{typeof(T).GetModuleName()}";
}
