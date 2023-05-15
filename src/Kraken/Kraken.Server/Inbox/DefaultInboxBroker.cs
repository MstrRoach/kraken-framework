using Kraken.Module.Inbox;
using Kraken.Module.Outbox;
using Kraken.Module.Processing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Kraken.Server.Inbox;

/// <summary>
/// Encargado de agregar en una cola de ejecucion a todas las
/// reacciones que debaan ejecutarse
/// </summary>
internal class DefaultInboxBroker : IProcessingService
{
    /// <summary>
    /// Logger para el broker
    /// </summary>
    private readonly ILogger<DefaultInboxBroker> _logger;

    /// <summary>
    /// Creador de token de cancelacion
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Canal para recibir y desde el cual procesar los eventos
    /// en cola
    /// </summary>
    private Channel<InboxMessage> _receivedEventsChannel = default;
    
    /// <summary>
    /// Despachador de los eventos de entrada
    /// </summary>
    private readonly IInboxDispatcher _dispatcher;

    public DefaultInboxBroker(ILogger<DefaultInboxBroker> logger,
        IInboxDispatcher dispatcher)
    {
        _logger = logger;
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Inicia la configuracion del procesador de eventos 
    /// de entrada
    /// </summary>
    /// <param name="stoppingToken"></param>
    public void Start(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Inbox] Staarting {name} processing >>>>>>", typeof(DefaultInboxBroker));
        // Configuracion para laa cancelacion del servicio
        stoppingToken.ThrowIfCancellationRequested();
        stoppingToken.Register(() => _cts.Cancel());
        // Configuracion de los canales
        _receivedEventsChannel = Channel.CreateBounded<InboxMessage>(new BoundedChannelOptions(5000)
        {
            AllowSynchronousContinuations = true,
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait
        });
        // ejecutamos la tarea de recepcion y configuramos la operacion
        Task.WhenAll(Enumerable.Range(0,1)
            .Select(_ => Task.Factory.StartNew(() => Process(stoppingToken),
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default)
            )
        );
        _logger.LogInformation("Receptor started . . .");
    }

    /// <summary>
    /// Realiza el procesamiento infinito para la cola en el
    /// canal configurado
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task Process(CancellationToken cancellationToken)
    {
        try
        {
            // Esperammos para leer el canal
            while (await _receivedEventsChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                // Intentamos recuperar un valor
                while (_receivedEventsChannel.Reader.TryRead(out var message))
                {
                    try
                    {
                        _logger.LogInformation("Message content: {data}", message.Event);
                        // Lo enviamos al despachador de eventos
                        //await _dispatcher.ProcessAsync(message);
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

        }
    }

    /// <summary>
    /// Recibe un mensaje y lo coloca en la cola de ejecucion
    /// </summary>
    /// <param name="message"></param>
    public void EnqueueToExecute(InboxMessage message)
    {
        try
        {
            // Si lo logra agregar, salimos
            if (_receivedEventsChannel.Writer.TryWrite(message))
                return;
            // Esperamos para agregarlo al canal
            while (_receivedEventsChannel
                .Writer
                .WaitToWriteAsync(_cts.Token)
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult())
            {
                // Si se agrega salimos
                if (_receivedEventsChannel.Writer.TryWrite(message))
                    break;
            }

        }catch (OperationCanceledException ex)
        {
            // Se cancelo la operacion
        }
        catch (Exception ex)
        {
            _logger.LogError("[Inbox Enqueue] Error to enqueue {name} to execution", message.GetType().Name);
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
