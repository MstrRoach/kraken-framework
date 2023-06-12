using Dottex.Kalypso.Module;
using Dottex.Kalypso.Module.TransactionalOutbox;
using Dottex.Kalypso.Module.TransactionalReaction;
using Dottex.Kalypso.Server.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalReaction;

internal class ReactionOutboxDispatcher : IOutboxDispatcher
{
    /// <summary>
    /// Contiene las relaciones entre eventos y reacciones que responden
    /// </summary>
    private readonly ReactionRegistry _reactionRegistry;

    /// <summary>
    /// Componente encargado de realizar las operaciones alrededor de
    /// las reacciones
    /// </summary>
    private readonly Reactor _reactor;

    public ReactionOutboxDispatcher(ReactionRegistry reactionRegistry,
        Reactor reactor)
    {
        _reactionRegistry = reactionRegistry;
        _reactor = reactor;
    }

    public async Task Process(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        // Obtenemos los handlers para los eventos entrantes
        var reactions = _reactionRegistry.Resolve(message.Event.GetType());
        // Creamos los mensajes de inbox
        var inboxMessages = reactions.Select(x => new ReactionMessage
        {
            Id = Guid.NewGuid(),
            CorrelationId = message.CorrelationId,
            TraceId = message.TraceId,
            User = message.User,
            Username = message.Username,
            Origin = message.Origin,
            Target = x.GetModuleName(),
            EventId = message.Id,
            Event = message.Event,
            Reaction = x
        }).ToList();
        // Enviamos las reacciones al concentrador
        await _reactor.Save(inboxMessages);
    }
}
