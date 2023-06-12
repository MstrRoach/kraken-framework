using System;

namespace Dottex.Kalypso.Module.Context;

/// <summary>
/// Define los datos de cada solicitud a traves 
/// del host
/// </summary>
public interface IContext
{
    /// <summary>
    /// Id de la solicitud http
    /// </summary>
    Guid RequestId { get; }

    /// <summary>
    /// Id de correlacion para procesos complejos
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Cadena utilizada como huella unica
    /// </summary>
    string TraceId { get; }

    /// <summary>
    /// Ip de la direccion desde donde se hizo la solicitud
    /// </summary>
    string IpAddress { get; }

    /// <summary>
    /// User agent desde donde se hizo la solicitud
    /// </summary>
    string UserAgent { get; }

    /// <summary>
    /// Contexto de identidad
    /// </summary>
    IIdentityContext Identity { get; }
}