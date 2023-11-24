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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;

namespace Dottex.Kalypso.Server;

/// <summary>
/// Opciones para configurar el servidor de kalypso
/// </summary>
public record KalypsoOptions
{
    /// <summary>
    /// Configuracion del servidor para saber la instancia
    /// </summary>
    public ServerOptions ServerOptions { get; set; } = new();

    /// <summary>
    /// Configuracion para la base de datos del servidor para almacenar
    /// la informacion
    /// </summary>
    public ServerDatabaseOptions DatabaseOptions { get; set; } = new();

    /// <summary>
    /// Configuracion para los nombres de las claims para los tipos especiales
    /// </summary>
    public IdentityContextOptions IdentityContextOptions { get; set; } = new();

    /// <summary>
    /// Diccionario que almacena los modulos con sus tipos de configuracion para
    /// las operaciones donde se requiera diferenciar servicios para cada modulo
    /// </summary>
    internal ModuleRegistry moduleRegistry = new ModuleRegistry();

    /// <summary>
    /// Agrega un modulo a la lista de descriptores para su procesamiento
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddModule<T>() where T : class, IModule, new()
        => moduleRegistry.Register<T>();

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
}