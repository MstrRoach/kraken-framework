using Kraken.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction
{
    /// <summary>
    /// Contiene el registro de los almacenes declarados en
    /// cada modulo y guarda la relacion entre ellas
    /// </summary>
    internal class ReactionStorageRegistry
    {
        /// <summary>
        /// Relacion entre el modulo y su registro de ejecucion para
        /// almacenar la reaccion ejecutada
        /// </summary>
        private readonly Dictionary<string, Type> _moduleExecutionLogs = new();

        /// <summary>
        /// Registra en el diccionario una entrada que relaciona el registro de ejecucion 
        /// con el modulo al que pertenece
        /// </summary>
        /// <param name="executionLog"></param>
        public void Register(Type executionLog) => _moduleExecutionLogs[GetKey(executionLog)] = executionLog;

        /// <summary>
        /// Recibe una reaccion y obtiene el registro de ejecucion que le pertenece para ejecutarlosa
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reaction"></param>
        /// <returns></returns>
        public Type Resolve(Type reaction) 
            => _moduleExecutionLogs.TryGetValue(GetKey(reaction), out var type) ? type : null;

        /// <summary>
        /// Obtiene el nombre del modulo a partir del tipo especificado
        /// </summary>
        /// <param name="outbox"></param>
        /// <returns></returns>
        private static string GetKey(Type executionLog) => $"{executionLog.GetModuleName()}";


    }
}
