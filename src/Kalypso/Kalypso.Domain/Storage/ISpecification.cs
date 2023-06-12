using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dottex.Kalypso.Domain.Core;

namespace Dottex.Kalypso.Domain.Storage;

/// <summary>
/// Define el componente para escribir filtros que permitan
/// limmitar las busquedas
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISpecification<T> where T : IAggregate
{
    bool IsSatisfied(T entity);
}
