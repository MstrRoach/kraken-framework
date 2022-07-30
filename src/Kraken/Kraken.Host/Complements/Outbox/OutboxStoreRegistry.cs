using Kraken.Core;
using Kraken.Core.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox;

/// <summary>
/// Define un registro de las unidades de
/// trabajo asociadas al modulo del que provienen
/// </summary>
internal class OutboxStoreRegistry
{
    /// <summary>
    /// Contiene la relacion entre los modulos y las unidades de
    /// trabajo que corresponden a cada modulo
    /// </summary>
    private readonly Dictionary<string, Type> _moduleOutboxs = new();

    /// <summary>
    /// Registra un tipo de unidad de trabjo dentro del diccionario para conservr
    /// la relacion entre modulo y unidad de trabajo
    /// </summary>
    /// <param name="outbox"></param>
    public void Register(Type outbox) => _moduleOutboxs[GetKey(outbox)] = outbox;

    /// <summary>
    /// Devuelve el tipo de la unidad de trabajo segun el tipo que se pasa por
    /// parametro para construir la unidad de trabajo asociada el modulo del tipo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Type Resolve<T>() => _moduleOutboxs.TryGetValue(GetKey<T>(), out var type) ? type : null;

    /// <summary>
    /// Devuelve el tipo de bandeja de salida asociada al modulo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public Type Resolve<T>(T message) => _moduleOutboxs.TryGetValue(GetKey(message.GetType()), out var type) ? type : null;

    /// <summary>
    /// Devuelve el tipo de la bandeja de salida asociada al tipo especificado para el modulo
    /// asignado
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    public Type Resolve(Type eventType) => _moduleOutboxs.TryGetValue(GetKey(eventType), out var type) ? type : null;

    /// <summary>
    /// Obtiene el nombre del modulo a partir del tipo especificado
    /// </summary>
    /// <param name="outbox"></param>
    /// <returns></returns>
    private static string GetKey(Type outbox) => $"{outbox.GetModuleName()}";

    /// <summary>
    /// Devuelve una llave a partir de un tipo generico pasado por parametro 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string GetKey<T>() => $"{typeof(T).GetModuleName()}";
}