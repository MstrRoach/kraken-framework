using Kraken.Core.Processing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal
{
    /// <summary>
    /// Wrapper para reintentar un proceso de forma indefinida
    /// </summary>
    public class InfiniteRetryProcessor : IProcessor
    {
        /// <summary>
        /// Proceso interno envuelto
        /// </summary>
        private readonly IProcessor _inner;

        /// <summary>
        /// Logger para el procesador en curso
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Creacion del procesador de intentos infinitos
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="loggerFactory"></param>
        public InfiniteRetryProcessor(IProcessor inner, ILoggerFactory loggerFactory)
        {
            _inner = inner;
            _logger = loggerFactory.CreateLogger<InfiniteRetryProcessor>();
        }

        /// <summary>
        /// Procesa infinitamente 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ProcessAsync(ProcessingContext context)
        {
            while (!context.IsStopping)
            {
                try
                {
                    await _inner.ProcessAsync(context);
                }
                catch (OperationCanceledException)
                {
                    // Waited exception, ignore
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Processor '{ProcessorName}' failed. Retrying...", _inner.ToString());
                    await context.WaitAsync(TimeSpan.FromSeconds(2));
                }
            }
        }

        /// <summary>
        /// Retorna el proceso que se esta ejecutando realmente
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _inner.ToString();
        }
    }
}
