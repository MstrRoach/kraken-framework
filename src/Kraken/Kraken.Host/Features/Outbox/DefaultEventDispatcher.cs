using Humanizer;
using Kraken.Core.Outbox;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox;

internal class DefaultEventDispatcher : IEventDispatcher
{

    /// <summary>
    /// Logger del orquestador
    /// </summary>
    private readonly ILogger<DefaultEventDispatcher> _logger;

    /// <summary>
    /// Origen del token de cancelacion
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Canal de procesamiento donde se almacenan los eventos
    /// que deben de procesarse despues que la unidad de
    /// trabajo confirmo cambios
    /// </summary>
    private Channel<ProcessMessage> _raisedEventsChannel = default;

    /// <summary>
    /// Procesador de los eventos locales
    /// </summary>
    private readonly IEventProcessor _processor;

    public DefaultEventDispatcher(ILogger<DefaultEventDispatcher> logger, IEventProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    /// <summary>
    /// Inicia la configuracion del proceso
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Start(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {name} ::: Stage", typeof(DefaultEventDispatcher).Name.Underscore());
        // Configuracion para la detencion del servicio
        stoppingToken.ThrowIfCancellationRequested();
        stoppingToken.Register(() => _cts.Cancel());

        // Configuracion de los canales
        _raisedEventsChannel = Channel.CreateBounded<ProcessMessage>(new BoundedChannelOptions(5000)
        {
            AllowSynchronousContinuations = true,
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        // Ejecutamos la tarea de produccion y configuramos la operacion
        Task.WhenAll(Enumerable.Range(0, 1)
            .Select(_ => Task.Factory.StartNew(
                () => Process(stoppingToken),
                stoppingToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default)
            ));

        _logger.LogInformation("Orchestrator started . . .");
    }

    /// <summary>
    /// Encola el mensaje dentro del canal para su procesamiento asincrono
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void EnqueueToExecute(ProcessMessage message)
    {
        try
        {
            // Si lo logra agregar, salimos
            if (_raisedEventsChannel.Writer.TryWrite(message))
                return;
            // Esperamos para agregarlo al canal
            while (_raisedEventsChannel.Writer.WaitToWriteAsync(_cts.Token).AsTask().ConfigureAwait(false).GetAwaiter().GetResult())
            {
                // Si se agrega salimos
                if (_raisedEventsChannel.Writer.TryWrite(message))
                    return;
            }
        }
        catch(OperationCanceledException)
        {
            // Se cancelo la operacion
        }
    }

    /// <summary>
    /// Realiza el procesamiento para el canal de eventos
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task Process(CancellationToken cancellationToken)
    {
        try
        {
            // Esperammos para leer del canal
            while (await _raisedEventsChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                // Intentamos recuperar un valor
                while (_raisedEventsChannel.Reader.TryRead(out var message))
                {
                    try
                    {
                        _logger.LogInformation("Message content: {data}", message.Event);
                        // Lo enviamos al despachador de eventos
                        await _processor.ProcessAsync(message);
                    }
                    catch (OperationCanceledException)
                    {
                        // Esperado
                    }catch (Exception ex)
                    {
                        _logger.LogError(ex,
                                $"An exception occurred when invoke subscriber. MessageId:{message.Id}");
                    }
                }
            }
        }catch (OperationCanceledException)
        {
            // Indicamos que la operacion se cancelo
            // expected
        }
    }

    /// <summary>
    /// Libera recursos administrados
    /// </summary>
    public void Dispose()
    {
        if (!_cts.IsCancellationRequested)
            _cts.Cancel();
    }
}
