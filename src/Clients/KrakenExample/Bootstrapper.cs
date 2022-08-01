using IdentityManagement;
using Kraken.Host;
using Kraken.Host.Features.Cors;
using Kraken.Host.Features.Documentation;
using KrakenExample.Features;
using Microsoft.OpenApi.Models;
using ProfileManagement;

namespace KrakenExample;

public static class Bootstrapper
{
    /// <summary>
    /// Configura el constructor del builder
    /// </summary>
    /// <returns></returns>
    public static Action<AppDescriptor> KrakenBuilder(IConfiguration configuration) 
        => (builder) =>
    {
        builder.AddModule<IdentityModule>();
        builder.AddModule<ProfileModule>();
        builder.AddDocumentation(x =>
        {
            x.Title = "Example Web Api";
            x.SecurityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };
        });
        builder.AddCorsPolicy(x =>
        {
            x.allowCredentials = true;
            x.allowedMethods = new[] { "*" };
            x.allowedHeaders = new[] { "*" };
            x.allowedOrigins = new[] { "*" };
        });
        builder.AddCustomAuthorization();
        builder.AddCustomAuthentication();
    };
}
