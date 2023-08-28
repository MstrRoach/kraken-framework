using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Request.Pagination;

/// <summary>
/// Define las opciones de paginacion para un query
/// </summary>
public abstract class PagedQuery : IPagedQuery
{
    /// <summary>
    /// Indica la pagina que se quiere recuperar
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Indica la cantidad de resultados por pagina
    /// </summary>
    public int Results { get; set; } = 10;

    /// <summary>
    /// Devuelve la cantidad de registros que deben saltarse}
    /// para hacer la paginacion correcta
    /// </summary>
    public int Skipped => (Page - 1) * Results;
}

/// <summary>
/// Clase abstracta especifica para la construccion de un elemento paginado
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PagedQuery<T> : PagedQuery, IPagedQuery<Paged<T>>
{
}
