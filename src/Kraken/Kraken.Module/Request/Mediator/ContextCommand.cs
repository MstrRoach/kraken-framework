using Kraken.Standard.Context;
using Kraken.Standard.Request;
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
public abstract class ContextCommand<TResponse> : ICommand<TResponse> 
{
    /// <summary>
    /// Contiene el contexto que acompaña al comando con informacion
    /// relevante.
    /// </summary>
    public IContext Context { get; set; }

}
