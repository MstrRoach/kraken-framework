using Dottex.Kalypso.Domain.Core;
using Dottex.Kalypso.Module;
using Dottex.Kalypso.Module.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Audit;

/// <summary>
/// Accesor para almacenar los registros de auditoria
/// </summary>
public class AuditStorage
{
    /// <summary>
    /// Proveedor de los servicios
    /// </summary>
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Registros de los modulos por tipo
    /// </summary>
    readonly ModuleRegistry _moduleRegistry;

    public AuditStorage(IServiceProvider serviceProvider, 
        ModuleRegistry moduleRegistry)
    {
        _serviceProvider = serviceProvider;
        _moduleRegistry = moduleRegistry;
    }

    /// <summary>
    /// Almacena un registro de auditoria
    /// </summary>
    /// <param name="record"></param>
    public void Save<T>(Change record) where T : IAggregate
    {
        var moduleType = _moduleRegistry.Resolve<T>();
        var auditModuleStorage = typeof(IAuditStorage<>).MakeGenericType(moduleType);
        var storage = _serviceProvider.GetService(auditModuleStorage) as IAuditStorage;
        storage.Save(record);
    }
}
