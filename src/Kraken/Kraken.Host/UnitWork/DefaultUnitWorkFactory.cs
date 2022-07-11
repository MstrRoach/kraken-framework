using Kraken.Core.UnitWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.UnitWork
{
    internal class DefaultUnitWorkFactory : IUnitWorkFactory
    {
        /// <summary>
        /// Definicion de las unidades de trabajo asociadas
        /// a cada modulo
        /// </summary>
        private readonly UnitWorkRegistry _unitWorkRegistry;

        /// <summary>
        /// Proveedor de los servicios para obtener la unidad de 
        /// trabajo en el alcance
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        public DefaultUnitWorkFactory(UnitWorkRegistry unitWorkRegistry, 
            IServiceProvider serviceProvider)
        {
            _unitWorkRegistry = unitWorkRegistry;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Devuelve una unidad de trabajo para el tipo especificado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IUnitWork CreateUnitWork<T>()
        {
            // Obtenemos el tipo de unidad de trabajo
            var unitWorkType = _unitWorkRegistry.Resolve<T>();
            // Cargamos la unidad de trabajo desde el proveedor de servicio y la devolvemos
            return _serviceProvider.GetRequiredService(unitWorkType) as IUnitWork;
        }
    }
}
