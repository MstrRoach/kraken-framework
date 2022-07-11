using Kraken.Core.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.UnitWork
{
    public abstract class AbstractUnitWork : IUnitWork
    {
        

        /// <summary>
        /// Inicia la transaccion y administra los 
        /// eventos que deben de lanzarse
        /// </summary>
        void IUnitWork.StartTransaction()
        {
            throw new NotImplementedException();
        }



        Task IUnitWork.Commit()
        {
            throw new NotImplementedException();
        }

        public abstract Task Commit();

        Task IUnitWork.Rollback()
        {
            throw new NotImplementedException();
        }

    }
}
