using Dottex.Kalypso.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Storage;

public class GenericSpecification<T> : ISpecification<T> where T : IAggregate
{
    /// <summary>
    /// Propiedad que contiene el criterio para la construccion 
    /// del filtro
    /// </summary>
    private Expression<Func<T, bool>> Criteria;

    /// <summary>
    /// Creador de la especificacion generica
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public static GenericSpecification<T> Create(Expression<Func<T, bool>> criteria)
        => new GenericSpecification<T> { Criteria = criteria };

    /// <summary>
    /// Aplicacion del filtro
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool IsSatisfied(T entity)
        => Criteria.Compile().Invoke(entity);
}
