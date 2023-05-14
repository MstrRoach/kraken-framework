using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Kraken.Module.Request.Mediator;

/// <summary>
/// Interface que define el administrador de los comandos
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{

}