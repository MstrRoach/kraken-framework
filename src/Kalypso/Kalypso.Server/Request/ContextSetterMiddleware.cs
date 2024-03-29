﻿using Dottex.Kalypso.Module.Context;
using Dottex.Kalypso.Module.Request.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Request;

internal sealed class ContextSetterMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : ContextCommand<TResponse>
{
    private readonly IContext _context;
    public ContextSetterMiddleware(IContext context)
    {
        _context = context;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        request.Context = _context;
        return await next();
    }
}
