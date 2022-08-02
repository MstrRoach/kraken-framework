using Kraken.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Storage
{
    /// <summary>
    /// Interface definida para las operaciones comunes para los 
    /// repositorios,asi como la consulta por especificaciones
    /// </summary>
    public interface IRepository<T> where T : IAggregate
    {
        /// <summary>
        /// Crea una entidad dentro del repositorio
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public Task Create(T aggregate);

        /// <summary>
        /// Actualiza un registro en la base de datos
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public Task Update(T aggregate);

        /// <summary>
        /// Elimina un registro de la base de datos
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public Task Delete(T aggregate);

        /// <summary>
        /// Busca un elemento que coincida con la espcificacion
        /// y lo devuelve
        /// </summary>
        /// <returns></returns>
        public Task<T> Get(ISpecification<T> specification);

        /// <summary>
        /// Busca los elemmentos que coincidan con la especificacion
        /// y devuelve una lista de ellos
        /// </summary>
        /// <returns></returns>
        public Task<List<T>> GetAll(ISpecification<T> specification);

        /// <summary>
        /// Indica si existe algun registro con la especificacion
        /// pasada por parametro
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public Task<bool> Exist(ISpecification<T> specification)
            => Task.FromResult(false);

        /// <summary>
        /// Realiza el conteo de los registros que coincidan con la
        /// especificacion pasada por parametro
        /// </summary>
        /// <param name="specification"></param>
        /// <returns></returns>
        public Task<int> Count(ISpecification<T>? specification = null)
            => Task.FromResult(0);
    }
}
