using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Domain.Repository.EntityFrameworkCore;

internal class DefaultUnitWork<TModule, TContext> : IUnitWork<TModule>
    where TModule : IModule
    where TContext : DbContext
{
    /// <summary>
    /// Id de la transaccion actual
    /// </summary>
    public Guid TransactionId => _transaction?.TransactionId ?? Guid.Empty;

    /// <summary>
    /// Contexto actual
    /// </summary>
    readonly TContext _context;

    /// <summary>
    /// Transaccion actual de la unidad de trabajo
    /// </summary>
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Constructor de la unidad de trabajo
    /// </summary>
    /// <param name="context"></param>
    public DefaultUnitWork(TContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Confirma una transaccion contra la base de datos
    /// </summary>
    /// <returns></returns>
    public async Task Commit()
    {
        await _transaction.CommitAsync();
    }

    /// <summary>
    /// Revierte la transaccion en el modulo
    /// </summary>
    /// <returns></returns>
    public async Task Rollback()
    {
        await _transaction.RollbackAsync();
    }

    /// <summary>
    /// Inicia la transaccion en el modulo
    /// </summary>
    public void StartTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }
}
