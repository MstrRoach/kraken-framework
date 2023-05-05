using Kraken.Server;
using Kraken.Server.Features.Cors;
using Kraken.Server.Features.Documentation;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var krakenApp = builder.ConfigureKrakenServer(server =>
{

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
},
services =>
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}, app =>
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
});

// Run kraken server
krakenApp.Run();



//var app = builder.Build();

//// Configure the HTTP request pipeline.


//app.Run();
