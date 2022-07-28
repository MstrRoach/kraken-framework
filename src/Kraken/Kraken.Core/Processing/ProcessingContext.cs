using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Processing
{
    /// <summary>
    /// Define un objeto compartido entre procesos para el procesamiento asincrono
    /// de tareas
    /// </summary>
    public class ProcessingContext : IDisposable
    {
        /// <summary>
        /// Indica la existencia de un ambito
        /// </summary>
        private IServiceScope? _scope;

        /// <summary>
        /// Proveedor de los servicios
        /// </summary>
        public IServiceProvider Provider { get; private set; }

        /// <summary>
        /// Token de cancelacion para los procesos
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Bandera para saber si la aplicacion esta en proceso de detenimiento
        /// </summary>
        public bool IsStopping => CancellationToken.IsCancellationRequested;

        /// <summary>
        /// Constructor de contexto con proveedor y tokende cancelacion
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        public ProcessingContext(IServiceProvider provider, CancellationToken cancellationToken)
        {
            Provider = provider;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Constructor de contexto con alcance de ambito
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        private ProcessingContext(IServiceScope scope,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            _scope = scope;
            Provider = provider;
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Crea un proceso con ammbito
        /// </summary>
        /// <returns></returns>
        public ProcessingContext CreateScope()
        {
            var serviceScope = Provider.CreateScope();

            return new ProcessingContext(serviceScope,
                serviceScope.ServiceProvider,
                CancellationToken);
        }

        /// <summary>
        /// Realiza un delay de cierto tiempo especificado por parametro
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task WaitAsync(TimeSpan timeout)
        {
            return Task.Delay(timeout, CancellationToken);
        }

        /// <summary>
        /// Lanza una operacion de cancelacion si se ha detenido el sistema o 
        /// cancelado la operacion
        /// </summary>
        public void ThrowIfStopping()
        {
            CancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Liberacion de recursos controlada
        /// </summary>
        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
