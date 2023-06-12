using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infrastructure;

internal class CommonUnitWork<T> : IUnitWork<T>
    where T : IModule
{
    public Guid TransactionId { get; internal set; }

    public Task Commit()
    {
        return Task.CompletedTask;
    }

    public Task Rollback()
    {
        return Task.CompletedTask;
    }

    public void StartTransaction()
    {
        return;
    }
}
