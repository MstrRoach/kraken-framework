using Kraken.Core.Processing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal
{
    public class KrakenServer : IProcessingServer
    {
        /// <summary>
        /// Origen del token de cancelacion para terminar el 
        /// procesamiento 
        /// </summary>
        private readonly CancellationTokenSource _cts;

        /// <summary>
        /// Logger para el servidor
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Fabrica de logger
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Proveedor de los servicios con ambito
        /// </summary>
        private readonly IServiceProvider _provider;

        public KrakenServer()
        {

        }

        public void Start(CancellationToken stoppingToken)
        {
            // Registramos el evento de cancelacion
            stoppingToken.Register(() => _cts.Cancel());

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
