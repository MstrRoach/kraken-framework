using Kraken.Core.UnitWork;
using MediatR;
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
        /// Bus de eventos para los eventos de infrastructura
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor para la unidad de trabajo base
        /// </summary>
        /// <param name="mediator"></param>
        public AbstractUnitWork(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Inicia la transaccion y administra los 
        /// eventos que deben de lanzarse
        /// </summary>
        public abstract void StartTransaction();


        /// <summary>
        /// Confirma los cambios de la base de da
        /// </summary>
        /// <returns></returns>
        public abstract Task Commit();
        public abstract Task Rollback();
    }
}
