using MediatR;

namespace Kraken.Core.Internal.Mediator;

//Marker
public interface ICommand<out TResult> : IRequest<TResult>
{
}