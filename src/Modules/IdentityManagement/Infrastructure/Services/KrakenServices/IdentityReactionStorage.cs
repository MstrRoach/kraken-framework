using Kraken.Core.Reaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Infrastructure.Services.KrakenServices
{
    internal class IdentityReactionStorage : IReactionStorage
    {
        private static ConcurrentBag<StorageRecord> StorageRecords = new ConcurrentBag<StorageRecord>();

        public IdentityReactionStorage()
        {

        }

        /// <summary>
        /// Agrega un registro al contenedor de los registros
        /// almacenados para las reacciones
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public Task SaveAsync(StorageRecord record)
        {
            StorageRecords.Add(record);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Actualiza el status y la fecha de actualizacion
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Task MarkAsDone(Guid guid)
        {
            var record = StorageRecords.First(x => x.Id == guid);
            if(record is not null)
            {
                record.Status = StorageRecordStatus.Processed;
                record.UpdateAt = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }
    }
}
