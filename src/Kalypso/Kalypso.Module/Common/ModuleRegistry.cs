using Dottex.Kalypso.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Common;

public sealed class ModuleRegistry
{
    /// <summary>
    /// Contiene la relacion de modulos con su tipo diferenciador que es el 
    /// utilizado para la configuracion del mismo.
    /// </summary>
    private readonly Dictionary<string, Type> _modules = new();

    /// <summary>
    /// Registra un configurador de modulo y lo asocia al modulo que pertenece
    /// para las operaciones donde se utilice para identificar a servicios especificos
    /// de modulos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Register<T>() where T : IModule
        => _modules[GetKey<T>()] = typeof(T);

    /// <summary>
    /// A partir del tipo, se devuelve el tipo de identificador que requiere segun el
    /// modulo al que pertence, en caso de no estar registrado devuelve nulo
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Type? Resolve<T>() => _modules.TryGetValue(GetKey<T>(), out var type) ? type : null;

    /// <summary>
    /// Obtiene a partir del tipo especificado el modulo al que pertenece y con ello
    /// el configurador registrado segun ese ensamblado
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Type? Resolve(Type type) => _modules.TryGetValue(type.GetModuleName(), out var module) ? module : null;

    /// <summary>
    /// Obtiene el nombre del modulo a partir del tipo generico
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string GetKey<T>() => typeof(T).GetModuleName();

}
