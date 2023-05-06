using Kraken.Standard.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Features.Contexts;

/// <summary>
/// Contructor del contexto de identidad provisto por el cliente
/// </summary>
public interface IIdentityContextBuilder
{
    /// <summary>
    /// Construye el contexto de identidad a partir de las
    /// claims pasadas por parametro
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    IIdentityContext Build(ClaimsPrincipal principal);

    /// <summary>
    /// Devuelve un contexto vacio per instanciable
    /// </summary>
    /// <returns></returns>
    IIdentityContext Empty();
}
