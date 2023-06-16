using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Domain.Audit;

/// <summary>
/// Interface para la definicion de la configuracion para el almacen
/// de auditoria
/// </summary>
public interface IAuditStorageExtension
{
    /// <summary>
    /// Agrega los servicios para la configuracion del repositorio en turno
    /// </summary>
    /// <param name="services"></param>
    void AddServices(IServiceCollection services);
}
