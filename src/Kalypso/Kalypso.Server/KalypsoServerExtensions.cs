using Dottex.Kalypso.Server.Common;
using Dottex.Kalypso.Server.Middlewares.Contexts;
using Dottex.Kalypso.Server.Middlewares.Correlation;
using Dottex.Kalypso.Server.Middlewares.ErrorHandling;
using Dottex.Kalypso.Server.Middlewares.Logging;
using Dottex.Kalypso.Server.Request;
using Dottex.Kalypso.Server.Transaction;
using Dottex.Kalypso.Server.TransactionalOutbox;
using Dottex.Kalypso.Server.TransactionalReaction;
using Dottex.Kalypso.Module.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Dottex.Kalypso.Server;

public static class KalypsoServerExtensions
{

    public static WebApplication ConfigureKalypsoServer(this WebApplicationBuilder builder,
        Action<ServerDescriptor> kalypsoSetup)
    {
        // Creamos la descripcion de los servicios configurados
        ServerDescriptor serverDescriptor = new();
        kalypsoSetup(serverDescriptor);
        // Registro de configuraciones del servidor Kalypso para ponerlas disponibles
        builder.Services.AddSingleton(builder.Configuration.GetNamedSection<ServerOptions>());
        // Registramos el descriptor como singleton
        builder.Services.AddSingleton(serverDescriptor);
        builder.Services.AddSingleton(serverDescriptor.moduleRegistry);
        // =============== Configurando las partes centrales de Kalypso ==================
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
        builder.Services.AddTransactionalOutbox(serverDescriptor.OutboxStorageDescriptor);
        builder.Services.AddTransactionalReaction(serverDescriptor.assemblies, serverDescriptor.ReactionStorageDescriptor);
        // builder.Services.AddTransactionalInbox(serverDescriptor.assemblies);
        // =============== Configuracion de los servicios de modulo =====================
        serverDescriptor.modules.ForEach(module => builder.Configuration.GetSection(module.Name).Bind(module));
        serverDescriptor.modules.ForEach(module => builder.Services.AddSingleton(Options.Create(module)));
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
        if (serverDescriptor.UseHttpsRedirection)
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