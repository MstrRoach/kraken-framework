using Dottex.Kalypso.Server;
using Dottex.Kalypso.Server.Middlewares.Documentation;
using Dottex.Kalypso.Server.Middlewares.Cors;
using AccessControl;
using KrakenThreeApi.CommonServices;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Dottex.Kalypso.Server.Middlewares.Correlation;
using Dottex.Kalypso.Server.Middlewares.ErrorHandling;
using Dottex.Kalypso.Server.Middlewares.Contexts;
using Dottex.Kalypso.Server.Middlewares.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.AddKalypso(options =>
{
    options.AddModule<AccessControlModule>();
});
builder.Services.AddDocumentation(options =>
{
    options.Title = "Example Web Api v3.0";
    options.SecurityScheme = new OpenApiSecurityScheme
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
builder.Services.AddCorsPolicy(options =>
{
    options.Name = "KalypsoPolicy";
    options.allowCredentials = true;
    options.allowedMethods = ["*"];
    options.allowedHeaders = ["*"];
    options.allowedOrigins = ["*"];
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//// Configure the HTTP request pipeline.

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
});

app.UseCorsPolicy();

app.UseCorrelationId();

app.UseErrorHandling();

app.UseRouting();

app.UseDocumentation();

app.UseContext();

app.UseLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
