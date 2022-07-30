using Kraken.Core.Internal.Mediator;
using System.Threading;
using System.Threading.Tasks;

namespace Kraken.Core.Internal;

public interface IKrakenHostt
{
    //Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand;
    //Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent;
    //Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Punto de entrada para la ejecucion de todas las operaciones con kraken
/// </summary>
public interface IKrakenHost
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