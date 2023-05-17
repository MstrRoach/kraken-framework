using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Domain;

/// <summary>
/// Interface definida para las operaciones comunes para los 
/// repositorios,asi como la consulta por especificaciones
/// </summary>
public interface IRepository<Aggregate, Specification>
        where Aggregate : IAggregate
        where Specification : class, ISpecification<Aggregate>
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
    public Task<Aggregate> Get(Specification specification);

    /// <summary>
    /// Busca los elemmentos que coincidan con la especificacion
    /// y devuelve una lista de ellos
    /// </summary>
    /// <returns></returns>
    public Task<List<Aggregate>> GetAll(Specification specification);

    /// <summary>
    /// Indica si existe algun registro con la especificacion
    /// pasada por parametro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<bool> Exist(Specification specification)
        => Task.FromResult(false);

    /// <summary>
    /// Realiza el conteo de los registros que coincidan con la
    /// especificacion pasada por parametro
    /// </summary>
    /// <param name="specification"></param>
    /// <returns></returns>
    public Task<int> Count(Specification? specification = null)
        => Task.FromResult(0);
}

