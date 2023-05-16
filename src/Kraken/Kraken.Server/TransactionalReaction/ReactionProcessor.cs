using Kraken.Module.TransactionalReaction;
using Kraken.Server.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalReaction;

internal class ReactionProcessor
{
    /// <summary>
    /// Almacen de las reacciones
    /// </summary>
    private readonly IReactionStorage _storage;

    /// <summary>
    /// Proveedor de los servicios del contenedor
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public ReactionProcessor(IReactionStorage storage, IServiceProvider serviceProvider)
    {
        _storage = storage;
        _serviceProvider = serviceProvider;

    }

    public async Task Process(ReactionMessage message, CancellationToken cancellationToken = default)
    {
        // Creamos el contexto virtual para setearlo en el handler
        var context = new DefaultContext(
            message.CorrelationId,
            message.TraceId,
            new DefaultIdentityContext(
                message.User,
                "Unknow",
                message.Username
                )
            );
        // Creamos el builder del handler
        var reactionHandlerBuilderType = typeof(ReactionHandlerBuilder<,>).MakeGenericType(message.Event.GetType(), message.Reaction);
        // Creamos la instancia
        var reactionHandlerBuilder = (ReactionHandlerBuilderBase)Activator.CreateInstance(reactionHandlerBuilderType) ?? throw new ArgumentNullException($"Could not create wrapper for type {reactionHandlerBuilderType.Name}");
        // Ejecutamos el handler
        await reactionHandlerBuilder.Handle(
            message,
            cancellationToken,
            _serviceProvider,
            context
            );
    }
}
