using MediatR;

namespace Kraken.Standard.Request;

//Marker
public interface ICommand<out TResult> : IRequest<TResult>
{
}