using Kraken.Standard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Operation.Request;

/// <summary>
/// Punto de entrada para la ejecucion de todas las operaciones con kraken
/// </summary>
public interface IAppHost
{
    /// <summary>
    /// Ejecuta los comandos y devuelve una respuesta
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ejecuta un query en el sistema y devuelve la respuesta esperada
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResult> ReadAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
