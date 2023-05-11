using Kraken.Standard.Outbox;
using Kraken.Standard.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Outbox;

/// <summary>
/// Implementacion por defecto para el almacen de los
/// mensajes de salida
/// </summary>
/// <typeparam name="T"></typeparam>
internal class DefaultOutboxStorage<T> : IOutboxStorage<T>
    where T : IModule
{
    private static ConcurrentBag<StorableMessage> outboxMessages = new ConcurrentBag<StorableMessage>();

    /// <summary>
    /// Almmacena el mensaje en la lista de mensajes almacenables
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task Save(StorableMessage message)
    {
        outboxMessages.Add(message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene todos los menssajes que no estan enviados
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<StorableMessage>> GetUnsentAsync()
    {
        var unsent = outboxMessages.Where(x => x.SentAt is null);
        return Task.FromResult(unsent);
    }

    /// <summary>
    /// Limpia la lista de mensajes almacenados
    /// </summary>
    /// <returns></returns>
    public Task Cleanup()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene un menssaje por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<StorableMessage> Get(Guid id)
    {
        var message = outboxMessages.Where(x => x.Id == id).FirstOrDefault();
        return Task.FromResult(message);
    }

    

    
}
