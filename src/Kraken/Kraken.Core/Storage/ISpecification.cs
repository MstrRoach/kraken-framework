using Kraken.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Storage;

/// <summary>
/// Define el componente para escribir filtros que
/// permitan obtener un registro basado en criterios
/// de tipo, sin fugas de dominio
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISpecification<T>
    where T : IAggregate
{
    /// <summary>
    /// Define un criterio para filtrar los registros
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Permite evaluar si un valor se satisface
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool IsSatisfiedBy(T entity);
}
