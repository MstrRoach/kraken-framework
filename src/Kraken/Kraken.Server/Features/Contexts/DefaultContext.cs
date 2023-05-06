using Kraken.Standard.Context;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public DefaultContext(Guid? correlationId, string traceId, 
        IIdentityContext identity = null, string ipAddress = null, string userAgent = null)
    {
        CorrelationId = correlationId ?? Guid.NewGuid();
        TraceId = traceId;
        Identity = identity ?? IIdentityContext.Empty;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
}
