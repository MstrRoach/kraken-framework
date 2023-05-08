using Kraken.Server.Features.Correlation;
using Kraken.Standard.Context;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Features.Contexts;

/// <summary>
/// Contexto por defecto para el API
/// </summary>
public class DefaultContext : IContext
{
    /// <summary>
    /// Id de la solicitud
    /// </summary>
    public Guid RequestId { get; } = Guid.NewGuid();

    /// <summary>
    /// Id para relacionar dos solicitudes
    /// </summary>
    public Guid CorrelationId { get; }

    /// <summary>
    /// Cadena para el trazado de la informacion
    /// </summary>
    public string TraceId { get; }

    /// <summary>
    /// Direccion ip del cliente
    /// </summary>
    public string IpAddress { get; }

    /// <summary>
    /// Cliente que hace la solicitud
    /// </summary>
    public string UserAgent { get; }

    /// <summary>
    /// Informacion de la identidad
    /// </summary>
    public IIdentityContext Identity { get; }

    /// <summary>
    /// Construccion del contexto por defecto
    /// </summary>
    /// <param name="context"></param>
    /// <param name="identityBuilder"></param>
    public DefaultContext(HttpContext context, DefaultIdentityContextBuilder identityBuilder)
        : this(context.TryGetCorrelationId(), context.TraceIdentifier, 
              identityBuilder.Build(context.User) ?? identityBuilder.Empty(),
              context.GetUserIpAddress(), context.Request.Headers["user-agent"])
    { }

    /// <summary>
    /// Construccion explicita del contexto
    /// </summary>
    /// <param name="correlationId"></param>
    /// <param name="traceId"></param>
    /// <param name="identity"></param>
    /// <param name="ipAddress"></param>
    /// <param name="userAgent"></param>
    public DefaultContext(Guid? correlationId, string traceId, IIdentityContext identity, 
        string ipAddress = null, string userAgent = null)
    {
        CorrelationId = correlationId ?? Guid.NewGuid();
        TraceId = traceId;
        Identity = identity;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}
