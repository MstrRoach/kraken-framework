using Kraken.Server.Features.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kraken.Server;

public static class KrakenServerExtensions
{

    public static WebApplication ConfigureKrakenServer(this WebApplicationBuilder builder,
        Action<ServerDescriptor> krakenSetup, 
        Action<WebApplication> pipelineSetup)
    {
        // Creamos la descripcion de los servicios configurados
        ServerDescriptor serverDescriptor = new();
        krakenSetup(serverDescriptor);
        // Registro de configuraciones del servidor kraken para ponerlas disponibles
        builder.Services.AddSingleton<ServerOptions>(builder.Configuration.GetNamedSection<ServerOptions>());
        // Registramos el descriptor como singleton
        builder.Services.AddSingleton(serverDescriptor);
        // --------------- Configuracion de las caracteristicas ------------------
        serverDescriptor.Documentation?.AddServices(builder.Services);
        serverDescriptor.Cors?.AddServices(builder.Services);
        serverDescriptor.Authorization?.AddServices(builder.Services);
        serverDescriptor.Authentication?.AddServices(builder.Services);
        // Construimos la app
        serverDescriptor.App = builder.Build();
        // --------------- Configuracion del pipeline del server -----------------
        // Usamos la redirecciones de headers
        serverDescriptor.App.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        // Configuracion de las politicas de cors
        serverDescriptor.Cors?.UseServices(serverDescriptor.App);
        // Agrega el id de correlacion al componente principal
        serverDescriptor.App.UseCorrelationId();
        return null;
    }

    private static void AddServices(this IServiceCollection services)
    {

    }

    /// <summary>
    /// Devuelve una instancia configurada del tipo pasado por
    /// parametros
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configuration"></param>
    public static T GetNamedSection<T>(this IConfiguration configuration)
        where T : new()
    {
        // Creamos la instancia con los valores por defecto
        var options = new T();
        // Obtenemos el valor de la seccion
        var optionName = typeof(T).Name;
        // bindeamos con el nombre de la clase
        configuration.GetSection(optionName).Bind(options);
        // devolvemos el valor configurado
        return options;
    }

    
}