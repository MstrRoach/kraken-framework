using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Inflow.Shared.Infrastructure.Modules;
using Kraken.Core.Modules;

namespace Kraken.Host.Modules;

public static class Extensions
{

    /// <summary>
    /// Agrega la informacion de los modulos disponible
    /// </summary>
    /// <param name="services"></param>
    /// <param name="modules"></param>
    /// <returns></returns>
    public static IServiceCollection AddModuleInfo(this IServiceCollection services, IList<IModule> modules)
    {
        var moduleInfoProvider = new ModuleInfoProvider();
        var moduleInfo =
            modules?.Select(x => new ModuleInfo(x.Name)) ??
            Enumerable.Empty<ModuleInfo>();
        moduleInfoProvider.Modules.AddRange(moduleInfo);
        services.AddSingleton(moduleInfoProvider);

        return services;
    }

    /// <summary>
    /// Agrega un endpoint donde se muestra la informacion de los modulos, futura mejora, utilizar una pagina
    /// estatica para mostrar la informacion mas adecuada
    /// </summary>
    /// <param name="endpoint"></param>
    public static void MapModuleInfo(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("modules", context =>
        {
            var moduleInfoProvider = context.RequestServices.GetRequiredService<ModuleInfoProvider>();
            return context.Response.WriteAsJsonAsync(moduleInfoProvider.Modules);
        });
    }
    
}