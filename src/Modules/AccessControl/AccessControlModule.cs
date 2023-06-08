using Domain.Repository.InMemory;
using Kraken.Domain;
using Kraken.Module.Common;
using Kraken.Module.Transaction;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl;

public class AccessControlModule : IModule
{
    public string Name => "AccessControl";

    public void Register(IServiceCollection services)
    {
        //services.AddScoped<IUnitWork<AccessControlModule>, CommonUnitWork<AccessControlModule>>();
        services.AddDomainDrivenDesign<AccessControlModule>(builder =>
        {
            builder.UseInMemoryRepository();
        });
    }
}