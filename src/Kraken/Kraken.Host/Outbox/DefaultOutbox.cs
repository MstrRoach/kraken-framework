using Humanizer;
using Kraken.Core.Internal.Serializer;
using Kraken.Core.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    /// <summary>
    /// Laa bandeja de salida estara disponible a nivel
    /// de alcance, para poder tener el almacenamiento
    /// conectado a la misma transaccion en caso de estarse
    /// ejecutando una.
    /// </summary>
    public class DefaultOutbox<TStorage> : IOutbox
        where TStorage : IOutboxStorage
    {
        /// <summary>
        /// Contiene el contenedor 
        /// </summary>
        private readonly TStorage _outboxStore;

        /// <summary>
        /// Serializador del modulo
        /// </summary>
        private readonly IJsonSerializer _serializer;

        public DefaultOutbox(TStorage outboxStore, IJsonSerializer serializer)
        {
            _outboxStore = outboxStore;
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
        public async Task SaveAsync(ProcessMessage message)
        {
            // Creamos la entidad de almacenamiento
            var outboxMsg = new StorageMessage
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
            await _outboxStore.SaveAsync(outboxMsg);
        }

        /// <summary>
        /// Se encarga de obtener los eventos que no han sido publicados
        /// convertirlos en mensajes de proceso, y enviarlos al canal de 
        /// procesamiento para su ejecucion
        /// </summary>
        /// <returns></returns>
        public Task PublishUnsentAsync()
        {
            return Task.CompletedTask;
        }

        public Task CleanupAsync(DateTime? to = null)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Obtiene el evento por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProcessMessage> Get(Guid id)
        {
            ProcessMessage message = null;
            // Obtenemmos el evento desde el storage
            var @event = await _outboxStore.Get(id);
            // Lo convertimos en mensaje de proceso
            if (@event is not null)
                message = new ProcessMessage
                {
                    Id = @event.Id,
                    CorrelationId = @event.CorrelationId,
                    UserId = @event.UserId,
                    TraceId = @event.TraceId,
                    Event = _serializer.Deserialize(@event.Data, Type.GetType(@event.Type))
                };
            return message;
        }
    }
}
