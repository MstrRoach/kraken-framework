using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Processing;

public interface IProcessingService : IDisposable
{
    /// <summary>
    /// Realiza una verificacion para revisar si el
    /// proceso esta vivo
    /// </summary>
    void HealthCheck() { }

    /// <summary>
    /// Inicia el procesamiento de la operacion
    /// </summary>
    /// <param name="stoppingToken"></param>
    void Start(CancellationToken stoppingToken);
}
