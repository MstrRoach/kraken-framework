using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Core;

/// <summary>
/// Interface para marcar una entidad como auditable
/// habilitada para registrar los cambios internos
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Representa el estado del objeto como
    /// un registro plano de tipo jsonObject
    /// </summary>
    JsonObject State { get; }
}
