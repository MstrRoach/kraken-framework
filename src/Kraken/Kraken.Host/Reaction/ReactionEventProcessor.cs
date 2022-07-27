using Kraken.Core.Outbox;
using Kraken.Host.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    /// <summary>
    /// Procesador de eventos utilizando el enfoque de reacciones de eventos, en 
    /// lugar de administradores de notificaciones, para envolver las reacciones 
    /// con los middlewares de transaccionalidad y de logeo que ya existen en 
    /// los commandos. 
    /// El componente estara en alcance singlenton, lo que requiere el uso de 
    /// estrategias para construir uno a uno las reacciones de cada evento que 
    /// se pase por parametro, esto con el proposito de ejecutar y llevar el control 
    /// de aquellas reacciones que han sido ejecutadas y de sabes cuales han sido ejecutadas por error
    /// </summary>
    internal class ReactionEventProcessor : IEventProcessor
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
        /// Constructor del procesador de reacciones
        /// </summary>
        /// <param name="reactionRegistry"></param>
        public ReactionEventProcessor(ReactionRegistry reactionRegistry, 
            IServiceProvider serviceProvider)
        {
            _reactionRegistry = reactionRegistry;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ProcessAsync(ProcessMessage message, CancellationToken cancellationToken = default)
        {
            // Obtenemos la lista de reacciones
            var reactions = _reactionRegistry.Resolve(message.Event.GetType());
            // Creamos el contexto virtual para compartirlo con las reacciones
            var identity = new IdentityContext(message.UserId, "System");
            var context = new Context(message.CorrelationId, message.TraceId, identity);
            // Recorremos las reacciones
            foreach (var reaction in reactions)
            {
                // Creammos el tipo generico
                var wrapperEventType = typeof(ReactionBuilder<,>).MakeGenericType(message.Event.GetType(), reaction);
                // Creammos la instancia
                var handler = (ReactionBuilderBase)Activator.CreateInstance(wrapperEventType) 
                    ?? throw new InvalidOperationException($"Could not create wrapper for type {wrapperEventType}");
                // Ejecutamos el handler
                await handler.Handle(message.Event, cancellationToken, _serviceProvider, context);
            }
            
            // Creamos el wrapper con los parametros de evento y reaccion
        }
    }
}
