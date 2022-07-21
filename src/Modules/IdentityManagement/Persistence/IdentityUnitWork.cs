using Kraken.Core.UnitWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Persistence
{
    internal class IdentityUnitWork : UnitWork
    {
        /// <summary>
        /// Accede al id de la transaccion actual
        /// </summary>
        public override Guid TransactionId => Guid.NewGuid();

        /// <summary>
        /// Constructor de la unidad de trabajo especificada
        /// </summary>
        /// <param name="mediator"></param>
        public IdentityUnitWork(IMediator mediator) : 
            base(mediator) { }

        /// <summary>
        /// Inicia la transaccion en el modulo
        /// </summary>
        public override void StartTransaction()
        {
        }

        /// <summary>
        /// Confirma una transaccion contra la base de datos
        /// </summary>
        /// <returns></returns>
        public override async Task Commit()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Revierte la transaccion en el modulo
        /// </summary>
        /// <returns></returns>
        public override async Task Rollback()
        {
            await Task.CompletedTask;
        }

    }
}
