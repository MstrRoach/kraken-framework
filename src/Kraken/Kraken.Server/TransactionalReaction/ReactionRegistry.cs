using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalReaction;

/// <summary>
/// Registro de todas las reacciones para cada evento
/// de dominio lanzado y cada uno de los eventos de modulo
/// </summary>
internal class ReactionRegistry
{
    /// <summary>
    /// Contiene la relacion entre los eventos y las reacciones que desencadenan en
    /// cada modulo disponible
    /// </summary>
    private readonly Dictionary<string, List<Type>> _eventHandlers = new();

    /// <summary>
    /// Registra una lista de reacciones asociadas a un tipo de evento especifico
    /// </summary>
    /// <param name="event"></param>
    /// <param name="handlers"></param>
    public void Register(Type @event, List<Type> handlers) => _eventHandlers[GetKey(@event)] = handlers;

    /// <summary>
    /// Obtiene la lista de reacciones que pertenecen a un evento especificado por parametro
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public List<Type> Resolve(Type @event)
        => _eventHandlers.TryGetValue(GetKey(@event), out var handlers) ? handlers : new();

    /// <summary>
    /// Obtiene la lista completa de las reacciones como una lista de solo lectura
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<Type> GetAllHandlers()
        => _eventHandlers.Values.SelectMany(x => x).ToList().AsReadOnly();

    /// <summary>
    /// Calcula la llave de un evento, a partir del tipo pasado por parametro
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    private static string GetKey(Type @event) => @event.AssemblyQualifiedName;
}
