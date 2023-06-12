using Humanizer;
using Dottex.Kalypso.Module.Processing;
using Dottex.Kalypso.Module.TransactionalReaction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

internal class ReactionBroker : IProcessingService
{
    /// <summary>
    /// Logger del broker
    /// </summary>
    private readonly ILogger<ReactionBroker> _logger;

    /// <summary>
    /// Origen del token de cancelacion
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Canal de procesamiento donde se almacenan los eventos
    /// que deben de procesarse despues que la unidad de
    /// trabajo confirmo cambios
    /// </summary>
    private Channel<ReactionMessage> _raisedReactionsChannel = default;

    /// <summary>
    /// Procesador de los eventos
    /// </summary>
    private readonly ReactionProcessor _processor;

    public ReactionBroker(ILogger<ReactionBroker> logger, ReactionProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    public void Start(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Reactor] Starting {name} processing >>>>>>", typeof(ReactionBroker).Name.Underscore());
        // Configuracion para la detencion del servicio
        stoppingToken.ThrowIfCancellationRequested();
        stoppingToken.Register(() => _cts.Cancel());
        // Configuracion de los canales
        _raisedReactionsChannel = Channel.CreateBounded<ReactionMessage>(new BoundedChannelOptions(5000)
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
    /// Realiza el procesamiento para el canal de eventos
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task Process(CancellationToken cancellationToken)
    {
        try
        {
            // Esperamos para leer del canal
            while (await _raisedReactionsChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                // Intentamos recuperar un valor
                while (_raisedReactionsChannel.Reader.TryRead(out var message))
                {
                    try
                    {
                        _logger.LogInformation("Message content: {data}", message.Event);
                        // Lo enviamos al despachador de eventos
                        await _processor.Process(message);
                    }
                    catch (OperationCanceledException)
                    {
                        // Esperado
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                                $"An exception occurred when invoke subscriber. MessageId:{message.Id}");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Indicamos que la operacion se cancelo
            // expected
        }
    }

    /// <summary>
    /// Encola el mensaje dentro del canal para su procesamiento asincrono
    /// </summary>
    /// <param name="message"></param>
    public void EnqueueToExecute(ReactionMessage message)
    {
        try
        {
            // Si lo logra agregar, salimos
            if (_raisedReactionsChannel.Writer.TryWrite(message))
                return;
            // Esperamos para agregarlo al canal
            while (_raisedReactionsChannel.Writer.WaitToWriteAsync(_cts.Token).AsTask().ConfigureAwait(false).GetAwaiter().GetResult())
            {
                // Si se agrega salimos
                if (_raisedReactionsChannel.Writer.TryWrite(message))
                    return;
            }
        }
        catch (OperationCanceledException)
        {
            // Se cancelo la operacion
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
