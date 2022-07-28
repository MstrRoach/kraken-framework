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

        /// <summary>
        /// Devuelve la lista de procesados
        /// </summary>
        /// <returns></returns>
        public Task<List<StorageRecord>> GetUnprocessedRecords()
        {
            // Obtenemos los registros que esten en status en proceso y tengan creados mas de 5 minutos
            var records = StorageRecords.Where(x => x.Status == StorageRecordStatus.OnProcess && DateTime.UtcNow.Subtract(x.CreateAt).Minutes > 5).ToList();
            return Task.FromResult(records);
            //return Task.FromResult(StorageRecords.ToList());
        }
    }
}
