using Kraken.Module.Common;
using Kraken.Server.Common;
using Kraken.Server.Middlewares.Contexts;
using Kraken.Server.Middlewares.Correlation;
using Kraken.Server.Middlewares.ErrorHandling;
using Kraken.Server.Middlewares.Logging;
using Kraken.Server.Outbox;
using Kraken.Server.Request;
using Kraken.Server.Transaction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kraken.Server;

public static class KrakenServerExtensions
{

    public static WebApplication ConfigureKrakenServer(this WebApplicationBuilder builder,
        Action<ServerDescriptor> krakenSetup)
    {
        // Creamos la descripcion de los servicios configurados
        ServerDescriptor serverDescriptor = new();
        krakenSetup(serverDescriptor);
        // Registro de configuraciones del servidor kraken para ponerlas disponibles
        builder.Services.AddSingleton<ServerOptions>(builder.Configuration.GetNamedSection<ServerOptions>());
        // Registramos el descriptor como singleton
        builder.Services.AddSingleton(serverDescriptor);
        builder.Services.AddSingleton(serverDescriptor.moduleRegistry);
        // =============== Configurando las partes centrales de kraken ==================
        builder.Services.AddContext(serverDescriptor.IdentityContextProperties);
        builder.Services.AddErrorHandling();
        builder.Services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        // =============== Configuracion de las caracteristicas del server ==============
        serverDescriptor.Documentation?.AddServices(builder.Services);
        serverDescriptor.Cors?.AddServices(builder.Services);
        serverDescriptor.Authorization?.AddServices(builder.Services);
        serverDescriptor.Authentication?.AddServices(builder.Services);

        // =============== Configuracion de las caracteristicas centrales ===============
        builder.Services.AddCommandAndQueryProcessing(serverDescriptor.assemblies);
        builder.Services.AddTransaction();
        builder.Services.AddTransactionalOutbox();
        // =============== Configuracion de los servicios de modulo =====================
        serverDescriptor.modules.ForEach(module => builder.Configuration.GetSection(module.Name).Bind(module));

        serverDescriptor.modules.ForEach(module => module.Register(builder.Services));

        builder.Services.AddSingleton<ServerBootstrapper>()
            .AddHostedService(x => x.GetRequiredService<ServerBootstrapper>());
        // =============== Configuracion por defecto de web api =========================
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        // =============== Configuracion del pipeline del server ========================
        // Construimos la app
        serverDescriptor.App = builder.Build();
        // Reenvio de headers
        serverDescriptor.App.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        // Cors activas
        serverDescriptor.Cors?.UseServices(serverDescriptor.App);
        // Correlacion de id
        serverDescriptor.App.UseCorrelationId();
        // Manejo global de excepciones
        serverDescriptor.App.UseErrorHandling();
        // Routing activado
        serverDescriptor.App.UseRouting();
        // Documentacion
        if (serverDescriptor.ShowDocumentation)
            serverDescriptor.Documentation?.UseServices(serverDescriptor.App);
        // Autenticacion
        serverDescriptor.Authentication?.UseServices(serverDescriptor.App);
        // Authorizacion
        serverDescriptor.Authorization?.UseServices(serverDescriptor.App);
        // Agregamos la configuracion de contextos
        serverDescriptor.App.UseContext();
        // Agregamos el loggeo de la solicitud
        serverDescriptor.App.UseLogging();
        // Redireccion http
        if(serverDescriptor.UseHttpsRedirection)
            serverDescriptor.App.UseHttpsRedirection();
        // Mapeo de controladores
        serverDescriptor.App.MapControllers();
        // Devolvemos la app configurada
        return serverDescriptor.App;
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