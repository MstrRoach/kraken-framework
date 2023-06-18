using Dottex.Kalypso.Server.Middlewares.Contexts;
using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.TransactionalOutbox;
using Dottex.Kalypso.Module.TransactionalReaction;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dottex.Kalypso.Module.Audit;

namespace Dottex.Kalypso.Server;

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
    /// Lista de modulos conectados al host de Kalypso
    /// </summary>
    internal List<IModule> modules = new List<IModule>();

    /// <summary>
    /// Diccionario que almacena los modulos con sus tipos de configuracion para
    /// las operaciones donde se requiera diferenciar servicios para cada modulo
    /// </summary>
    internal ModuleRegistry moduleRegistry = new ModuleRegistry();

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
        modules.Add(instance);
        // Obtenemos los ensamblados
        var assembly = type.Assembly;
        // Agregamos el ensamblado a la lista de ensamblados
        assemblies.Add(assembly);
        // Agregamos la relacion de modulos con sus clases diferenciadoras
        moduleRegistry.Register<T>();
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
    /// Bandera para habilitar el mostrar u ocultar la documentacion
    /// </summary>
    public bool ShowDocumentation { get; set; } = false;

    /// <summary>
    /// Objeto para definir las propiedades de claim para obtener los valores
    /// en el contexto de identidad
    /// </summary>
    public IdentityContextProperties IdentityContextProperties { get; set; } = new();

    /// <summary>
    /// Contiene las configuraciones para la base de datos en memoria
    /// </summary>
    public ServerDatabaseProperties ServerDatabaseProperties { get; set; } = new();

    /// <summary>
    /// Contiene la aplicacion web con los servicios agregados
    /// </summary>
    public WebApplication App { get; internal set; }

    /// <summary>
    /// Descriptor del servicio de bandeja de salida
    /// </summary>
    internal ServiceDescriptor? OutboxStorageDescriptor;

    /// <summary>
    /// Permite registrar un servicio para el almacenamiento para la
    /// bandeja de entrada transaccional
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddOutboxStorage<T>()
        where T : IOutboxStorage
    {
        OutboxStorageDescriptor = ServiceDescriptor.Describe(
            typeof(IOutboxStorage),
            typeof(T),
            ServiceLifetime.Singleton
        );
    }

    /// <summary>
    /// Descriptor del servicio de almacenamiento para las reacciones
    /// </summary>
    internal ServiceDescriptor? ReactionStorageDescriptor;

    /// <summary>
    /// Permite registrar un servicio para el almacenamiento para
    /// las reacciones
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddReactionStorage<T>()
        where T : IReactionStorage
    {
        ReactionStorageDescriptor = ServiceDescriptor.Describe(
            typeof(IReactionStorage),
            typeof(T),
            ServiceLifetime.Singleton
        );
    }

    /// <summary>
    /// Descriptor para el almacenamiento para las auditorias
    /// </summary>
    internal ServiceDescriptor? AuditStorageDescriptor;

    /// <summary>
    /// Permite registrar un servicio para almacenamiento dedica a auditoria
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddAuditStorage<T>()
        where T : IAuditStorage
    {
        AuditStorageDescriptor = ServiceDescriptor.Describe(
            typeof(IAuditStorage),
            typeof(T),
            ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Metodo que limpia las listas de ensamblado y de instancias para liberacion 
    /// de memoria controlada
    /// </summary>
    internal void Clear()
    {
        modules.Clear();
        assemblies.Clear();
    }

}
