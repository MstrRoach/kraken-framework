using Kraken.Standard.Server;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server;

/// <summary>
/// Define la estructura, los servicios, los modulos, los
/// features y el pipeline para ejecutar el servidor
/// </summary>
public class ServerDescriptor
{
    /// <summary>
    /// Ensamblados que se cuentan como modulos
    /// </summary>
    internal List<Assembly> assemblies = new List<Assembly>();

    /// <summary>
    /// Lista de modulos conectados al host de kraken
    /// </summary>
    internal List<IModule> modules = new List<IModule>();

    /// <summary>
    /// Agrega los ensamblados de donde provienen los modulos especificados
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddModule<T>() where T : IModule
    {
        // Obtienen el tipo del modulo de entrada
        var type = typeof(T);
        // Si ya existe en la lista de modulos salimos
        if (modules.Any(x => x.GetType() == type))
            return;
        // Creamos una instancia del modulo
        var instance = Activator.CreateInstance(type) as IModule;
        // Agregamos la instancia a la lista de modulos
        this.modules.Add(instance);
        // Obtenemos los ensamblados
        var assembly = type.Assembly;
        // Agregamos el ensamblado a la lista de ensamblados
        this.assemblies.Add(assembly);
    }

    /// <summary>
    /// Define las configuraciones a nivel servicios y pipeline para
    /// agregar documentacion al servidor.
    /// </summary>
    public IFeature? Documentation { get; set; }

    /// <summary>
    /// Contiene las configuraciones para agregar las cors
    /// </summary>
    public IFeature? Cors { get; set; }

    /// <summary>
    /// Slot para agregar las configuraciones para laa authorizacion
    /// </summary>
    public IFeature? Authorization { get; set; }

    /// <summary>
    /// Lugar donde almacenar las configuraciones para la autenticacion
    /// </summary>
    public IFeature? Authentication { get; set; }

    /// <summary>
    /// Indica si la redireccion esta activa
    /// </summary>
    public bool UseHttpsRedirection { get; set; } = false;

    /// <summary>
    /// Contiene la aplicacion web con los servicios agregados
    /// </summary>
    public WebApplication App { get; internal set; }

    /// <summary>
    /// Metodo que limpia las listas de ensamblado y de instancias para liberacion 
    /// de memoria controlada
    /// </summary>
    internal void Clear()
    {
        this.modules.Clear();
        this.assemblies.Clear();
    }
}
