using Kraken.Standard.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Middlewares.Contexts;

public sealed class DefaultIdentityContextBuilder
{
    /// <summary>
    /// Propieades para crear la identidad del usuario desde
    /// las claims
    /// </summary>
    private readonly IdentityContextProperties _properties;

    public DefaultIdentityContextBuilder(IdentityContextProperties properties)
    {
        _properties = properties;
    }

    /// <summary>
    /// Construye el contexto
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public IIdentityContext Build(ClaimsPrincipal principal)
    {
        if (principal is null)
            return null;
        return new DefaultIdentityContext(principal, _properties);
    }

    /// <summary>
    /// Constructor no inicializado
    /// </summary>
    /// <returns></returns>
    public IIdentityContext Empty() => new DefaultIdentityContext();
}
