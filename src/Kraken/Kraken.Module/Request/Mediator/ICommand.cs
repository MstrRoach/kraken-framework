using MediatR;

namespace Kraken.Module.Request.Mediator;

//Marker
public interface ICommand<out TResult> : IRequest<TResult>
{
}