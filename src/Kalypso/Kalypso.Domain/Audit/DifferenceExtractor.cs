// See https://aka.ms/new-console-template for more information
using Dottex.Kalypso.Domain.Core;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

public sealed class DifferenceExtractor
{
    /// <summary>
    /// Obtiene el delta de modificaciones para las dos intancias
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="oldInstance"></param>
    /// <param name="newInstance"></param>
    /// <returns></returns>
    public string? GetDelta<T>(T oldInstance, T newInstance)
    {
        var oldDocument = GetJsonRepresentation(oldInstance).RootElement;
        var newDocument = GetJsonRepresentation(newInstance).RootElement;
        var dd = JsonObject.Create(oldDocument);
        
        return GetDelta(oldDocument, newDocument);
    }

    /// <summary>
    /// Obtiene el delta basada en dos documentos json
    /// </summary>
    /// <param name="oldDocument"></param>
    /// <param name="newDocument"></param>
    /// <returns></returns>
    public string? GetDelta(JsonElement oldDocument, JsonElement newDocument)
    {
        var delta = CompareElements(oldDocument, newDocument);
        return delta?.ToJsonString();
    }

    /// <summary>
    /// Compara dos elementos json a partir de su naturaleza
    /// </summary>
    /// <param name="old"></param>
    /// <param name="newer"></param>
    /// <returns></returns>
    private JsonNode? CompareElements(JsonElement old, JsonElement newer)
    {
        // Si ambos son nulos no hay diferencia
        if (old.IsNullOrUndefined() && newer.IsNullOrUndefined())
            return default;
        // Obtenemos la clase de elemento ya sea del antiguo o del actual
        var kind = old.IsNullOrUndefined() ? newer.ValueKind : old.ValueKind;
        // Evaluacion del tipo de objeto
        JsonNode? delta = kind switch
        {
            JsonValueKind.Object => GetObjectDelta(old, newer),
            JsonValueKind.Array => GetArrayDelta(old, newer),
            _ => GetValueDelta(old, newer)
        };
        // Devolucion del delta
        return delta;
    }

    /// <summary>
    /// Obtiene el delta a partir de la representacion anterior de los elementos y 
    /// la representacion actual del mismo
    /// </summary>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private JsonObject? GetObjectDelta(JsonElement old, JsonElement current)
    {
        // Contenedor del delta
        var deltaObject = ProcessObject(old, current);
        var operation = (old.IsNullOrUndefined(), current.IsNullOrUndefined()) switch
        {
            (true, false) => "Add",
            (false, true) => "Delete",
            _ => "Update"
        };
        deltaObject.AddOperation(operation);
        return deltaObject.Count > 2 ? deltaObject : null;
    }

    /// <summary>
    /// Recibe dos elementos y calcula la diferencia de ambos
    /// </summary>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private JsonObject ProcessObject(JsonElement old, JsonElement current)
    {
        // En este punto no deberian de venir ambos nulos
        // Obtenemos las propiedades del elemento que no este nulo.
        var props = old.IsNullOrUndefined() 
            ? current.EnumerateObject().ToList() 
            : old.EnumerateObject().ToList();

        var deltaObject = new JsonObject();

        foreach (var prop in props)
        {
            // Si es un id, entonces, lo agregamos directo al objeto, por que debe ir siempre
            if (prop.Name == "Id")
            {
                deltaObject.Add(prop.Name, prop.Value.AsValue());
                continue;
            }

            // Obtenemos el old value y el current segun si son validos para obtenerlos
            var oldValue = !old.IsNullOrUndefined() ? old.GetProperty(prop.Name) : default ;
            var currentValue = !current.IsNullOrUndefined() ? current.GetProperty(prop.Name) : default ;
            // Se comparan
            var deltaValue = CompareElements(oldValue, currentValue);
            // Si el delta es nulo, entonces son iguales
            if (deltaValue is null)
                continue;
            // Si no son iguales, entonces se agrega el delta al objeto
            deltaObject.Add(prop.Name, deltaValue);
        }

        return deltaObject;;
    }

    /// <summary>
    /// Obtiene el valor de diferencia entre los dos valores, si existe
    /// diferencia, en caso contrario devuelve nulo
    /// </summary>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private JsonNode? GetValueDelta(JsonElement old, JsonElement current)
    {
        var oldValue = old.AsValue();
        var currentValue = current.AsValue();
        // Hasta aqui no deberia llegar ambos nulos, asi que si uno de los dos es nulo, devolvemos el otro
        if (old.IsNullOrUndefined()) return currentValue;
        if(current.IsNullOrUndefined()) return oldValue;
        // Si ambos son validos, comparamos la igualdad del valor
        if (JsonNode.DeepEquals(oldValue, currentValue))
            return null;
        // Si no son iguales, devolvemos el valor actual, que es el que se modifico
        return current.AsValue();
    }

