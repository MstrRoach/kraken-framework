using System;
using System.Collections.Generic;

namespace Inflow.Shared.Abstractions.Contexts;

public interface IIdentityContext
{
    /// <summary>
    /// Indica si el solicitante esta autenticado
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Id del solicitante
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Rol del solicitante
    /// </summary>
    string Role { get; }

    /// <summary>
    /// Lista de reclamos del solicitante
    /// </summary>
    Dictionary<string, IEnumerable<string>> Claims { get; }

    /// <summary>
    /// Indica si es usuario
    /// </summary>
    /// <returns></returns>
    bool IsUser();

    /// <summary>
    /// Indica si es administrador
    /// </summary>
    /// <returns></returns>
    bool IsAdmin();
}