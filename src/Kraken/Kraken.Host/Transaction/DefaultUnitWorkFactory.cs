using Kraken.Core.Transaction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Transaction
{
    /// <summary>
    /// Fabrica de uniddad de trabajo por defecto.
    /// </summary>
    internal class DefaultUnitWorkFactory : IUnitWorkFactory
    {
        /// <summary>
        /// Proveedor de los servicios de ambito
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        public DefaultUnitWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Devuelve una unidad de trabajo nula, pues la implementacion
        /// por defecto no lleva la transaccionalidad implementada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IUnitWork? CreateUnitWork<T>()
            => _serviceProvider
            .CreateScope()
            .ServiceProvider
            .GetService<IUnitWork>();
    }
}
