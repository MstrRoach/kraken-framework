// See https://aka.ms/new-console-template for more information
using IdentityManagement;
using Kraken.Core.UnitWork;
using Kraken.Host;
using Kraken.Host.UnitWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var builder = WebApplication.CreateBuilder(args);

IReadOnlyDictionary<string, string> TestConfigurationProvider = new Dictionary<string, string>();
ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
configurationBuilder.AddInMemoryCollection(TestConfigurationProvider);
var configuration = configurationBuilder.Build();

builder.Services.AddKraken(configuration, x => x.AddModule<IdentityModule>());

var app = builder.Build();

// Inyectamos los servicios en la canalizacion de solicitudes
app.UseKraken();

foreach (var service in builder.Services)
{
    Console.WriteLine($"{service.ServiceType.FullName}:::{service.ImplementationType?.FullName}");
}

var serviceProvider = builder.Services.BuildServiceProvider();

var handlers = serviceProvider
    .GetServices<INotificationHandler<TransactionStarted>>()
    .ToList();

//var handlerCentral = serviceProvider.GetServices<>

var dd = 3;
