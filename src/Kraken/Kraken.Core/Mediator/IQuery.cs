using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Mediator;

/// <summary>
/// Interface para todas aquellas peticiones que solo son de lectura y no provocan cambios internos
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IQuery<out TResult> : IRequest<TResult>
{
}
