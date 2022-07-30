using Kraken.Core.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Processing
{
    internal class KrakenBootstrapper : BackgroundService, IBootstrapper
    {
        /// <summary>
        /// Logger del componente
        /// </summary>
        private readonly ILogger<KrakenBootstrapper> _logger;

        /// <summary>
        /// Proveedor de servicios para recuperar los trabajos en segundo plano
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Lista de procesamientos a correr en segundo plano
        /// </summary>
        private IEnumerable<IProcessingServer> _processors = default!;

        /// <summary>
        /// Origen para el token de cancelacion
        /// </summary>
        private readonly CancellationTokenSource _cts = new();

        /// <summary>
        /// Bandera para indicar si todo el componente fue liberado
        /// </summary>
        private bool _disposed;

        public KrakenBootstrapper(ILogger<KrakenBootstrapper> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Ejecuta el trabajo en segundo plano del arrancador
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => RunAsync();

        /// <summary>
        /// Ejecuta las operaciones del arrancador
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            _logger.LogDebug("### Kraken background task is starting.");

            try
            {
                _logger.LogDebug("### Loading processors to execute");
                // Cargamos los procesadores
                _processors = _serviceProvider.GetServices<IProcessingServer>();
            }
            catch (Exception ex)
            {
                _logger.LogDebug("### Error to load processors.");
            }

            // Registramos la accion para cuando se detenga el procesamiento
            _cts.Token.Register(() =>
            {
                _logger.LogDebug("### Kraken background task is stopping.");

                foreach (var item in _processors)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogWarning(ex, $"Expected an OperationCanceledException, but found '{ex.Message}'.");
                    }
                }
            });

            // Iniciamos el proceso central
            await Process();
            // Ponemos el log de debug
            _logger.LogDebug(logo);
        }

        /// <summary>
        /// Procesamiento principal
        /// </summary>
        /// <returns></returns>
        protected virtual Task Process()
        {
            // Iniciamos el proceso en cada procesador
            foreach (var item in _processors)
            {
                // Lanzamos error si se solicita la cancelarcion
                _cts.Token.ThrowIfCancellationRequested();

                try
                {
                    _logger.LogDebug("Starting process {process} . . .", item.GetType().Name);
                    // Iniciamos cada proceso
                    item.Start(_cts.Token);
                    _logger.LogDebug("Process {process} is started", item.GetType().Name);
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Starting the processors throw an exception.");
                }
            }
            // Salimos
            return Task.CompletedTask;
        }


        /// <summary>
        /// Operacion realizada cuando el trabajo en segundo plano se detiene
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Indicamos con el token de cancelacion que se cancele
            _cts.Cancel();
            // Detenemos el proceso interno
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Libera los recursos administrados por
        /// el servicio en segundo plano
        /// </summary>
        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _cts.Cancel();
            _cts.Dispose();
            _disposed = true;
        }

        private const string logo = @"
===================================================================================================
                                          wmmw.
                                        mmmmmmmmw
                                        mmmmmmmmm
                               .f  )mw  mmmmmmmmm  am   )
                                     mk )mmmmmmm  mk
                                     )mwfmmmmmmmr(m
                             mmmmmm.  fmmmmmmmmwmmr  wmmkmmw
                            (k     fmmmmmmmmkmmmmmmmk.     )r
                             fww..     )mmmmmmkmm     .. wmf
                                  )k  )mm mmmmk mm   m
                                 mk  mmk amk mmw mmm  mw
                                mkwmmr.mmkf   )mmw(kmmwmk
                                    mmf          )fmw
                                    m.   .     .   )m
                                     fff         fff
===================================================================================================
                                  ** Kraken started **
===================================================================================================";
    }
}
