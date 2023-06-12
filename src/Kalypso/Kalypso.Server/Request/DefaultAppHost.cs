using Dottex.Kalypso.Module.Request.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Request;

internal sealed class DefaultAppHost : IAppHost
{
    /// <summary>
    /// Componente para acceder a los queries y comandos 
    /// de la applicacion
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor para el mediador
    /// </summary>
    /// <param name="mediator"></param>
    public DefaultAppHost(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Ejecuta un comando con una respuesta
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        => await _mediator.Send(command, cancellationToken);

    /// <summary>
    /// Ejecuta un query y obtiene la respuesta del mismo
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TResult> ReadAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        => await _mediator.Send(query, cancellationToken);
}
