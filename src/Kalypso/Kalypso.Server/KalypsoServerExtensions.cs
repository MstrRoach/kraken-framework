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
using Dottex.Kalypso.Server.Audit;
using Dapper;

namespace Dottex.Kalypso.Server;

public static class KalypsoServerExtensions
{
    /// <summary>
    /// Agrega kalypso como proveedor de ciertas configuraciones previas
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="kalypsoSetup"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddKalypso(this WebApplicationBuilder builder, Action<KalypsoOptions> kalypsoSetup)
    {
        // Creamos la descripcion de los servicios configurados
        KalypsoOptions serverDescriptor = new();
        kalypsoSetup(serverDescriptor);
        // Registro de configuraciones del servidor Kalypso para ponerlas disponibles
        builder.Services.AddSingleton(builder.Configuration.GetNamedSection<ServerOptions>());
        // Registramos el descriptor como singleton
        builder.Services.AddSingleton(serverDescriptor);
        builder.Services.AddSingleton(serverDescriptor.moduleRegistry);
        builder.Services.AddSingleton(serverDescriptor.DatabaseOptions);
        
        // =============== Configurando las partes centrales de Kalypso ==================
        builder.Services.AddContext(serverDescriptor.IdentityContextOptions);
        builder.Services.AddErrorHandling();
        builder.Services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        SqlMapper.AddTypeHandler<Guid>(new GuidTypeHandler());
          
        // =============== Configuracion de las caracteristicas centrales ===============
        builder.Services.AddCommandAndQueryProcessing(serverDescriptor.moduleRegistry.Assemblies);
        builder.Services.AddTransaction();
        builder.Services.AddTransactionalOutbox(serverDescriptor.OutboxStorageDescriptor);
        builder.Services.AddTransactionalReaction(serverDescriptor.moduleRegistry.Assemblies, serverDescriptor.ReactionStorageDescriptor);
        builder.Services.AddAudit(serverDescriptor.AuditStorageDescriptor);

        // =============== Configuracion de los servicios de modulo =====================
        serverDescriptor.moduleRegistry.Configure(builder.Configuration, builder.Services);

        builder.Services.AddSingleton<ServerBootstrapper>()
            .AddHostedService(x => x.GetRequiredService<ServerBootstrapper>());

        return builder;
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