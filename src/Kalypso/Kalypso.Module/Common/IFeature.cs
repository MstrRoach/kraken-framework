using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Common
{
    /// <summary>
    /// Interface para definir los servicios que se
    /// configuran dentro del host
    /// </summary>
    [Obsolete]
    public interface IFeature
    {
        /// <summary>
        /// Registra servicios hijos para terceros
        /// </summary>
        /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
        void AddServices(IServiceCollection services);

        /// <summary>
        /// Agrega los servicios a la canalizacion de la solicitud
        /// </summary>
        /// <param name="app"></param>
        void UseServices(IApplicationBuilder app);
    }
}
