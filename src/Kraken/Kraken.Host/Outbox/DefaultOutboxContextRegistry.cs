using Kraken.Core.Outbox;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox
{
    internal class DefaultOutboxContextRegistry : IOutboxContextRegistry
    {
        /// <summary>
        /// Cache en memoria para almacenar las transacciones actuales
        /// </summary>
        private readonly IMemoryCache _cache;

        public DefaultOutboxContextRegistry(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Agrega una transsaccion y el contexto asociado a ella
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetTransaction(IOutboxContext context) 
            => _cache.Set(context.TransactionId, context, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            });

    }
}
