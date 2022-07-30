using Kraken.Core.Features.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Features.Documentation
{
    /// <summary>
    /// Registra la configuracion de la documentacion desde las
    /// opciones de kraken
    /// </summary>
    public static class DocumentationExtensions
    {
        /// <summary>
        /// Registra la configuracion de la documentacion desde las opciones de
        /// kraken
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static AppDescriptor AddDocumentation(this AppDescriptor krakenOptions, 
            Action<DocumentationOptions>? setup = null)
        {
            if (krakenOptions.Documentation is not null)
                throw new InvalidOperationException("Documentation already been configured. See your kraken host builder");
            // Creamos las configuraciones por defecto
            var options = new DocumentationOptions();
            // Si hay configuracion la ejecutammos
            if (setup is not null) setup(options);
            // Setteammos la configuracion en las opciones de kraken
            krakenOptions.Documentation = new DocumentationFeature(options);
            // Devolvemos las opciones de kraken
            return krakenOptions;
        }
    }
}
