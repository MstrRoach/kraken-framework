using IdentityManagement;
using Kraken.Core.UnitWork;
using Kraken.Host;
using Kraken.Host.Features.Cors;
using Kraken.Host.Features.Documentation;
using Kraken.Host.Modules;
using KrakenExample;
using MediatR;
using Microsoft.OpenApi.Models;
using ProfileManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddKraken(builder.Configuration ,Bootstrapper.KrakenBuilder());
builder.Services.AddKraken(builder.Configuration ,x =>
{
    x.AddModule<IdentityModule>();
    x.AddModule<ProfileModule>();
    x.AddDocumentation(x =>
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
    x.AddCorsPolicy(x =>
    {
        x.allowCredentials = true;
        x.allowedMethods = new string[] { "*" };
        x.allowedHeaders = new string[] { "*" };
        x.allowedOrigins = new string[] { "*" };
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inyectamos los servicios en la canalizacion de solicitudes
app.UseKraken();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapGet("/", context => context.Response.WriteAsync("Kraken Example API"));
//    endpoints.MapModuleInfo();
//});

app.Run();
