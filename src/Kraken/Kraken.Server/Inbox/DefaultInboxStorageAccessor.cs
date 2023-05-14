using Humanizer;
using Kraken.Module.Inbox;
using Kraken.Module.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

/// <summary>
/// Fabrica para construccion de los
/// almacenadores de handlers por modulo
/// Los handlers se almacenan en el storage
/// </summary>
public class DefaultInboxStorageAccessor
{
    /// <summary>
    /// Proveedor de servicios para crear los storages
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Lista de modulos asociados a su configurador
    /// </summary>
    private readonly ModuleRegistry _moduleRegistry;

    public DefaultInboxStorageAccessor(IServiceProvider serviceProvider,
        ModuleRegistry moduleRegistry)
    {
        _serviceProvider = serviceProvider;
        _moduleRegistry = moduleRegistry;
    }


    /// <summary>
    /// Almacena todos los handlers en sus respectivos
    /// almacenamientos segun en donde se deben de generar
    /// las operaciones
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    public async Task SaveAll(List<InboxMessage> records)
    {
        // Recorremos la lista de records, los convertimos en records
        // y obtenemos el modulo al que pertenecen, para agregarlo al
        // diccionario, y acumular los records a partir del modulo al que pertenecen
        var moduleHandlersRelation = records.Aggregate(new Dictionary<Type, List<InboxRecord>>(), (acc, current) =>
        {
            var record = new InboxRecord
            {
                Id = current.Id,
                CorrelationId = current.CorrelationId,
                EventId = current.EventId,
                EventType = current.Event.AssemblyQualifiedName,
                Name = current.Handler.Name.Underscore(),
                Type = current.Handler.AssemblyQualifiedName,
                CreateAt = DateTime.UtcNow,
                Status = InboxRecordStatus.OnProcess
            };
            var module = _moduleRegistry.Resolve(current.Handler);
            if (!acc.TryGetValue(module, out var moduleList))
            {
                moduleList = new List<InboxRecord>();
                acc.Add(module, moduleList);
            }
            moduleList.Add(record);
            return acc;
        });
        // Creamos un alcance para obtener los servicios
        var scope = _serviceProvider.CreateScope();
        // Creamos el tipo del reaction storage
        var inboxStorageOpenType = typeof(IInboxStorage<>);
        var defaultInboxStorageOpenType = typeof(DefaultInboxStorage<>);
        // Recorremos las llaves del diccionario
        foreach (var module in moduleHandlersRelation.Keys)
        {
            // Creamos el tipo cerrado
            var inboxStorageType = inboxStorageOpenType.MakeGenericType(module);
            // Obtenemos el servicio
            var inboxStorage = scope.ServiceProvider.GetService(inboxStorageType) as IInboxStorage;
            // Si no es nulo guardamos los records pertenecientes y continuamos
            if (inboxStorage is not null)
            {
                await inboxStorage.SaveAll(moduleHandlersRelation[module]);
                continue;
            }
            // Creamos el reaction storage por defecto
            var defaultInboxStorageType = defaultInboxStorageOpenType.MakeGenericType(module);
            // Obtenemos el servicio
            var defaultInboxStorage = scope.ServiceProvider.GetService(defaultInboxStorageType) as IInboxStorage;
            // Guardamos el registro
            await defaultInboxStorage.SaveAll(moduleHandlersRelation[module]);
        }
    }
}
