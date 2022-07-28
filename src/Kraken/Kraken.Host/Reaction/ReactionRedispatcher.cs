using Humanizer;
using Kraken.Core.Contexts;
using Kraken.Core.Outbox;
using Kraken.Core.Processing;
using Kraken.Core.Reaction;
using Kraken.Host.Contexts;
using Kraken.Host.Internal;
using Kraken.Host.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction;

/// <summary>
/// Proceso encargado de la
/// </summary>
internal class ReactionRedispatcher : InfinityProcessingServer
{
   
    /// <summary>
    /// Indica la cantidad de tiempo que espera entre procesamiento
    /// </summary>
    private int ProcessDelay => 60;

    /// <summary>
    /// Proveedor de los servicios necesarios para la
    /// ejecucion de las tareas
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Contiene el registro de los storages para la construccion 
    /// de un outbox especifico
    /// </summary>
    private readonly OutboxStoreRegistry _outboxStoreRegistry;

    /// <summary>
    /// Constructor del servicio infinito
    /// </summary>
    /// <param name="logger"></param>
    public ReactionRedispatcher(ILogger<ReactionRedispatcher> logger, IServiceProvider serviceProvider,
        OutboxStoreRegistry outboxStoreRegistry)
        : base(logger, typeof(ReactionRedispatcher).Name, TimeSpan.FromSeconds(2))
    {
        _serviceProvider = serviceProvider;
        _outboxStoreRegistry = outboxStoreRegistry;
    }

    /// <summary>
    /// Proceso que estara protegido para reiniciarse en cuanto falle
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task ProcessAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[ReactionRedispatcher] Starting reaction redispatch... ");
        // Ejecutamos la logica que se repetira infinitamente
        using var scope = _serviceProvider.CreateScope();
        // Obtenemos las reacciones que no se han ejecutado
        var reactionStorages = scope.ServiceProvider.GetServices<IReactionStream>();
        // Obtenemos todas las reacciones, y las agrupamos por tipo de evento, para ejecutar los eventos juntos
        var unproccessedReactions = reactionStorages
            .SelectMany(x => x.GetUnprocessedRecords().ConfigureAwait(false).GetAwaiter().GetResult())
            .ToList();
        // Recorremos para cargar los distintos eventos que contienen
        var events = unproccessedReactions
            .Select(x => new EventDetail { Id = x.EventId, Type = x.Event, Event = null })
            .GroupBy(x => x)
            .Select(x => x.FirstOrDefault())
            .Select(x => x with { Event = GetOutbox(x.Type, scope.ServiceProvider).Get(x.Id) })
            .ToDictionary(x => x.Id, x => x);
        // Disponemos a recorrer las reacciones
        foreach (var reaction in unproccessedReactions)
        {
            // Creamos el contexto
            var context = this.GetContext(reaction);
            // Construimos el tipo cerrado para construir el handler
            var closedReactionBuilder = typeof(ReactionBuilder<,>).MakeGenericType(reaction.Event,reaction.Reaction);
            // Creamos la instancia
            var reactionBuilder = (ReactionBuilderBase)Activator.CreateInstance(closedReactionBuilder);
            // Si el builder es nulo, continuamos con el siguiente
            if (reactionBuilder is null)
                break;
            try
            {
                // Ejecutamos el handler
                await reactionBuilder.Handle(events[reaction.EventId],
                    reaction, 
                    cancellationToken, 
                    _serviceProvider,
                    context);
            }
            catch (Exception ex)
            {
                // Si el handler falla, entonces, ejecutamos el siguiente
                continue;
            }
        }
        // Esperamos para la siguiente ejecucion
        await Task.Delay(TimeSpan.FromSeconds(ProcessDelay));
    }

    /// <summary>
    /// Obtiene el evento convertido en objeto y cargado desde la bandeja de
    /// salida especificada
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventId"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    private object GetEvent(Type eventType, Guid eventId,IServiceProvider serviceProvider)
    {
        var outbox = GetOutbox(eventType, serviceProvider);
        return outbox.Get(eventId).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Construye el contexto a partir de un registro de proceso
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    private IContext GetContext(ProcessRecord record)
    {
        var identity = new IdentityContext(Guid.Empty,"Kraken Redispatcher");
        return new Context(record.CorrelationId, $"{Guid.NewGuid():N}", identity);
    }

    /// <summary>
    /// Obtiene la bandeja de salida para el evento pasado por parametro
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    private IOutbox GetOutbox(Type eventType, IServiceProvider serviceProvider)
    {
        var outboxStorage = _outboxStoreRegistry.Resolve(eventType);
        var outboxClosedType = typeof(DefaultOutbox<>).MakeGenericType(outboxStorage);
        return serviceProvider.GetService(outboxClosedType) as IOutbox;
    }

}

public record EventDetail
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Typo de evento
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Dato del evento
    /// </summary>
    public object Event { get; set; }
}

