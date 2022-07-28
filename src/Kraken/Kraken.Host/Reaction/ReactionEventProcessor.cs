using Kraken.Core.Outbox;
using Kraken.Core.Reaction;
using Kraken.Host.Contexts;
using Microsoft.Extensions.DependencyInjection;
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
        /// Decide y construye los artefactos para almacenar
        /// las reacciones que deben de ejecutarse y llevar un
        /// control de los mismos
        /// </summary>
        private readonly IReactionStreamFactory _reactionStreamFactory;

        /// <summary>
        /// Constructor del procesador de reacciones
        /// </summary>
        /// <param name="reactionRegistry"></param>
        public ReactionEventProcessor(ReactionRegistry reactionRegistry, 
            IServiceProvider serviceProvider, IReactionStreamFactory reactionStreamFactory)
        {
            _reactionRegistry = reactionRegistry;
            _serviceProvider = serviceProvider;
            _reactionStreamFactory = reactionStreamFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ProcessAsync(ProcessMessage message, CancellationToken cancellationToken = default)
        {
            // Obtenemos la lista de reacciones
            var reactions = _reactionRegistry.Resolve(message.Event.GetType());
            // Creamos el contexto virtual para compartirlo con las reacciones
            var identity = new IdentityContext(message.UserId, "System");
            var context = new Context(message.CorrelationId, message.TraceId, identity);
            // Convertimos las reacciones en registros de proceso
            var reactionRecords = reactions.Select(x => new ProcessRecord
            {
                Id = Guid.NewGuid(),
                EventId = message.Id,
                Event = message.Event.GetType(),
                CorrelationId = message.CorrelationId,
                Reaction = x
            }).ToList();
            // Guardamos todas las reacciones
            await this.SaveReactionRecords(reactionRecords);
            // Recorremos las reacciones
            foreach (var reaction in reactionRecords)
            {
                // Creammos el tipo generico
                var wrapperEventType = typeof(ReactionBuilder<,>).MakeGenericType(reaction.Event, reaction.Reaction);
                try
                {
                    // Creammos la instancia
                    var handler = (ReactionBuilderBase)Activator.CreateInstance(wrapperEventType)
                        ?? throw new InvalidOperationException($"Could not create wrapper for type {wrapperEventType}");
                    // Ejecutamos el handler. Dentro del handler si se ejecuto correctamente,
                    // este se debe de actualizar para indicar que se ejecuto de forma correcta
                    // dentro de la base de datos
                    await handler.Handle(message.Event, reaction, cancellationToken, _serviceProvider, context);
                }
                catch (Exception ex)
                {
                    // Indicamos que no se proceso completamente el mensaje
                    continue;
                }
            }
        }

        /// <summary>
        /// Permite realizar el guardado de los registros de proceso utilizando
        /// un alcance corto, que permita ahorrar recursos
        /// </summary>
        /// <param name="records"></param>
        private async Task SaveReactionRecords(List<ProcessRecord> records)
        {
            /*
             * El alcance en comun permitira que los servicios con alcance corto, 
             * puedan almacenarse por si se llaman a componentes que ya fueron
             * instanciados antes, esto, para ahorrar recursos y no utilizar alcances
             * por cada solicitud. Esto, permite que si existen dos registros para el
             * mismo modulo, cargaran el mismo streamm al estar en el mismo alcance
             */
            // Creamos el alcance en comun
            using var scope = _serviceProvider.CreateScope();
            // Recorremos las reacciones
            foreach (var record in records)
            {
                // Creamos el stream para el record
                var reactionStream = _reactionStreamFactory.CreateReactionStream(record.Reaction, scope.ServiceProvider);
                // Guardamos el registro
                await reactionStream.SaveAsync(record);
            }
        }

    }
}
