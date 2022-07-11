using Kraken.Core.UnitWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement.Persistence
{
    internal class IdentityUnitWork : IUnitWork
    {
        
        public async void StartTransaction()
        {
            await Task.CompletedTask;
        }
        public async Task Commit()
        {
            await Task.CompletedTask;
        }

        public async Task Rollback()
        {
            await Task.CompletedTask;
        }

    }
}
