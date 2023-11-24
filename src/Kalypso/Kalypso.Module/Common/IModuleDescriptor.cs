using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dottex.Kalypso.Module.Common;

public interface IModuleDescriptor
{
    Type Type { get; }
    Assembly Assembly { get; }
    void ConfigureModule(IConfiguration configuration, IServiceCollection services);
}
