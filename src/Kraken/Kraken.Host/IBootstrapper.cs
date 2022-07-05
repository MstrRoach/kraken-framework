using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host;

/// <summary>
/// Permite inicializar y contener las operaciones en segundo plano
/// </summary>
public interface IBootstrapper
{
    /// <summary>
    /// Inicia el procesamiento
    /// </summary>
    /// <returns></returns>
    Task RunAsync();
}
