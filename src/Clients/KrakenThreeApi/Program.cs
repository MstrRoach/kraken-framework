using Dottex.Kalypso.Server;
using Dottex.Kalypso.Server.Middlewares.Documentation;
using Dottex.Kalypso.Server.Middlewares.Cors;
using AccessControl;
using KrakenThreeApi.CommonServices;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var krakenApp = builder.ConfigureKalypsoServer(server =>
{
    server.AddModule<AccessControlModule>();
    //server.AddOutboxStorage<InMemoryOutboxStorage>();
    server.AddReactionStorage<InMemoryReactionStorage>();
    server.ShowDocumentation = true;
    server.UseHttpsRedirection = true;
    server.AddDocumentation(doc =>
    {
        doc.Title = "Example Web Api v3.0";
        doc.SecurityScheme = new OpenApiSecurityScheme
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
    server.AddCorsPolicy(cors =>
    {
        cors.allowCredentials = true;
        cors.allowedMethods = new[] { "*" };
        cors.allowedHeaders = new[] { "*" };
        cors.allowedOrigins = new[] { "*" };
    });
});

// Run kraken server
krakenApp.Run();



//var app = builder.Build();

//// Configure the HTTP request pipeline.


//app.Run();
