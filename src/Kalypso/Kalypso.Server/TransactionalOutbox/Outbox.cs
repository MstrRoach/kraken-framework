using Humanizer;
using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.TransactionalOutbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

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

    /// <summary>
    /// Componente encargado de encolar los eventos y ejecutarlos
    /// </summary>
    private readonly OutboxBroker _outboxBroker;

    public Outbox(IOutboxStorage storage,
        IJsonSerializer serializer,
        OutboxBroker outboxBroker)
    {
        _storage = storage;
        _serializer = serializer;
        _outboxBroker = outboxBroker;
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
        var outboxRecord = new OutboxRecord
        {
            Id = eventMessage.Id,
            CorrelationId = eventMessage.CorrelationId,
            TransactionId = eventMessage.TransactionId,
            TraceId = eventMessage.TraceId,
            Origin = eventMessage.Origin,
            User = eventMessage.User,
            Username = eventMessage.Username,
            EventName = eventMessage.Event.GetType().Name.Underscore(),
            EventType = eventMessage.Event.GetType().AssemblyQualifiedName,
            Event = _serializer.Serialize(eventMessage.Event),
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            Status = OutboxRecordStatus.Pending
        };
        // Lo guardamos en el storage
        await _storage.Save(outboxRecord);
    }

    /// <summary>
    /// Libera los eventos para que sean ejecutados por el procesador de eventos
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public async Task Publish(Guid transactionId)
    {
        // Obtenemos los eventos de la transaccion desde el storage
        var events = await _storage.GetAll(transactionId);
        // Actualizamos los eventos para indicar su intento de procesamiento
        var updatedEvents = events.ToList().Select(x => x with
        {
            ConfirmedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            Status = OutboxRecordStatus.OnProcess
        });
        // Registramos en el storage
        await _storage.UpdateAll(updatedEvents);
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
        foreach (var message in messages)
            _outboxBroker.EnqueueToExecute(message);
    }

    /// <summary>
    /// Cancela los eventos para la transaccion pasada por parametro
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public async Task Cleanup(Guid transactionId)
    {
        // Llamamos al storage para eliminar los registros que pertenecen a una transaccion
        await _storage.DeleteAll(transactionId);
    }
}
