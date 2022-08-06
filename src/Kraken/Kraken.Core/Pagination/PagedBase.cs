using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Pagination;

/// <summary>
/// Clase base para indicar detalles de paginacion que interesen a los
/// usuarios del api. 
/// </summary>
public abstract class PagedBase
{
    /// <summary>
    /// Indica cual es la pagina actual
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Indica los resultados por pagina
    /// </summary>
    public int ResultsPerPage { get; set; }

    /// <summary>
    /// Indica la cantidad de paginas totales
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indica el total de resultados
    /// </summary>
    public long TotalResults { get; set; }

    /// <summary>
    /// Constructor vacio del paginado
    /// </summary>
    protected PagedBase()
    {
    }

    /// <summary>
    /// Constructor con todos los valores especificados para la paginacion
    /// </summary>
    /// <param name="currentPage"></param>
    /// <param name="resultsPerPage"></param>
    /// <param name="totalPages"></param>
    /// <param name="totalResults"></param>
    protected PagedBase(int currentPage, int resultsPerPage,
        int totalPages, long totalResults)
    {
        CurrentPage = currentPage;
        ResultsPerPage = resultsPerPage;
        TotalPages = totalPages;
        TotalResults = totalResults;
    }
}
