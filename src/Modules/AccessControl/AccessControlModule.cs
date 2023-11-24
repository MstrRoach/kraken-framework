using AccessControl.Infrastructure;
using Dottex.Domain.Repository.InMemory;
using Dottex.Kalypso.Domain;
using Dottex.Kalypso.Module.Common;
using Dottex.Kalypso.Module.Transaction;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl;

public class AccessControlModule : IModule
{
    public string Name => "AccessControl";
    public string Database {  get; set; }

    public void Register(IServiceCollection services)
    {
        services.AddScoped<IUnitWork<AccessControlModule>, UnitWork<AccessControlModule>>();
        services.AddDomainDrivenDesign<AccessControlModule>(builder => builder.UseInMemoryRepository<AccessControlModule>());
    }
}