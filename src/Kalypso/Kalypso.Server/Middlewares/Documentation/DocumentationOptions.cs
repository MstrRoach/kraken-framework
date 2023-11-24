using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Documentation;

/// <summary>
/// Configuracion para el esquema  documentacion
/// con validacion de datos y esquema de seguridad
/// </summary>
public class DocumentationOptions
{
    /// <summary>
    /// Version del API
    /// </summary>
    public string Version { get; set; } = "v8";

    /// <summary>
    /// Titulo de la aplicacion
    /// </summary>
    public string Title { get; set; } = "Kalypso Project Base";

    /// <summary>
    /// Descripcion del API
    /// </summary>
    public string Description { get; set; } = "App powered by Kalypso Framework";

    /// <summary>
    /// Correo del desarrollador
    /// </summary>
    public string Email { get; set; } = "imct.jesus.antonio@gmail.com";

    /// <summary>
    /// Nombre del desarrollador
    /// </summary>
    public string Name { get; set; } = "Jesus Antonio Martinez Hernandez";

    /// <summary>
    /// Direccion donde desplegar la docummentacion en json. Debe de contener
    /// el template {documentName} que sera llenado con la version del api
    /// especificada arriba
    /// </summary>
    public string Url { get; set; } = "/swagger/docs/{documentName}/swagger.json";

    /// <summary>
    /// Indica el prefix que debe aparecer en la liga del ui. Si es vacio, entonces
    /// aparecera en la ruta raiz.
    /// </summary>
    public string RoutePrefix { get; set; } = "swagger";

    /// <summary>
    /// Titulo del documento donde se despliega la doc en json
    /// </summary>
    public string DocumentTitle { get; set; } = "Kalypso Reference";

    /// <summary>
    /// Indica si la generacion de la documentacion xml se genera
    /// </summary>
    public bool XmlGenEnabled { get; set; } = false;

    /// <summary>
    /// Define un esquema de seguridad para configurar el acceso hacia swagger
    /// </summary>
    public OpenApiSecurityScheme SecurityScheme { get; set; } = default;
}
