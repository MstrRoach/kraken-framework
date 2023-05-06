using System;
using System.Collections.Generic;

namespace Kraken.Standard.Context;

public interface IIdentityContext
{
    /// <summary>
    /// Indica si el solicitante esta autenticado
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Id del solicitante
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Nombre del usuario actual
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Rol del solicitante
    /// </summary>
    string Role { get; }

    /// <summary>
    /// Lista de reclamos del solicitante
    /// </summary>
    Dictionary<string, IEnumerable<string>> Claims { get; }

    /// <summary>
    /// Intenta obtener el claim especificado
    /// </summary>
    /// <param name="claimName"></param>
    /// <returns></returns>
    string TryGetClaim(string claimName);
}