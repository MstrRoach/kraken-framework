using MediatR;

namespace Dottex.Kalypso.Module.Request.Mediator;

//Marker
public interface ICommand<out TResult> : IRequest<TResult>
{
}