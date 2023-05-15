using Humanizer;
using Kraken.Module.Common;
using Kraken.Module.TransactionalOutbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

internal sealed class Outbox
{
    /// <summary>
    /// Almacen para la bandeja de salida
    /// </summary>
    private readonly IOutboxStorage _storage;

    /// <summary>
    /// Serializador
    /// </summary>
    private readonly IJsonSerializer _serializer;

    public Outbox(IOutboxStorage storage,
        IJsonSerializer serializer)
    {
        _storage = storage;
        _serializer = serializer;
    }

    /// <summary>
    /// Almacena el event message con la informacion suficiente para
    /// recuperarlo y armar el evento para reenviarlo
    /// </summary>
    /// <param name="eventMessage"></param>
    /// <returns></returns>
    public async Task Save(OutboxMessage eventMessage)
    {
        // Convertimos el outbox message en outbox record para almacenarlo
        var outboxRecord = new OutboxRecord(
            eventMessage.Id,
            eventMessage.CorrelationId,
            eventMessage.TransactionId,
            eventMessage.traceId,
            eventMessage.origin,
            eventMessage.User,
            eventMessage.Username,
            eventMessage.Event.GetType().Name.Underscore(),
            eventMessage.Event.GetType().AssemblyQualifiedName,
            _serializer.Serialize(eventMessage.Event),
            DateTime.UtcNow,
            null,
            OutboxRecordStatus.InTransaction,
            null
        );
        // Lo guardamos en el storage
        await _storage.Save(outboxRecord);
    }

    /// <summary>
    /// Libera los eventos para que sean ejecutados por el procesador de eventos
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public async Task ReleaseEvents(Guid transactionId)
    {
        // Obtenemos los eventos de la transaccion desde el storage
        var events = await _storage.GetAll(transactionId);
        
        // Convertimos todos los registros en eventos
        var messages = events.Select(x => new OutboxMessage(
                x.Id,
                x.CorrelationId, 
                x.TransactionId,
                x.Origin,
                x.User,
                x.Username,
                x.TraceId,
                _serializer.Deserialize(x.Event, Type.GetType(x.EventType))
            )
        );
        // Recorremos la lista de mensajes para ponerlos en cola
        foreach ( var message in messages )
        {
            // Lo agregamos a la cola de ejecucion

        }
        throw new NotImplementedException();
    }
}
