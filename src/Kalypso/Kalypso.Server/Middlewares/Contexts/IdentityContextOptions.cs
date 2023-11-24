using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Middlewares.Contexts;

/// <summary>
/// Clase para definir los valores desde donde se obtiene las propiedades
/// para el contexto de identidad, para poder ser configurado desde fuera 
/// del marco sin necesidad de tener un builder personalizado
/// </summary>
public class IdentityContextOptions
{
    /// <summary>
    /// Clave que identifica al id del usuario
    /// </summary>
    public string Id { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Clave para identificar al nombre del usuario
    /// </summary>
    public string Name { get; set; } = ClaimTypes.Name;

    /// <summary>
    /// Clave para identificar al rol del usuario
    /// </summary>
    public string Role { get; set; } = ClaimTypes.Role;
}
