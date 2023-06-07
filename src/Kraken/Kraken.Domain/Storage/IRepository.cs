using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kraken.Domain.Core;

namespace Kraken.Domain.Storage;

/// <summary>
/// Define el repositorio para los agregados especificados para dominio
/// </summary>
/// <typeparam name="Aggregate"></typeparam>
public interface IRepository<Aggregate> where Aggregate : IAggregate
{
    /// <summary>
    /// Crea una entidad dentro del repositorio
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Create(Aggregate aggregate);

    /// <summary>
    /// Actualiza un registro en la base de datos
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Update(Aggregate aggregate);

    /// <summary>
    /// Elimina un registro de la base de datos
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public Task Delete(Aggregate aggregate);

    /// <summary>
    /// Busca un elemento que coincida con la espcificacion
    /// y lo devuelve
    /// </summary>
    /// <returns></returns>
    public Task<Aggregate> Get(ISpecification<Aggregate> specification);

    /// <summary>
    /// Busca los elemmentos que coincidan con la especificacion
    /// y devuelve una lista de ellos
    /// </summary>
    /// <returns></returns>
    public Task<List<Aggregate>> GetAll(ISpecification<Aggregate> specification);

    /// <summary>
    /// Indica si existe algun registro con la especificacion
    /// pasada por parametro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<bool> Exist(ISpecification<Aggregate> specification)
        => Task.FromResult(false);

    /// <summary>
    /// Realiza el conteo de los registros que coincidan con la
    /// especificacion pasada por parametro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<int> Count(ISpecification<Aggregate>? specification = null)
        => Task.FromResult(0);
}
