using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Audit;

/// <summary>
/// Define el contrato para crear un almacen para
/// auditoria
/// </summary>
public interface IAuditStorage
{
    /// <summary>
    /// Almacena un registro de auditoria
    /// </summary>
    /// <param name="record"></param>
    void Save(Change record);
}
