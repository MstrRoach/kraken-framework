using Dottex.Kalypso.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Storage;

public abstract class Specification<T> : ISpecification<T>
    where T : IAggregate
{

    /// <summary>
    /// Propiedad que contiene el criterio para la construccion 
    /// del filtro
    /// </summary>
    public Expression<Func<T, bool>> Criteria { get; protected set; }

    /// <summary>
    /// Funcion que se llama para filtrar los datos desde el exterior
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool IsSatisfied(T entity)
        => Criteria.Compile().Invoke(entity);
}
