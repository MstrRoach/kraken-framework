using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Complements.Reaction
{
    /// <summary>
    /// Fabrica para crear el componente de reaccion stream
    /// encargado de centralizar el control de las reacciones 
    /// para los eventos
    /// </summary>
    public interface IReactionStreamFactory
    {
        /// <summary>
        /// Crea el componente reaction stream que le pertenece
        /// al tipo solicitado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IReactionStream CreateReactionStream<T>(IServiceProvider? serviceProvider = null);

        /// <summary>
        /// Crea el componente de reaction stream asociado
        /// al tipo y al modulo del elmento pasado por parametro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        IReactionStream CreateReactionStream(Type item, IServiceProvider? serviceProvider = null);
    }
}
