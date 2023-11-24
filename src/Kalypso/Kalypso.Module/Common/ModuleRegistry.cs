using Dottex.Kalypso.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Common;

public sealed class ModuleRegistry
{
    /// <summary>
    /// Diccionaro donde se almancenan los descriptores de modulo
    /// </summary>
    private readonly Dictionary<string, IModuleDescriptor> _moduleDescriptors = new();

    /// <summary>
    /// Registra un modulo en el registro
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Register<T>() where T : class, IModule, new()
    {
        var moduleKey = GetKey<T>();
        var type = typeof(T);
        var name = type.Name.Replace("Module", string.Empty);
        var descriptor = new ModuleDescriptor<T>
        {
            Name = name,
            Assembly = type.Assembly,
            Type = type
        };
        _moduleDescriptors[moduleKey] = descriptor;
    }

    /// <summary>
    /// A partir del tipo, se devuelve el tipo de identificador que requiere segun el
    /// modulo al que pertence, en caso de no estar registrado devuelve nulo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Type? Resolve<T>()
    {
        var key = GetKey<T>();
        return _moduleDescriptors.TryGetValue(key, out var descriptor) ? descriptor.Type : null;
    }

    /// <summary>
    /// Obtiene el nombre del modulo a partir del tipo generico
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string GetKey<T>() => typeof(T).GetModuleName();

    /// <summary>
    /// Obtiene los ensamblados de todos los modulos descritos
    /// </summary>
    public List<Assembly> Assemblies => _moduleDescriptors.Values.Select(x => x.Assembly).ToList();

    /// <summary>
    /// Realiza la configuracion de los ajustes por modulo y los
    /// almancea en la inyeccion de dependencias
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Configure(IConfiguration configuration, IServiceCollection services)
    {
        foreach (var descriptor in _moduleDescriptors.Values)
        {
            descriptor.ConfigureModule(configuration, services);
        }
    }
}
