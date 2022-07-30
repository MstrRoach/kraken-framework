using Kraken.Core.Internal;
using Kraken.Core.Internal.Mediator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Internal;

internal class DefaultHost : IAppHost
{

    /// <summary>
    /// Contiene todos los servicios disponibles en el modulo
    /// </summary>
    internal IServiceProvider _serviceProvider { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="commandDispatcher"></param>
    public DefaultHost(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Ejecuta un comando con una respuesta
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();
        return await mediator!.Send(command, cancellationToken);
    }

    /// <summary>
    /// Ejecuta un query y obtiene la respuesta del mismo
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResult> ReadAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>();
        return await mediator!.Send(query, cancellationToken);
    }
}
