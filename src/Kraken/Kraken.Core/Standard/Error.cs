using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Standard
{
    /// <summary>
    /// Define una estructura standar para envolver un error
    /// en la capa de aplicacion
    /// </summary>
    public record Error
    {
        /// <summary>
        /// Codigo del error, communmente es el nommbre del error
        /// </summary>
        public string errorCode { get; private set; }

        /// <summary>
        /// MMMensaje del error con mas informmacion acerca del mmismmo
        /// </summary>
        public string errorMessage { get; private set; }

        /// <summary>
        /// Constructor del objeto de error para
        /// detallar los errores
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        public Error(string errorCode, string errorMessage)
        {
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
        }

    }
}
