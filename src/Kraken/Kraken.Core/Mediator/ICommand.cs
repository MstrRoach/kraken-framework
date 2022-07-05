using MediatR;

namespace Kraken.Core.Commands;

//Marker
public interface ICommand<out TResult> : IRequest<TResult>
{
}