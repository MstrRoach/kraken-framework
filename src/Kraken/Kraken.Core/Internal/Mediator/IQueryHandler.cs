﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.Mediator
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
       where TQuery : IQuery<TResult>
    {
    }
}
