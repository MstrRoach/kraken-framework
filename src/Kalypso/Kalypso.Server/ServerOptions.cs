using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server;

/// <summary>
/// Opciones para la instancia
/// </summary>
public record ServerOptions
{
    public string Name { get; set; } = "Kalypso";
    public string Instance { get; set; } = "Default";
    public string Version { get; } = "v8.0.0";
}
