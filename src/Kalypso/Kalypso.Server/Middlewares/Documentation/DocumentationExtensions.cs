using Dottex.Kalypso.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Documentation
{
    /// <summary>
    /// Registra la configuracion de la documentacion desde las
    /// opciones de kalypso
    /// </summary>
    public static class DocumentationExtensions
    {
        /// <summary>
        /// Registra la configuracion de la documentacion desde las opciones de
        /// kalypso
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ServerDescriptor AddDocumentation(this ServerDescriptor kalypsoOptions,
            Action<DocumentationOptions>? setup = null)
        {
            if (kalypsoOptions.Documentation is not null)
                throw new InvalidOperationException("Documentation already been configured. See your kalypso host builder");
            // Creamos las configuraciones por defecto
            var options = new DocumentationOptions();
            // Si hay configuracion la ejecutammos
            if (setup is not null) setup(options);
            // Setteammos la configuracion en las opciones de kalypso
            kalypsoOptions.Documentation = new DocumentationFeature(options);
            // Devolvemos las opciones de kalypso
            return kalypsoOptions;
        }
    }
}
