using AccessControl.Infrastructure;
using Kraken.Standard.Server;
using Kraken.Standard.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl;

public class AccessControlModule : IModule
{
    public string Name => "AccessControl";

    public void Register(IServiceCollection services)
    {
        services.AddScoped<IUnitWork<AccessControlModule>, CommonUnitWork<AccessControlModule>>();
    }
}