using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server;

/// <summary>
/// Opciones para la instancia
/// </summary>
public class ServerOptions
{
    public string Name { get; set; } = "Kraken";
    public string Instance { get; set; } = "Default";
    public string Version { get; set; } = "3.0.0";
}
