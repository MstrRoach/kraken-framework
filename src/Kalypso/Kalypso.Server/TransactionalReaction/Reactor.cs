using Humanizer;
using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.TransactionalReaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

/// <summary>
/// Commponente encargado de administrar las operaciones
/// de reaccion a eventos
/// </summary>
internal class Reactor
{
    /// <summary>
    /// Serializador
    /// </summary>
    private readonly IJsonSerializer _serializer;

    /// <summary>
    /// Almacen para las reacciones
    /// </summary>
    private readonly IReactionStorage _reactionStorage;

    /// <summary>
    /// Proceso para centralizar en una cola de ejecucion
    /// las reacciones a ejecutar
    /// </summary>
    private readonly ReactionBroker _broker;

    public Reactor(IJsonSerializer serializer,
        IReactionStorage reactionStorage,
        ReactionBroker broker)
    {
        _serializer = serializer;
        _reactionStorage = reactionStorage;
        _broker = broker;
    }

    /// <summary>
    /// Almacena las reacciones en el storage definido y los
    /// pone en cola de ejecucion en el broker para su procesamiento
    /// </summary>
    /// <param name="reactions"></param>
    /// <returns></returns>
    public async Task Save(List<ReactionMessage> reactions)
    {
        // Convertimos las reacciones en registros de almacenamiento
        var reactionRecords = reactions.Select(x => new ReactionRecord
        {
            Id = x.Id,
            CorrelationId = x.CorrelationId,
            TraceId = x.TraceId,
            User = x.User,
            Username = x.Username,
            Origin = x.Origin,
            Target = x.Target,
            EventId = x.EventId,
            EventName = x.Event.GetType().Name.Underscore(),
            EventType = x.Event.GetType().AssemblyQualifiedName,
            Event = _serializer.Serialize(x.Event),
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            LastAttemptAt = DateTime.UtcNow,
            Status = ReactionRecordStatus.OnProcess,
            ReactionType = x.Reaction.GetType().AssemblyQualifiedName
        }).ToList();
        // Guardamos las reacciones
        await _reactionStorage.SaveAll(reactionRecords);
        // Las agregamos a la cola de ejecucion
        foreach (var reaction in reactions)
            _broker.EnqueueToExecute(reaction);
    }

}
