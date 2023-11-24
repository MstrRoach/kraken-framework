using Dottex.Kalypso.Module.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Documentation
{
    public class DocumentationFeature : IFeature
    {
        /// <summary>
        /// Opciones para configurar la documentacion
        /// </summary>
        private readonly DocumentationOptions _options;

        /// <summary>
        /// Constructor del feature para agregar la documentacion
        /// </summary>
        /// <param name="options"></param>
        public DocumentationFeature(DocumentationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Construye el feature a partir de una accion de configuracion
        /// </summary>
        /// <param name="setup"></param>
        public DocumentationFeature(Action<DocumentationOptions> setup)
        {
            _options = new DocumentationOptions();
            setup(_options);
        }

        /// <summary>
        /// Registra los servicios necesarios para la documentacion
        /// en el contenedor de servicios
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton(_options);
            services.AddSwaggerGen(swagger =>
            {
                swagger.EnableAnnotations();
                swagger.CustomSchemaIds(x => x.FullName);
                // Datos de documentacion basicos
                swagger.SwaggerDoc(_options.Version, new OpenApiInfo
                {
                    Title = _options is not null ? _options.Title : "kalypso Server",
                    Version = _options is not null ? _options.Version : "v1",
                    Description = _options is not null ? _options.Description : "Modular Monolith Api",
                    Contact = CreateContact(_options.Name, _options.Email)
                });

                // Agregamos los esquemas de seguridad
                if (_options.SecurityScheme is not null)
                {
                    swagger.AddSecurityDefinition(_options.SecurityScheme.Reference.Id, _options.SecurityScheme);
                    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { _options.SecurityScheme, new string[]{ } }
                });
                }

                // Generacion de la documentacion
                //var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //swagger.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// Agrega los servicios a la canalizacion de solicitud
        /// </summary>
        /// <param name="app"></param>
        public void UseServices(IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<DocumentationOptions>();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = _options.Url;
            });

            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint(_options.Url.Replace("{documentName}", _options.Version), _options.Title);
                ui.RoutePrefix = _options.RoutePrefix;
                ui.DocumentTitle = _options.DocumentTitle;
            });
        }

        /// <summary>
        /// Construye la instancia de un contacto de open api
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        internal OpenApiContact CreateContact(string name, string email)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email))
                return null;
            return new OpenApiContact
            {
                Name = name,
                Email = email
            };
        }
    }
}
