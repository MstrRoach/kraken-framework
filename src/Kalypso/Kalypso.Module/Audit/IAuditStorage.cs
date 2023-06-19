using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Audit;

/// <summary>
/// Contrato para la definicion de almacen para
/// centralizar las auditorias de entidades
/// </summary>
public interface IAuditStorage
{
    /// <summary>
    /// Obtiene registros a partir de una serie de filtros
    /// </summary>
    /// <returns></returns>
    List<AuditLog> GetAll(AuditFilter filter);


    /// <summary>
    /// Almacena un registro de auditoria
    /// </summary>
    /// <param name="record"></param>
    void Save(AuditLog record);
}
