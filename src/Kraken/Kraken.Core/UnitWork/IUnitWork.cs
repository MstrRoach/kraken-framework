using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.UnitWork
{
    /// <summary>
    /// Define la unidad de trabajo para las
    /// operaciones de transaccionalidad
    /// </summary>
    public interface IUnitWork
    {

        /// <summary>
        /// Inicia una transaccion
        /// </summary>
        Task StartTransaction();

        /// <summary>
        /// Confirma la transaccion
        /// </summary>
        Task Commit();

        /// <summary>
        /// Cancela la transaccion y revierte los cambios
        /// </summary>
        Task Rollback();
    }
}
