using Dottex.Kalypso.Domain.Audit;
using Dottex.Kalypso.Module.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Domain.AuditStorage.InMemory;

public class InMemoryAuditStorageExtensions<TModule> : IAuditStorageExtension<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Opciones configuradas para el almacen de  auditoria para
    /// el modulo
    /// </summary>
    private InMemoryAuditStorageOptions<TModule> options;

    /// <summary>
    /// Constructor con la configuracion
    /// </summary>
    /// <param name="options"></param>
    public InMemoryAuditStorageExtensions(InMemoryAuditStorageOptions<TModule> options)
    {
        this.options = options;
    }

    /// <summary>
    /// Agrega los servicios para habilitar el almacen de auditoria para
    /// el modulo
    /// </summary>
    /// <param name="services"></param>
    public void AddServices(IServiceCollection services)
    {
        // Agregamos las opciones de configuracion
        services.AddSingleton(options);
        var type = typeof(DefaultAuditStorage<TModule>);
        // Agregamos el almacen para el modulo
        services.AddScoped<IAuditStorage<TModule>, DefaultAuditStorage<TModule>>();
        // Agregamos el arrancador
        services.AddSingleton<DatabaseBootstrap<TModule>>();
        // Inicializamos la base de datos
        services.BuildServiceProvider()
            .GetService<DatabaseBootstrap<TModule>>()!
            .Initialize();
    }
}
