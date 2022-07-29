using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Repository
{
    /// <summary>
    /// Interface definida para las operaciones comunes para los 
    /// repositorios,asi como la consulta por especificaciones
    /// </summary>
    public interface IRepository<T> where T : Aggregate
    {
    }
}
