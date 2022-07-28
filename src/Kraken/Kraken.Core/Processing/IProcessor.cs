using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Processing
{
    /// <summary>
    /// Define la abtrraccion para un proceso en segundo plano
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ProcessAsync(ProcessingContext context);
    }
}
