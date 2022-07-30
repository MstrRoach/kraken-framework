using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.Transaction
{
    /// <summary>
    /// Define la unidad de trabajo para las
    /// operaciones de transaccionalidad
    /// </summary>
    public interface IUnitWork
    {
        /// <summary>
        /// Ejecuta la operacion con la transaccionalidad administrada
        /// por la unidad de trabajo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        Task ExecuteAsync(Func<Task> action);

        /// <summary>
        /// Ejecuta una operacion con respuesta, administrada
        /// por la unidad de trabajo y envuelva en los procesos
        /// de transaccionalidad
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}
