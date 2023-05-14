
using Kraken.Module.Outbox;
using Kraken.Module.Reaction;
using Kraken.Server.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Reaction;

internal class DefaultReactionDispatcher : IOutboxDispatcher
{
    /// <summary>
    /// Registro donde tenemos el control de las reacciones que responden a cada evento
    /// </summary>
    private readonly ReactionRegistry _reactionRegistry;

    /// <summary>
    /// Proveedor de los servicios del contenedor
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Fabrica para la obtencion del almacen de reacciones
    /// </summary>
    private readonly DefaultReactionStorageAccessor _reactionStorageFactory;

    /// <summary>
    /// Constructor del processador de reacciones
    /// </summary>
    /// <param name="reactionRegistry"></param>
    /// <param name="serviceProvider"></param>
    public DefaultReactionDispatcher(ReactionRegistry reactionRegistry,
            IServiceProvider serviceProvider,
            DefaultReactionStorageAccessor reactionStorageFactory)
    {
        _reactionRegistry = reactionRegistry;
        _serviceProvider = serviceProvider;
        _reactionStorageFactory = reactionStorageFactory;
    }

    public async Task ProcessAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        // Obtenemos las reacciones paraa el evento
        var reactions = _reactionRegistry.Resolve(message.Event.GetType());
        // Creamos el contexto virtual para compartirlo con las reacciones
        var context = new DefaultContext(
            message.CorrelationId,
            message.TraceId, 
            new DefaultIdentityContext(
                message.UserId, 
                "Unknow", 
                message.Username
                )
            );
        // Convertimos las reacciones en registro de procesamiento
        var reactionMessages = reactions.Select(x => new ReactionMessage
        {
            Id = Guid.NewGuid(),
            EventId = message.Id,
            Event = message.Event.GetType(),
            CorrelationId = message.CorrelationId,
            Reaction = x
        }).ToList();
        // Mandamos a guardar los registros de reaccion
        await _reactionStorageFactory.SaveAll(reactionMessages);
        // Creamos el tipo generico para el builder de reacciones
        var reactionBuilderOpenType = typeof(ReactionBuilder<,>);
        // Recorremos las reacciones para procesarlas
        foreach (var reaction in reactionMessages)
        {
            // Creamos el wrapper para la reaccion
            var reactionBuilderType = reactionBuilderOpenType.MakeGenericType(reaction.Event, reaction.Reaction);
            try
            {
                // Creamos la instancia
                var reactionBuilder = (ReactionBuilderBase)Activator.CreateInstance(reactionBuilderType) ?? throw new ArgumentNullException($"Could not create wrapper for type {reactionBuilderType.Name}");
                // Ejecutamos el handler con toda la informacion necesaria
                await reactionBuilder.Handle(reaction.Event, reaction, cancellationToken, _serviceProvider, context);
            }
            catch (Exception ex)
            {
                // Indicamos que no se proceso completamente el mensaje
                continue;
            }
        }
    }
}
