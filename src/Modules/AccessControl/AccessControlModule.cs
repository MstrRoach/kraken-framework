using AccessControl.Infrastructure;
using Kraken.Module.Server;
using Kraken.Module.Transaction;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl;

public class AccessControlModule : IModule
{
    public string Name => "AccessControl";

    public void Register(IServiceCollection services)
    {
        //services.AddScoped<IUnitWork<AccessControlModule>, CommonUnitWork<AccessControlModule>>();
        //services.AddScoped<IOutboxStorage<AccessControlModule>, CommonOutboxStorage>();
        //services.AddScoped<IInboxStorage<AccessControlModule>, CommonReactionStorage>();
    }
}