using Kraken.Core.Mediator.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core
{
    public static class KrakenCoreExtensions
    {
        /// <summary>
        /// Obtiene el nombre del modulo al que pertenece el comando
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetModuleName(this object value)
            => value?.GetType().GetModuleName() ?? string.Empty;


        /// <summary>
        /// Obtiene el nombre del modulo revisando el nombre del namespace del comando
        /// </summary>
        /// <param name="type"></param>
        /// <param name="namespacePart"></param>
        /// <param name="splitIndex"></param>
        /// <returns></returns>
        public static string GetModuleName(this Type type, int splitIndex = 0)
        {
            if (type?.Namespace is null)
                return string.Empty;
            // Si el tipo es intancia de module event
            if(typeof(IModuleEvent).IsAssignableFrom(type))
                splitIndex = 1;

            return type.Namespace.Split(".")[splitIndex].ToLowerInvariant();
        }
    }
}