    /// <summary>
    /// Calcula los deltas para las listas pasadas por parametro a traves
    /// de estrategia de entidades o registros
    /// </summary>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private JsonNode? GetArrayDelta(JsonElement old, JsonElement current)
    {
        // No pueden venir ambos element nulos
        // Las pasamos a lista asegurando que los nulos se vuelvan listas vacias
        var oldList = old.ToList();
        var currentList = current.ToList();

        // Si ambas estan vacias, lo que no deberia, entonces no hay delta
        if (!oldList.Any() && !currentList.Any())
            return null;
        // Se realiza la estrategia para tratar los elementos dependiendo si usan id o son por valor
        var delta = TryEntityComparison(oldList, currentList, out var output) 
            ? output : 
            RecordComparison(oldList, currentList);
        // Se devuelve el delta
        return delta;
    }

    /// <summary>
    /// Hace la evaluacion de dos listas de elementos para obtener las diferencias de sus
    /// items basados en el id
    /// </summary>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    private bool TryEntityComparison(List<JsonElement> old, List<JsonElement> current, out JsonArray? delta)
    {
        delta = new JsonArray();
        // Obtiene el tipo del id si es que existe
        var IdentifierType = old.GetIdentifierType() ?? current.GetIdentifierType();
        // Si no existe, no podemos procesar como entidad
        if (IdentifierType == null)
            return false;
        // Ejecutamos la comparacion con el tipo obtenido del id
        delta = this.GetType()
            .GetMethod("EntityComparison", BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(IdentifierType)
            .Invoke(this, [old, current]) as JsonArray;
        // Devolvemos si el delta existe o no
        return delta is not null;
    }

    /// <summary>
    /// Ordena los registros a partir de su id y los evalua uno a uno en el comparador general de objetos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private JsonArray? EntityComparison<T>(List<JsonElement> old, List<JsonElement> current)
    {
        // Convertimos las listas de elementos en diccionarios
        var oldSorted = old.ToDictionary(x => x.GetProperty("Id").AsValue().GetValue<T>());
        var currentSorted = current.ToDictionary(x => x.GetProperty("Id").AsValue().GetValue<T>());
        // Se juntan las keys y se eliminan duplicados
        var allKeys = oldSorted.Keys.Union(currentSorted.Keys);
        var delta = new JsonArray();
        foreach (var id in allKeys)
        {
            // Obtenemos el elemento para la key en turno para cada lista
            oldSorted.TryGetValue(id, out var oldElement);
            currentSorted.TryGetValue(id, out var currentElement);
            // Se comparan los elementos
            var elementDelta = CompareElements(oldElement, currentElement);
            // Si son iguales continuamos
            if (elementDelta is null)
                continue;
            delta.Add(elementDelta);
        }

        return delta.Any() ? delta : default;
    }

    /// <summary>
    /// Realiza la estrategia para comparacion de elementos basada en valores. 
    /// </summary>
    /// <param name="old"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private JsonNode? RecordComparison(List<JsonElement> old, List<JsonElement> current)
    {
        // Primero debemos de crear los ids basados en las propiedades de los elementos
        var oldSorted = old.ToDictionary(x => x.BuildRecordIdentifier());
        var currentSorted = current.ToDictionary(x => x.BuildRecordIdentifier());
        // Juntamos los ids eliminando duplicados
        var allKeys = oldSorted.Keys.Union(currentSorted.Keys).ToList();
        var delta = new JsonArray();
        foreach (var id in allKeys)
        {
            oldSorted.TryGetValue(id, out var oldElement);
            currentSorted.TryGetValue(id, out var currentElement);
            var elementDelta = CompareElements(oldElement, currentElement);
            if (elementDelta is null)
                continue;
            delta.Add(elementDelta);
        }

        return delta.Any() ? delta : default;
    }

    /// <summary>
    /// Convierte una instancia de un objeto en un documento json
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public JsonDocument GetJsonDocument(object element) 
        => JsonSerializer.SerializeToDocument(element, options: new JsonSerializerOptions
        {
            Encoder = null
        });

    public JsonDocument GetJsonRepresentation(object element)
    {
        string[] aggregateKeys = ["AggregateId", "State", "AggregateRootType", "DomainEvents"];
        var jsonElement = JsonSerializer.SerializeToNode(element);
        var cleanElement = aggregateKeys.Aggregate(jsonElement.AsObject(), (node, next) =>
        {
            node.Remove(next);
            return node;
        });
        return cleanElement.Deserialize<JsonDocument>();
    }
}
