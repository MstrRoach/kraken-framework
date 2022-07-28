using Humanizer;
using Kraken.Core.Processing;
using Kraken.Host.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Reaction;

/// <summary>
/// Proceso encargado de la
/// </summary>
public class ReactionRedispatcher : InfinityProcessingServer
{
   
    /// <summary>
    /// Indica la cantidad de tiempo que espera entre procesamiento
    /// </summary>
    public int ProcessDelay => 60;

    /// <summary>
    /// Constructor del servicio infinito
    /// </summary>
    /// <param name="logger"></param>
    public ReactionRedispatcher(ILogger<ReactionRedispatcher> logger)
        : base(logger, typeof(ReactionRedispatcher).Name, TimeSpan.FromSeconds(2))
    {

    }

    /// <summary>
    /// Proceso que estara protegido para reiniciarse en cuanto falle
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public override async Task ProcessAsync()
    {
        _logger.LogInformation("[ReactionRedispatcher] Starting reaction redispatch... ");
        // Ejecutamos la logica que se repetira infinitamente


        // Esperamos
        await Task.Delay(TimeSpan.FromSeconds(ProcessDelay));
    }

}

