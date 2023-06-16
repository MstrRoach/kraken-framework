using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dottex.Kalypso.Domain.Audit;

public class ChangeExtractor
{
    private string ListChange = "$Array__$Action";

    /// <summary>
    /// Obtiene los cambios entre dos entidades
    /// </summary>
    /// <param name="oldEntity"></param>
    /// <param name="newEntity"></param>
    /// <returns></returns>
    public Dictionary<string, object> GetChanges(JsonObject oldEntity, JsonObject newEntity)
    {
        var keys = oldEntity.Select(x => x.Key).ToList();
        keys = newEntity.Where(x => !keys.Contains(x.Key))
            .Select(x => x.Key)
            .Concat(keys).ToList();
        var changes = new Dictionary<string, object>();
        foreach (var key in keys)
        {
            // Obtenemos el valor de la clave en la entidad vieja y la actual
            var oldValue = oldEntity[key];
            var newValue = newEntity[key];
            // Si ambos son nulos, son iguales
            if (oldValue is null && newValue is null)
                continue;
            
            // Si los nodos son de lista
            if(oldValue is JsonArray && newValue is JsonArray)
            {
                var valuesAdded = GetListChanges(
                    oldValue.AsArray(), 
                    newValue.AsArray()
                    );

                var valuesRemoved = GetListChanges(
                    newValue.AsArray(),
                    oldValue.AsArray()
                    );

                if (valuesAdded.Count > 0)
                {
                    var addedKey = ListChange
                        .Replace("$Array", key)
                        .Replace("$Action","Added");
                    changes.Add(addedKey, valuesAdded);
                }

                if (valuesRemoved.Count > 0)
                {
                    var removedKey = ListChange
                        .Replace("$Array", key)
                        .Replace("$Action", "Removed");
                    changes.Add(removedKey, valuesRemoved);
                } 
                continue;
            }

            // Obtenemos el tipo
            var valueType = GetValueType(oldValue ?? newValue);
            // Obtenemos el valor de comparacion
            var areEquals = valueType switch
            {
                "guid" => IsGuidEquals(oldValue, newValue),
                "string" => IsStringEquals(oldValue, newValue),
                "datetime" => IsDatetimeEquals(oldValue, newValue),
                "object" => Equals(oldValue, newValue),
                "null" => true,
                _ => !(oldValue is null ^ newValue is null)
            };
            // si son iguales continuamos
            if (areEquals)
                continue;
            // Comparacion de valores
            if (oldValue == newValue)
                continue;
            // Agregamos el cambio
            changes.Add(key, newValue);


        }
        return changes;
    }

    /// <summary>
    /// Obtiene una representacion en diccionario de los valores de
    /// la entidad
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Dictionary<string,object> GetSnapshot(JsonObject entity)
    {
        // Obtenemos las llaves
        var keys = entity.Select(x => x.Key).ToList();
        // Diccionario de los cambios
        var snapshot = new Dictionary<string, object>();
        // Recorremos las llaves
        foreach (var key in keys)
        {
            // Obtenemos el valor
            var value = entity[key];
            // Agregamos el valor
            snapshot.Add(key,value);
        }
        return snapshot;
    }

    /// <summary>
    /// Obtiene el valor del tipo para ejecutar la comparacion
    /// correcta
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private string GetValueType(JsonNode node)
    {
        var value = node.AsValue();
        if (node is null)
            return "null";
        if (value.TryGetValue<DateTime>(out _))
            return "datetime";
        if (value.TryGetValue<Guid>(out _))
            return "guid";
        if (value.TryGetValue<string>(out _))
            return "string";
        return "object";
    }

    /// <summary>
    /// Compara dos nodos a partir del comparador
    /// para Guids
    /// </summary>
    /// <param name="oldNode"></param>
    /// <param name="newNode"></param>
    /// <returns></returns>
    private bool IsGuidEquals(JsonNode oldNode, JsonNode newNode)
    {
        // Si alguno de los dos es nulo, entonces no son iguales
        if (oldNode is null || newNode is null)
            return false;
        if (!oldNode.AsValue()
            .TryGetValue<Guid?>(out var oldValue)
            & !newNode.AsValue().TryGetValue<Guid?>(out var newValue))
            return false;
        return Guid.Equals(oldValue, newValue);
    }

    /// <summary>
    /// Compara dos nodos a partir de la comparacion de cadenas
    /// </summary>
    /// <param name="oldNode"></param>
    /// <param name="newNode"></param>
    /// <returns></returns>
    private bool IsStringEquals(JsonNode oldNode, JsonNode newNode)
    {
        // Si alguno de los dos es nulo, entonces no son iguales
        if (oldNode is null || newNode is null)
            return false;
        if (!oldNode.AsValue()
            .TryGetValue<string>(out var oldValue)
            & !newNode.AsValue().TryGetValue<string>(out var newValue))
            return false;
        return string.Equals(oldValue, newValue);
    }

    /// <summary>
    /// Compara dos nodos a partir de la comparacion de fechas
    /// </summary>
    /// <param name="oldNode"></param>
    /// <param name="newNode"></param>
    /// <returns></returns>
    private bool IsDatetimeEquals(JsonNode oldNode, JsonNode newNode)
    {
        // Si alguno de los dos es nulo, entonces no son iguales
        if (oldNode is null || newNode is null)
            return false;
        if (!oldNode.AsValue()
            .TryGetValue<DateTime?>(out var oldValue)
            & !newNode.AsValue().TryGetValue<DateTime?>(out var newValue))
            return false;
        // Si alguno de los dos es nulo entonces no son iguales
        return DateTime.Equals(oldValue, newValue);
    }

    /// <summary>
    /// Obtiene el listado de elementos de la lista nueva que no se
    /// encuentran en la lista antigua
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseList"></param>
    /// <param name="newList"></param>
    /// <returns></returns>
    public JsonArray GetListChanges(JsonArray baseList, JsonArray newList)
    {
        // Si la lista antigua esta vacia, entonces devolvemos la lista nueva
        if (baseList.Count == 0)
            return newList;
        var changes = new JsonArray();
        foreach (var item in newList)
        {
            // Si en la lista antigua existe el elemento actual entonces no cambio
            if (baseList.Any(baseItem => AreEquals(baseItem.AsObject(), item.AsObject())))
                continue;
            // Agregamos el elemento que cambio
            changes.Add(item);
        }
        return changes;
    }

    /// <summary>
    /// Indica si dos entidades son iguales o no. Solo
    /// son iguales si no existen cambios en los valores de sus campos
    /// </summary>
    /// <param name="oldEntity"></param>
    /// <param name="newEntity"></param>
    /// <returns></returns>
    private bool AreEquals(JsonObject oldEntity, JsonObject newEntity)
    {
        var changes = GetChanges(oldEntity, newEntity) as Dictionary<string, object>;
        return changes.Count == 0;
    }
}