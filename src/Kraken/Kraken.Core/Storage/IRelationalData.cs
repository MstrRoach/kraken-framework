using Kraken.Core.Pagination;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Storage
{
    /// <summary>
    /// Define el acceso facil y rapido hacia los modelos de lectura,
    /// todas las lecturas se aseguran para permitir que estas sean rapidas,
    /// que no afecten ni escriban nada y que ademas puedan ser libres
    /// Es necesario que se declare el servicio para el compilador para
    /// el modulo
    /// </summary>
    public interface IRelationalData<M> 
        where M : IModule
    {
        /// <summary>
        /// Devuelve el primer elemento o el valor por
        /// defecto para un tipo que coincida con el query
        /// especificado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> FirstOrDefault<T>(Query query);

        /// <summary>
        /// Devuelve una lista de los elementos que coinciden
        /// con el query especificado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Select<T>(Query query);

        /// <summary>
        /// Se encarga de ejecutar una consulta y paginarla segun la informacion
        /// de paginacion pasada por parametro
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pagedQuery"></param>
        /// <returns>Una lista de registros envuelta en informacion de paginacion</returns>
        Task<Paged<T>> Paginate<T>(Query query, PagedQuery pagedQuery);
    }
}
