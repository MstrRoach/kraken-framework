using Dottex.Kalypso.Module.Audit;
using Dottex.Kalypso.Module.Processing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Audit;

internal static class AuditExtensions
{
    /// <summary>
    /// Habilita los servicios generales para auditoria
    /// </summary>
    /// <returns></returns>
    public static IServiceCollection AddAudit(this IServiceCollection services,
        ServiceDescriptor? auditStorageDescriptor)
    {
        // Agrega el servicio por defecto de almacenamiento
        services.Add(auditStorageDescriptor ?? 
            ServiceDescriptor.Describe(
                typeof(IAuditStorage),
                typeof(DefaultAuditStorage),
                ServiceLifetime.Singleton
                ));
        // Agregamos el inicializador
        services.AddSingleton<IInitializer, AuditInitializer>();
        // Retornamos los registros
        return services;
    }
}
