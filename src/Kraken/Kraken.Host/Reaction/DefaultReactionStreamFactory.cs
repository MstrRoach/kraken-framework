using Humanizer;
using Kraken.Core.Reaction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    internal class DefaultReactionStreamFactory : IReactionStreamFactory
    {
        /// <summary>
        /// Logger para el commponente
        /// </summary>
        private readonly ILogger<DefaultReactionStreamFactory> _logger;

        /// <summary>
        /// Mapeo de los almacenes con los modulos a los que pertenecen
        /// </summary>
        private readonly ReactionStorageRegistry _reactionStorageRegistry;

        /// <summary>
        /// Proveedor de los servicios para obtener los registros de reaction
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor del modulo
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="reactionStorageRegistry"></param>
        /// <param name="serviceProvider"></param>
        public DefaultReactionStreamFactory(ILogger<DefaultReactionStreamFactory> logger,
            ReactionStorageRegistry reactionStorageRegistry,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _reactionStorageRegistry = reactionStorageRegistry;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Devuelve el stream perteneciente al modulo del cual se deriva el
        /// tipo pasado por parametro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IReactionStream CreateReactionStream<T>(IServiceProvider? serviceProvider = null)
            => CreateReactionStream(typeof(T), serviceProvider);

        /// <summary>
        /// Devuelve el stream asociado al tipo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IReactionStream CreateReactionStream(Type item, IServiceProvider? serviceProvider = null)
        {
            // Setteamos el origen del proveedor de servicios
            var localServiceProvider = serviceProvider ?? _serviceProvider;
            // Logeamos un poquito
            _logger.LogInformation("[Reaction Broker] Processin reaction {reaction}, from module {module}",
                item.Name.Underscore(),
                item.GetModuleName());
            // Obtenemos el almacen para realizar
            var reactionStorage = _reactionStorageRegistry.Resolve(item);
            // Creamos el tipo cerrado
            var reactionLogType = typeof(DefaultReactionStream<>).MakeGenericType(reactionStorage);
            // Obtenemos el servicio del proveedor de servicios
            var reactionLog = localServiceProvider.GetService(reactionLogType) as IReactionStream;
            // Si es nulo el servicio entonces no hay nada registrado
            if (reactionLog is null)
                throw new InvalidOperationException($"Reaction Log is not registered for module: {item.GetModuleName()}");
            // Guardamos el registro
            return reactionLog;
        }

        ///// <summary>
        ///// Se encarga de resolver y ejecutar el registro de reaccion para la reaccion actual
        ///// </summary>
        ///// <param name="record"></param>
        ///// <returns></returns>
        ///// <exception cref="InvalidOperationException"></exception>
        //public async Task SaveAsync(ProcessRecord record)
        //{
        //    _logger.LogInformation("[Reaction Broker] Processin reaction {reaction}, from module {module}",
        //        record.Reaction.Name.Underscore(),
        //        record.Reaction.GetModuleName());
        //    // Obtenemmos el tipo del almacen para la reaccion
        //    var reactionStorage = _reactionStorageRegistry.Resolve(record.Reaction);
        //    // Creamos el tipo cerrado para el log
        //    var reactionLogType = typeof(DefaultReactionStream<>).MakeGenericType(reactionStorage);
        //    //// Obtenemos el servicio del proveedor de servicios
        //    //var reactionLog = _serviceProvider.GetService(reactionLogType) as IReactionLog;
        //    //// Si es nulo el servicio entonces no hay nada registrado
        //    //if (reactionLog is null)
        //    //    throw new InvalidOperationException($"Reaction Log is not registered for module: {record.Reaction.GetModuleName()}");
        //    //// Guardamos el registro
        //    //await reactionLog.SaveAsync(record);
        //}
    }
}
