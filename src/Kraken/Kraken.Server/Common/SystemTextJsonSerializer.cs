using Kraken.Module.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kraken.Server.Common;

/// <summary>
/// Servicio de serializacion para toda la aplicacion
/// </summary>
public class SystemTextJsonSerializer : IJsonSerializer
{
    /// <summary>
    /// Configuraciones para el serializador
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, Options);

    public object Deserialize(string value, Type type) => JsonSerializer.Deserialize(value, type, Options);
}
