﻿using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Features.Documentation
{
    /// <summary>
    /// Configuracion para el esquema  documentacion
    /// con validacion de datos y esquema de seguridad
    /// </summary>
    public class DocumentationOptions
    {
        /// <summary>
        /// Version del API
        /// </summary>
        public string Version { get; set; } = "v2";

        /// <summary>
        /// Titulo de la aplicacion
        /// </summary>
        public string Title { get; set; } = "Kraken Project Base";

        /// <summary>
        /// Descripcion del API
        /// </summary>
        public string Description { get; set; } = "App powered by Kraken Framework";

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
        public string Url { get; set; } = "/kraken/docs/{documentName}/kraken.json";

        /// <summary>
        /// Titulo del documento donde se despliega la doc en json
        /// </summary>
        public string DocumentTitle { get; set; } = "Kraken Reference";

        /// <summary>
        /// Indica si usa la ui de swagger o por el contrario
        /// la ui de redoc
        /// </summary>
        public bool UseSwaggerUI { get; set; } = true;

        /// <summary>
        /// Define un esquema de seguridad para configurar el acceso hacia swagger
        /// </summary>
        public OpenApiSecurityScheme SecurityScheme { get; set; } = default;
    }
}
