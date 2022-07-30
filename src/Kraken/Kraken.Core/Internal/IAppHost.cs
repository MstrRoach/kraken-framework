using Kraken.Core.Internal.Mediator;
using System.Threading;
using System.Threading.Tasks;

namespace Kraken.Core.Internal;

// <summary>
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
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ejecuta un query en el sistema y devuelve la respuesta esperada
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResult> ReadAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}