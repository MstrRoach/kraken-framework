using Kraken.Module.Outbox;
using Kraken.Module.Inbox;
using Kraken.Server.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Kraken.Server.Inbox;

internal class InboxOutboxDispatcher : IOutboxDispatcher
{
    /// <summary>
    /// Registro donde tenemos el control de los handlers que corresponden a cada
    /// evento entrante
    /// </summary>
    private readonly InboxHandlerRegistry _inboxHandlerRegistry;

    /// <summary>
    /// Proveedor de los servicios del contenedor
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Fabrica para la obtencion del almacen de reacciones
    /// </summary>
    private readonly DefaultInboxStorageAccessor _inboxStorageAccessor;

    /// <summary>
    /// Constructor del processador de reacciones
    /// </summary>
    /// <param name="inboxHandlerRegistry"></param>
    /// <param name="serviceProvider"></param>
    public InboxOutboxDispatcher(InboxHandlerRegistry inboxHandlerRegistry,
            IServiceProvider serviceProvider,
            DefaultInboxStorageAccessor inboxStorageAccessor)
    {
        _inboxHandlerRegistry = inboxHandlerRegistry;
        _serviceProvider = serviceProvider;
        _inboxStorageAccessor = inboxStorageAccessor;
    }

    public async Task ProcessAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        // Obtenemos los handlers para los eventos entrantes
        var handlers = _inboxHandlerRegistry.Resolve(message.Event.GetType());
        // Creamos el contexto virtual para compartirlo con los handlers
        var context = new DefaultContext(
            message.CorrelationId,
            message.TraceId,
            new DefaultIdentityContext(
                message.UserId,
                "Unknow",
                message.Username
                )
            );
        // Convertimos los handlers en registro de inbox en registro de procesamiento
        var inboxMessages = handlers.Select(x => new InboxMessage
        {
            Id = Guid.NewGuid(),
            EventId = message.Id,
            Event = message.Event.GetType(),
            CorrelationId = message.CorrelationId,
            Handler = x
        }).ToList();
        // Mandamos a guardar los registros de reaccion
        await _inboxStorageAccessor.SaveAll(inboxMessages);
        // Los mandamos a la cola para agregarlos

        // Creamos el tipo generico para el builder de reacciones
        var inboxHandlerBuilderOpenType = typeof(InboxHandlerBuilder<,>);
        // Recorremos las reacciones para procesarlas
        foreach (var inboxMsg in inboxMessages)
        {
            // Creamos el wrapper para la reaccion
            var inboxHandlerBuilderType = inboxHandlerBuilderOpenType.MakeGenericType(inboxMsg.Event, inboxMsg.Handler);
            try
            {
                // Creamos la instancia
                var InboxHandlerBuilder = (InboxHandlerBuilderBase)Activator.CreateInstance(inboxHandlerBuilderType) ?? throw new ArgumentNullException($"Could not create wrapper for type {inboxHandlerBuilderType.Name}");
                // Ejecutamos el handler con toda la informacion necesaria
                await InboxHandlerBuilder.Handle(message.Event, inboxMsg, cancellationToken, _serviceProvider, context);
            }
            catch (Exception ex)
            {
                // Indicamos que no se proceso completamente el mensaje
                continue;
            }
        }
    }
}
