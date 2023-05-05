using Kraken.Module.Request.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Request.Pagination;

/// <summary>
/// Interface para indicar que un query tiene que ser paginado
/// </summary>
public interface IPagedQuery
{
    /// <summary>
    /// Indica la pagina que se quiere recuperar
    /// </summary>
    int Page { get; set; }

    /// <summary>
    /// Indica la cantidad de resultados a devolver
    /// </summary>
    int Results { get; set; }
}

/// <summary>
/// Interface para especializar a un query que permita agregar
/// ciertas caracteristicas extras para la paginacion
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPagedQuery<T> : IPagedQuery, IQuery<T>
{
}
