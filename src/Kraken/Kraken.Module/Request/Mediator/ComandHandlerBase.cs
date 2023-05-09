﻿using Kraken.Standard.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Request.Mediator;

/// <summary>
/// Clase base para todos los comandos que les pertime el acceso al 
/// contexto con informacion del usuario relevante
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class ComandHandlerBase<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    public IContext Context { get; set; }

    public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
}
