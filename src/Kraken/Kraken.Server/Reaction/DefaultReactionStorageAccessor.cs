using Humanizer;
using Kraken.Module.Reaction;
using Kraken.Module.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Reaction;

/// <summary>
/// Fabrica para construccion de los
/// almacenadores de reacciones por modulo
/// Las reacciones se almacenan en el storage
/// </summary>
public class DefaultReactionStorageAccessor
{
    /// <summary>
    /// Proveedor de servicios para crear los storages
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Lista de modulos asociados a su configurador
    /// </summary>
    private readonly ModuleRegistry _moduleRegistry;

    public DefaultReactionStorageAccessor(IServiceProvider serviceProvider,
        ModuleRegistry moduleRegistry)
    {
        _serviceProvider = serviceProvider;
        _moduleRegistry = moduleRegistry;
    }


    /// <summary>
    /// Almacena todas las reacciones en sus respectivos
    /// almacenamientos segun en donde se deben de generar
    /// las reacciones
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    public async Task SaveAll(List<ReactionMessage> records)
    {
        // Recorremos la lista de records, los convertimos en records
        // y obtenemos el modulo al que pertenecen, para agregarlo al
        // diccionario, y acumular los records a partir del modulo al que pertenecen
        var moduleReactionRelation = records.Aggregate(new Dictionary<Type, List<ReactionRecord>>(),(acc, current) =>
        {
            var record = new ReactionRecord
            {
                Id = current.Id,
                CorrelationId = current.CorrelationId,
                EventId = current.EventId,
                EventType = current.Event.AssemblyQualifiedName,
                Name = current.Reaction.Name.Underscore(),
                Type = current.Reaction.AssemblyQualifiedName,
                CreateAt = DateTime.UtcNow,
                Status = ReactionRecordStatus.OnProcess
            };
            var module = _moduleRegistry.Resolve(current.Reaction);
            if (!acc.TryGetValue(module, out var moduleList))
            {
                moduleList = new List<ReactionRecord>();
                acc.Add(module, moduleList);
            }
            moduleList.Add(record);
            return acc;
        });
        // Creamos un alcance para obtener los servicios
        var scope = _serviceProvider.CreateScope();
        // Creamos el tipo del reaction storage
        var reactionStorageOpenType = typeof(IReactionStorage<>);
        var defaultReactionStorageOpenType = typeof(DefaultReactionStorage<>);
        // Recorremos las llaves del diccionario
        foreach (var module in moduleReactionRelation.Keys)
        {
            // Creamos el tipo cerrado
            var reactionStorageType = reactionStorageOpenType.MakeGenericType(module);
            // Obtenemos el servicio
            var reactionStorage = scope.ServiceProvider.GetService(reactionStorageType) as IReactionStorage;
            // Si no es nulo guardamos los records pertenecientes y continuamos
            if(reactionStorage is not null)
            {
                await reactionStorage.SaveAll(moduleReactionRelation[module]);
                continue;
            }
            // Creamos el reaction storage por defecto
            var defaultReactionStorageType = defaultReactionStorageOpenType.MakeGenericType(module);
            // Obtenemos el servicio
            var defaultReactionStorage = scope.ServiceProvider.GetService(defaultReactionStorageType) as IReactionStorage;
            // Guardamos el registro
            await defaultReactionStorage.SaveAll(moduleReactionRelation[module]);
        }
    }
}
