using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Features.Cors
{
    /// <summary>
    /// Optiones configurables para las cors en el sistema, que permiten definir el comportamiento
    /// de los cors. Una instancia no configurada es restrictiva
    /// </summary>
    public sealed class CorsOptions
    {
        /// <summary>
        /// Nombre de la configuracion para utilizar en los 
        /// app settings
        /// </summary>
        public static string Name = "CorsOptions";

        /// <summary>
        /// Permite indicar si se aceptan las credenciales
        /// </summary>
        public bool allowCredentials { get; set; } = false;

        /// <summary>
        /// Lista de origenes permitidos
        /// </summary>
        public IEnumerable<string> allowedOrigins { get; set; } = new List<string>();

        /// <summary>
        /// Lista de metodos permitidos
        /// </summary>
        public IEnumerable<string> allowedMethods { get; set; } = new List<string>();

        /// <summary>
        /// Lista de Headers permitidos
        /// </summary>
        public IEnumerable<string> allowedHeaders { get; set; } = new List<string>();

        /// <summary>
        /// Lista de headers expuestos
        /// </summary>
        public IEnumerable<string> exposedHeaders { get; set; } = new List<string>();
    }
}
