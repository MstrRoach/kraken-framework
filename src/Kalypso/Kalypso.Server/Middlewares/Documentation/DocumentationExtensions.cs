using Dottex.Kalypso.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Documentation;

/// <summary>
/// Registra la configuracion de la documentacion desde las
/// opciones de kalypso
/// </summary>
public static class DocumentationExtensions
{
    /// <summary>
    /// Configura la generacion de la documentacion
    /// </summary>
    /// <param name="services"></param>
    /// <param name="setup"></param>
    /// <returns></returns>
    public static IServiceCollection AddDocumentation(this IServiceCollection services, 
        Action<DocumentationOptions> setup)
    {
        var options = new DocumentationOptions();
        setup(options);
        services.AddSingleton(options);
        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();
            swagger.CustomSchemaIds(x => x.FullName);
            // Datos de documentacion basicos
            swagger.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options?.Title ?? "kalypso Server",
                Version = options?.Version ?? "v8",
                Description = options?.Description ?? "Modular Monolith Application",
                Contact = CreateContact(options.Name, options.Email)
            });

            // Agregamos los esquemas de seguridad
            if (options.SecurityScheme is not null)
            {
                swagger.AddSecurityDefinition(options.SecurityScheme.Reference.Id, options.SecurityScheme);
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { options.SecurityScheme, new string[]{ } }
                });
            }

            if (options.XmlGenEnabled)
            {
                // Generacion de la documentacion
                var xmlFile = $"KalypsoDoc.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
            }

        });

        return services;
    }

    /// <summary>
    /// Construye la instancia de un contacto de open api
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    private static OpenApiContact CreateContact(string name, string email)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email))
            return null;
        return new OpenApiContact
        {
            Name = name,
            Email = email
        };
    }

    /// <summary>
    /// Configura la documentacion en la canalizacion
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseDocumentation(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<DocumentationOptions>();

        if (options is null)
            return app;

        app.UseSwagger(c =>
        {
            c.RouteTemplate = options.Url;
        });

        app.UseSwaggerUI(ui =>
        {
            ui.SwaggerEndpoint(options.Url.Replace("{documentName}", options.Version), options.Title);
            ui.RoutePrefix = options.RoutePrefix;
            ui.DocumentTitle = options.DocumentTitle;
        });

        return app;
    }
}
