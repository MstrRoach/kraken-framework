using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Transaction;

public interface IUnitWorkFactory
{
    /// <summary>
    /// Crea una unidad de trabajo para el modulo al
    /// cual pertenece el tipo pasado por parametro
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IUnitWork CreateUnitWork<T>();
}
