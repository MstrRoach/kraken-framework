using Kraken.Module.Context;
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
    /// Construye el contexto de identidad a partir de estos tres 
    /// campos
    /// </summary>
    /// <param name="id"></param>
    /// <param name="role"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public IIdentityContext Build(string id, string role, string name)
        => new DefaultIdentityContext(id, role, name);

    /// <summary>
    /// Constructor no inicializado
    /// </summary>
    /// <returns></returns>
    public IIdentityContext Empty() => new DefaultIdentityContext();
}
