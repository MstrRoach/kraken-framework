using Kraken.Core;
using Kraken.Core.Features;
using Kraken.Core.Features.Cors;
using Kraken.Core.Features.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host;

public class AppDescriptor
{
    /// <summary>
    /// Lista de ensamblados en donde estan definidos los modulos
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
    /// Contiene las configuraciones necesarias para agregar
    /// la caracteristica de documentacion
    /// </summary>
    public IDocumentationFeature? Documentation { get; set; }

    /// <summary>
    /// Contiene las configuraciones para agregar las cors
    /// </summary>
    public ICorsFeature? Cors { get; set; }

    /// <summary>
    /// Slot para agregar las configuraciones para laa authorizacion
    /// </summary>
    public IAuthorizationFeature? Authorization { get; set; }

    /// <summary>
    /// Lugar donde almacenar las configuraciones para la autenticacion
    /// </summary>
    public IKrakenFeature? Authentication { get; set; }

    /// <summary>
    /// Metodo que limpia las listas de ensamblado y de instancias para liberacion 
    /// de memoria controlada
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    internal void Clear()
    {
        this.modules.Clear();
        this.assemblies.Clear();
    }

    

}
