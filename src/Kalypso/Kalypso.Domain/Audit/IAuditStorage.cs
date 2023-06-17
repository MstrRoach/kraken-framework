using Dottex.Kalypso.Module.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Audit;

/// <summary>
/// Contrato para las acciones de almacen de
/// auditorias
/// </summary>
public interface IAuditStorage
{
    /// <summary>
    /// Almacena un registro de auditoria
    /// </summary>
    /// <param name="record"></param>
    void Save(Change record);
}

/// <summary>
/// Define el contrato para crear un almacen para
/// auditoria por modulo
/// </summary>
public interface IAuditStorage<TModule> : IAuditStorage 
    where TModule : IModule
{
    
}
