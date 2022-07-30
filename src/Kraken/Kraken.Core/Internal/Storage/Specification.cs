using Kraken.Core.Internal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Internal.Storage
{

    /// <summary>
    /// Define el comportamiento por defecto para la creacion de una 
    /// especificacion para repositorios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Specification<T> : ISpecification<T>
        where T : IAggregate
    {
        /// <summary>
        /// Forza a escribir el criterio en el constructor
        /// </summary>
        /// <param name="criteria"></param>
        public Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Filtro que se aplicara
        /// </summary>
        public Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// Metodo encargado de el filtrado
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = Criteria.Compile();
            return predicate(entity);
        }

    }
}
