using Kraken.Module.Context;
using System.Security.Claims;

namespace Kraken.Server.Middlewares.Contexts;

internal class DefaultIdentityContext : IIdentityContext
{
    public bool IsAuthenticated { get; } = false;

    public string Id { get; } = Guid.Empty.ToString();

    public string Name { get; } = "Unknown";

    public string Role { get; } = "Unknown";

    public Dictionary<string, IEnumerable<string>> Claims { get; } = new();

    /// <summary>
    /// Identidad desconocida
    /// </summary>
    public DefaultIdentityContext()
    {

    }

    /// <summary>
    /// Constructor del contexto con id, role y nombre especifico
    /// </summary>
    /// <param name="id"></param>
    /// <param name="role"></param>
    /// <param name="name"></param>
    public DefaultIdentityContext(string id, string role, string name)
    {
        Id = id ?? Guid.Empty.ToString();
        IsAuthenticated = Id != Guid.Empty.ToString();
        Role = role;
        Name = name;
    }

    /// <summary>
    /// Constructor del contexto a partir de las claims del usuario
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="properties"></param>
    public DefaultIdentityContext(ClaimsPrincipal principal, IdentityContextProperties properties)
    {
        if (principal == null)
            return;
        if (principal?.Identity is null || string.IsNullOrWhiteSpace(principal.Identity.Name))
        {
            return;
        }
        IsAuthenticated = principal.Identity?.IsAuthenticated is true;
        Id = IsAuthenticated ? principal.Claims.SingleOrDefault(x => x.Type == properties.Id).Value : Guid.Empty.ToString();
        Role = principal.Claims.SingleOrDefault(x => x.Type == properties.Role)?.Value;
        Name = principal.Claims.SingleOrDefault(x => x.Type == properties.Name).Value;
        Claims = principal.Claims.GroupBy(x => x.Type)
            .ToDictionary(x => x.Key, x => x.Select(c => c.Value.ToString()));
    }

    /// <summary>
    /// Obtiene la claim por el nombre
    /// </summary>
    /// <param name="claimName"></param>
    /// <returns></returns>
    public string TryGetClaim(string claimName)
    {
        var claim = Claims.GetValueOrDefault(claimName);
        if (claim is null)
            return null;
        return claim.First();
    }
}