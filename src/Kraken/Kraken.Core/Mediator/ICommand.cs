using MediatR;

namespace Kraken.Core.Mediator;

//Marker
public interface ICommand<out TResult> : IRequest<TResult>
{
}