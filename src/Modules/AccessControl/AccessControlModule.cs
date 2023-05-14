using AccessControl.Infrastructure;
using Kraken.Module.Outbox;
using Kraken.Module.Reaction;
using Kraken.Module.Server;
using Kraken.Module.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl;

public class AccessControlModule : IModule
{
    public string Name => "AccessControl";

    public void Register(IServiceCollection services)
    {
        services.AddScoped<IUnitWork<AccessControlModule>, CommonUnitWork<AccessControlModule>>();
        services.AddScoped<IOutboxStorage<AccessControlModule>, CommonOutboxStorage>();
        services.AddScoped<IReactionStorage<AccessControlModule>, CommonReactionStorage>();
    }
}