using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Outbox;

public interface IOutboxFactory
{
    /// <summary>
    /// Creaa la bandeja de entrada que corresponde al tipo
    /// pasado por el parametro generico
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IOutbox Create<T>();
}
