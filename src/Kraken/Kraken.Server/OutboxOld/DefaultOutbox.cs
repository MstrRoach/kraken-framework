using Humanizer;
using Kraken.Module.Common;
using Kraken.Module.Outbox;
using Kraken.Module.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.OutboxOld;

internal class DefaultOutbox<TModule> : IOutbox
    where TModule : IModule
{
    /// <summary>
    /// Componente que corresponde
    /// </summary>
    private readonly IOutboxStorage<TModule> _storage;

    /// <summary>
    /// Serializador
    /// </summary>
    private readonly IJsonSerializer _serializer;

    /// <summary>
    /// Constructor privado para la construccion explicita del componente
    /// </summary>
    /// <param name="storage"></param>
    public DefaultOutbox(DefaultOutboxStorageFactory storageFactory, IJsonSerializer serializer)
    {
        _storage = storageFactory.Create<TModule>();
        _serializer = serializer;
    }

    /// <summary>
    /// Obtiene el mensaje y lo convierte en una entidad almacenable dentro
    /// de la base de datos utilizando la tienda especificada para esta 
    /// bandeja de salida
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task Save(OutboxMessage message)
    {
        // Creamos la entidad de almacenamiento
        var outboxMsg = new OutboxRecord
        {
            Id = message.Id,
            CorrelationId = message.CorrelationId,
            UserId = message.UserId,
            Name = message.Event.GetType().Name.Underscore(),
            Type = message.Event.GetType().AssemblyQualifiedName,
            Data = _serializer.Serialize(message.Event),
            TraceId = message.TraceId,
            CreatedAt = DateTime.UtcNow
        };
        // La guardamos usando el contenedor
        await _storage.Save(outboxMsg);
    }

    /// <summary>
    /// Se encarga de obtener los eventos que no han sido publicados
    /// convertirlos en mensajes de proceso, y enviarlos al canal de 
    /// procesamiento para su ejecucion
    /// </summary>
    /// <returns></returns>
    public Task PublishUnsent()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Limpia todos los mensajes publicados y que sean mas viejos que la fecha pasada
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    public Task Cleanup(DateTime? to = null)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene el evento por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<OutboxMessage> Get(Guid id)
    {
        OutboxMessage message = null;
        // Obtenemmos el evento desde el storage
        var stored = await _storage.Get(id);
        // Lo convertimos en mensaje de proceso
        if (stored is null)
            return message;
        var @event = _serializer.Deserialize(stored.Data, Type.GetType(stored.Type));
        message = new OutboxMessage(stored.Id, stored.CorrelationId, stored.UserId, stored.Name, stored.TraceId, @event);
        return message;
    }

}
