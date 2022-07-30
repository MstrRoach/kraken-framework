using Kraken.Core.Features.Cors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Features.Cors;

public static class CorsExtensions
{
    /// <summary>
    /// Agrega la politica de cors al descriptor de la aplicacion
    /// </summary>
    /// <param name="krakenOptions"></param>
    /// <param name="setup"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static AppDescriptor AddCorsPolicy(this AppDescriptor krakenOptions, 
        Action<CorsOptions> setup)
    {
        if (krakenOptions.Cors is not null)
            throw new InvalidOperationException("Cors already been configured. See your kraken host builder");
        // Si las opciones de cors son nulas lanzamos excepcion
        if (setup is null)
            throw new ArgumentNullException(nameof(setup));
        // Creamos las configuraciones por defecto
        var options = new CorsOptions();
        // Ejecutamos las configuraciones
        setup(options);
        // Setteammos la configuracion en las opciones de kraken
        krakenOptions.Cors = new CorsFeature(options);
        // Devolvemos las opciones de kraken
        return krakenOptions;
    }
}
