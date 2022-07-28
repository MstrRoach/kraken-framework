
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Processing;

/// <summary>
/// Clase abstracta encargada de la repeticion y control de un proceso
/// especifico
/// </summary>
public abstract class InfinityProcessingServer : IProcessingServer
{
    /// <summary>
    /// Logger para informar del estado del servicio a los llamadores
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Origen del token de cancelacion
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// nombre del proceso actual
    /// </summary>
    private string _processName;

    /// <summary>
    /// Tiempo de espera para reiniciar el servicio
    /// </summary>
    private TimeSpan _timeout;

    /// <summary>
    /// Constructor del proceso infitnito
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="processName"></param>
    /// <param name="timeout"></param>
    public InfinityProcessingServer(ILogger logger, string processName, TimeSpan timeout)
    {
        _logger = logger;
        _processName = processName;
        _timeout = timeout;
    }

    /// <summary>
    /// Inicia la administracion del servicio principal
    /// </summary>
    /// <param name="stoppingToken"></param>
    public void Start(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {name} ::: Stage", _processName);
        // Configuracion para la detencion del servicio
        stoppingToken.ThrowIfCancellationRequested();
        stoppingToken.Register(() => _cts.Cancel());

        // Ejecutamos la tarea de produccion y configuramos la operacion
        Task.WhenAll(Enumerable.Range(0, 1)
            .Select(_ => Task.Factory.StartNew(
                () => Process(stoppingToken),
                stoppingToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default)
            ));
        // Logger final
        _logger.LogInformation("{} started . . .", _processName);
    }

    /// <summary>
    /// Realiza la administracion del proceso principal
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public async Task Process(CancellationToken stoppingToken)
    {
        // Mientras no se pida la cancelacion de la operacion
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Ejecutamos el proceso
                await ProcessAsync();
            }
            catch (OperationCanceledException)
            {
                // Ignoramos por que se detuvo correctammente
            }
            catch (Exception ex)
            {
                // Loggeamos el error 
                _logger.LogWarning(ex, "Process '{Name}' failed. Retrying...", _processName);
                // Esperamos
                await Task.Delay(_timeout, stoppingToken);
            }
        }
    }

    /// <summary>
    /// Procesa la tarea especificada, y monitoreada
    /// de forma permanente
    /// </summary>
    /// <returns></returns>
    public abstract Task ProcessAsync();

    /// <summary>
    /// Libera recursos administrados
    /// </summary>
    public void Dispose()
    {
        if (!_cts.IsCancellationRequested)
            _cts.Cancel();
    }
}
