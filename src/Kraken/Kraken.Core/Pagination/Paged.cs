using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Pagination;

/// <summary>
/// Indica la envoltura para una serie de resultados 
/// que utilizan la paginacion por servidor
/// </summary>
/// <typeparam name="T"></typeparam>
public class Paged<T> : PagedBase
{
    /// <summary>
    /// Lista de solo lectura de los elementos
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Indica si la lista esta vacia
    /// </summary>
    public bool Empty => Items is null || !Items.Any();

    /// <summary>
    /// Constructor por defecto de la paginacion
    /// </summary>
    public Paged()
    {
        CurrentPage = 1;
        TotalPages = 1;
        ResultsPerPage = 10;
    }

    /// <summary>
    /// Constructor para la construccion de un elemento paginado con los detalles
    /// de la paginacion integrados
    /// </summary>
    /// <param name="items"></param>
    /// <param name="currentPage"></param>
    /// <param name="resultsPerPage"></param>
    /// <param name="totalPages"></param>
    /// <param name="totalResults"></param>
    public Paged(IReadOnlyList<T> items,
        int currentPage, int resultsPerPage,
        int totalPages, long totalResults) :
        base(currentPage, resultsPerPage, totalPages, totalResults)
    {
        Items = items;
    }

    /// <summary>
    /// Crea un resultado paginado desde la construccion statica
    /// </summary>
    /// <param name="items"></param>
    /// <param name="currentPage"></param>
    /// <param name="resultsPerPage"></param>
    /// <param name="totalPages"></param>
    /// <param name="totalResults"></param>
    /// <returns></returns>
    public static Paged<T> Create(IReadOnlyList<T> items,
        int currentPage, int resultsPerPage,
        int totalPages, long totalResults)
        => new(items, currentPage, resultsPerPage, totalPages, totalResults);

    /// <summary>
    /// Construye un resultado paginado utilizando un mapeo de resultados
    /// </summary>
    /// <param name="result"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static Paged<T> From(PagedBase result, IReadOnlyList<T> items)
        => new(items, result.CurrentPage, result.ResultsPerPage,
            result.TotalPages, result.TotalResults);

    /// <summary>
    /// Devuelve unn resultado paginado vacio
    /// </summary>
    public static Paged<T> AsEmpty => new();

    /// <summary>
    /// Mapea un resultado a partir de una funcion con los items
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="map"></param>
    /// <returns></returns>
    public Paged<TResult> Map<TResult>(Func<T, TResult> map)
        => Paged<TResult>.From(this, Items.Select(map).ToList());
}
