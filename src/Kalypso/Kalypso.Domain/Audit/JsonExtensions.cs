// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class JsonExtensions
{

    /// <summary>
    /// Indica si un elemento es nulo o indefinido
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool IsNullOrUndefined(this JsonElement element)
    {
        if (element.ValueKind is JsonValueKind.Null) return true;
        if (element.ValueKind is JsonValueKind.Undefined) return true;
        return false;
    }

    /// <summary>
    /// Convierte un Json Element en un Json value para las operaciones
    /// de comparacion usando Nodes
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static JsonValue AsValue(this JsonElement element) 
        => JsonValue.Create(element);

    /// <summary>
    /// Agrega al json object una entrada con los valores de la propiedad
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static JsonObject Add(this JsonObject delta, JsonProperty prop, bool IsDelete = false)
    {
        if(prop.Value.ValueKind is JsonValueKind.Array)
        {

        }
        delta.Add(prop.Name, prop.Value.AsValue());
        return delta;
    }

    /// <summary>
    /// Agrega una entrada con los valores prefabricados para marcar una opearcion
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static JsonObject AddOperation(this JsonObject delta, string value)
    {
        delta.Add("__Operation", value);
        return delta;
    }

    /// <summary>
    /// Asegura la devolucion de una lista aun cuando el elemento
    /// sea nulo o indefinido
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static List<JsonElement> ToList(this JsonElement element)
    {
        if(element.IsNullOrUndefined())
            return new List<JsonElement>();
        return element.EnumerateArray().ToList();
    }

    /// <summary>
    /// Obtiene el tipo para el identificador de una lista de elementos
    /// </summary>
    /// <param name="elements"></param>
    /// <returns></returns>
    public static Type? GetIdentifierType(this List<JsonElement> elements)
    {
        var firts = elements.FirstOrDefault();
        if (firts.ValueKind is JsonValueKind.Undefined)
            return null;
        if (!firts.TryGetProperty("Id", out var id))
            return null;

        var type = id.ValueKind switch
        {
            JsonValueKind.Object => typeof(object),
            JsonValueKind.String => typeof(string),
            JsonValueKind.Number => typeof(double),
            JsonValueKind.True => typeof(bool),
            JsonValueKind.False => typeof(bool),
            _ => typeof(object),
        };
        return type;
    }

    /// <summary>
    /// Calcula el hashcode para un elemento
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static int BuildRecordIdentifier(this JsonElement element)
    {
        var hash = 691;
        
        foreach (var prop in element.EnumerateObject())
        {
            if (prop.Value.ValueKind is JsonValueKind.Array)
                continue;
            if (prop.Value.ValueKind is JsonValueKind.Undefined)
                continue;
            if (prop.Value.ValueKind is JsonValueKind.Null)
                continue;
            if (prop.Value.ValueKind is JsonValueKind.Object)
                continue;

            var propHash = typeof(JsonExtensions)
            .GetMethod("GetJsonElementHashCode", BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(prop.Value.GetJsonElementType())
            .Invoke(null, [prop.Value]) as int?;

            hash *= 397 + propHash.Value;
        }
        return hash;
    }

    /// <summary>
    /// Obtiene el tipo del elemento
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Type GetJsonElementType(this JsonElement element)
    {
        var type = element.ValueKind switch
        {
            JsonValueKind.Object => typeof(object),
            JsonValueKind.String => typeof(string),
            JsonValueKind.Number => typeof(double),
            JsonValueKind.True => typeof(bool),
            JsonValueKind.False => typeof(bool),
            _ => typeof(object),
        };
        return type;
    }

    /// <summary>
    /// Calcula el codigo hash de un json element accediendo al valor nativo
    /// de la propiedad para obtener el hash mas cercano al comun
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <returns></returns>
    public static int GetJsonElementHashCode<T>(JsonElement element)
    {
        if(element.AsValue().TryGetValue<T>(out var value))
            return value.GetHashCode();
        return 0;
    }
}
