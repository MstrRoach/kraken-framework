using Humanizer;
using Kraken.Core.Reaction;
using Kraken.Core.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    internal class DefaultReactionStream<T> : IReactionStream
        where T : IReactionStorage
    {
        /// <summary>
        /// Almacen especifico para almacener la reaccion
        /// </summary>
        private readonly T _storage;

        public DefaultReactionStream(T storage)
        {
            _storage = storage;
        }

        

        /// <summary>
        /// Realiza las operaciones de almacenaje para el registro
        /// pasado por parametro
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task SaveAsync(ProcessRecord record)
        {
            // Convertimos la entidad de process en storage
            var storageRecord = new StorageRecord
            {
                Id = record.Id,
                EventId = record.EventId,
                Name = record.Reaction.Name.Underscore(),
                Type = record.Reaction.AssemblyQualifiedName,
                CreateAt = DateTime.UtcNow
            };
            // La guardamos en el almacen definido
            await _storage.SaveAsync(storageRecord);
        }

        /// <summary>
        /// Actualiza el estado para un registro con el id pasado 
        /// por parametro
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task MarkReactionAsDone(Guid guid)
        {
            await _storage.MarkAsDone(guid);
        }
    }
}
