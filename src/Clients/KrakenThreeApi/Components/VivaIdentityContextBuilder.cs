using Kraken.Server.Features.Contexts;
using Kraken.Standard.Context;
using System.Security.Claims;

namespace KrakenThreeApi.Components;

public class VivaIdentityContextBuilder : IIdentityContextBuilder
{
    public IIdentityContext Build(ClaimsPrincipal principal)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Constructor no inicializado
    /// </summary>
    /// <returns></returns>
    public IIdentityContext Empty() => new VivaIdentityContext();
}

public class VivaIdentityContext : IIdentityContext
{
    public bool IsAuthenticated { get; }

    public string Id { get; }

    public string Name { get; }

    public string Role { get; }

    public Dictionary<string, IEnumerable<string>> Claims { get; }

    /// <summary>
    /// Identidad desconocida
    /// </summary>
    public VivaIdentityContext()
    {
        IsAuthenticated = false;
        Id = Guid.Empty.ToString();
        Name = "Unknown";
        Role = "Unknown";
    }



    public string TryGetClaim(string claimName)
    {
        throw new NotImplementedException();
    }
}
