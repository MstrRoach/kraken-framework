using Kraken.Module.OutboxOld;
using Kraken.Module.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infrastructure;

/// <summary>
/// Implementacion por defecto para el almacen de los
/// mensajes de salida
/// </summary>
/// <typeparam name="T"></typeparam>
internal class CommonOutboxStorage : IOutboxStorage<AccessControlModule>
{
    private static ConcurrentBag<OutboxRecord> outboxMessages = new ConcurrentBag<OutboxRecord>();

    /// <summary>
    /// Almmacena el mensaje en la lista de mensajes almacenables
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task Save(OutboxRecord message)
    {
        outboxMessages.Add(message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene todos los menssajes que no estan enviados
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<OutboxRecord>> GetUnsentAsync()
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
    public Task<OutboxRecord> Get(Guid id)
    {
        var message = outboxMessages.Where(x => x.Id == id).FirstOrDefault();
        return Task.FromResult(message);
    }




}
