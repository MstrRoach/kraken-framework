using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Dottex.Kalypso.Module.Common;

public record ModuleDescriptor<T> : IModuleDescriptor
    where T : class, IModule, new()
{
    public string Name { get; set; }
    public Assembly Assembly { get; set; }
    public Type Type { get; set; }

    /// <summary>
    /// Toma el modulo, lo crea, configura las preferencias, y
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public void ConfigureModule(IConfiguration configuration, IServiceCollection services)
    {
        T Module = new();
        configuration.GetSection(Name).Bind(Module);
        var options = Options.Create(Module);
        services.AddSingleton(options);
        Module.Register(services);
    }
}
