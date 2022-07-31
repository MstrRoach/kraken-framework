using Kraken.Core;
using Kraken.Core.EventBus;
using Kraken.Core.Serializer;
using Kraken.Core.Transaction;
using Kraken.Core.UnitWork;
using Kraken.Host.Contexts;
using Kraken.Host.EventBus;
using Kraken.Host.Exceptions;
using Kraken.Host.Logging;
using Kraken.Host.Mediator;
using Kraken.Host.Modules;
using Kraken.Host.Outbox;
using Kraken.Host.Processing;
using Kraken.Host.Reaction;
using Kraken.Host.Serializer;
using Kraken.Host.Storage;
using Kraken.Host.Transaction;
using Kraken.Host.UnitWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Kraken.Host;

public static class KrakenExtensions
{
    /// <summary>
    /// Llave con la que identificamos al id de correlacion
    /// </summary>
    private const string CorrelationIdKey = "correlation-id";

    /// <summary>
    /// Llave con la que se obtiene el encabezado para obtener
    /// la direccion ip del cliente
    /// </summary>
    private static string HeaderIpAddressKey = "x-forwarded-for";

    /// <summary>
    /// Agrega todos los servicios de kraken y los modulos de kraken
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IServiceCollection AddKraken(this IServiceCollection services, 
        IConfiguration configuration ,Action<AppDescriptor> setup)
    {
        // Creamos las configuraciones
        AppDescriptor krakenOptions = new();
        setup(krakenOptions);
        // Las registramos como singlenton
        services.AddSingleton(krakenOptions);


        // ------------------------- Configuracion de las partes centrales de kraken
        //services.AddKrakenKernel(krakenOptions.assemblies);
        // Agregamos el serializador por defecto
        services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
        // Agregamos el host de kraken
        services.AddSingleton<IAppHost, DefaultHost>();
        // Agregamos el mediador
        services.AddMediator(krakenOptions.assemblies);
        // Agregamos los servicios por defecto
        // Fabrica de unidad de trabajo por defecto
        services.AddSingleton<IUnitWorkFactory, DefaultUnitWorkFactory>();
        // Unidad de trabajo por defecto
        services.AddScoped<IUnitWork, DefaultUnitWork>();
        // Agregamos el bus de eventos por defecto
        services.AddScoped<IEventBus, DefaultEventBus>();
        // Agregamos los repositorios y todo lo que tenga que ver con eso
        services.AddRepositorySupport(krakenOptions.assemblies);
        // Informacion del modulo
        services.AddModuleInfo(krakenOptions.modules);
        // Contexto de ejecucion
        services.AddContext();
        // Agregamos el manejo de las excepciones
        services.AddErrorHandling();

        // ------------------------- Configuracion de las caracteristicas adicionales
        // Agregamos la documentacion
        krakenOptions.Documentation?.AddServices(services);
        // Agregamos la configuracion de CORS
        krakenOptions.Cors?.AddServices(services);
        // Agregamos la authorizacion
        krakenOptions.Authorization?.AddServices(services);
        // Agregamos la autenticacion
        krakenOptions.Authentication?.AddServices(services);

        // ------------------------- Configuracion de las partes opcionales de kraken
        // Agrega las operaciones de transaccionalidad
        services.AddUnitWorks(krakenOptions.assemblies);
        // Agrega soporte de bandeja de salida para los eventos asincronos
        services.AddOutbox(krakenOptions.assemblies);
        // Agrega soporte para las reacciones
        services.AddReactions(krakenOptions.assemblies);


        // ------------------------- Configuracion de los modulos
        // Obtenemos las configuraciones de todos los modulos
        krakenOptions.modules.ForEach(module => configuration.GetSection(module.Name).Bind(module));
        // Registramos los modulos
        krakenOptions.modules.ForEach(module => module.Register(services));
        // Configuramos el arrancador
        services.AddSingleton<KrakenBootstrapper>()
            .AddSingleton<IBootstrapper>(x => x.GetRequiredService<KrakenBootstrapper>())
            .AddHostedService(x => x.GetRequiredService<KrakenBootstrapper>());
        // Devolvemos los servicios
        return services;
    }

    /// <summary>
    /// Agrega los servicios a la canalizacion de solicitudes del host
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseKraken(this WebApplication app)
    {
        // Creamos las opciones
        var krakenOptions = app.Services.GetRequiredService<AppDescriptor>();
        // Agregamos los componentes para la canalizacion de solicitudes

        // Usamos la redirecciones de headers
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        // Agregamos la configuracion de las politicas de cors
        krakenOptions.Cors?.UseServices(app);

        // Agrega el id de correlacion al componente principal
        app.UseCorrelationId();

        // Agregamos la administracion de errores globales
        app.UseErrorHandling();

        // Agregamos la seleccion de punto final, La autenticacion
        // debe hacerse despues de este punto en el pipeline para
        // asegurar la toma de desiciones correctas
        app.UseRouting();

        // Agregamos la documentacion
        if (!app.Environment.IsProduction())
            krakenOptions.Documentation?.UseServices(app);

        // Aqui debe de ir la seleccion de la autenticacion para
        // tener la informacion lista para el contexto
        krakenOptions.Authentication?.UseServices(app);

        // Agregamos la authorizacion en dado caso que sea confirmada
        krakenOptions.Authorization?.UseServices(app);

        // Agrega la extraccion de contextos
        app.UseContext();

        // Aqui agregamos el loggeo de la solicitud
        app.UseLogging();

        // Agregamos las configuraciones de los modulos al pipeline
        krakenOptions?.modules.ForEach(module => module.Use(app));
        krakenOptions?.Clear();

        // Devolvemos la applicacion
        return app;
    }

    

    /// <summary>
    /// Intenta recuperar el id de correlacion de la solicitud
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Guid? TryGetCorrelationId(this HttpContext context)
        => context.Items.TryGetValue(CorrelationIdKey, out var id) ? (Guid)id : null;

    /// <summary>
    /// Agrega el id de correlacion en la solicitud
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.Use((ctx, next) =>
        {
            ctx.Items.Add(CorrelationIdKey, Guid.NewGuid());
            return next();
        });

    /// <summary>
    /// Obtiene la ip del origen de la solicitud
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUserIpAddress(this HttpContext context)
    {
        if (context is null)
        {
            return string.Empty;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (context.Request.Headers.TryGetValue(HeaderIpAddressKey, out var forwardedFor))
        {
            var ipAddresses = forwardedFor.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (ipAddresses.Any())
            {
                ipAddress = ipAddresses[0];
            }
        }

        return ipAddress ?? string.Empty;
    }
}
