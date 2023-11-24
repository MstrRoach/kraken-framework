using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server;

/// <summary>
/// Contiene las configuraciones para la base de datos
/// del servidor en la memoria
/// </summary>
public class ServerDatabaseOptions
{

    /// <summary>
    /// Path hacia los datos de las aplicaciones
    /// </summary>
    public string ApplicationData { get; set; }
        = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

    /// <summary>
    /// Nombre de la aplicacion
    /// </summary>
    public string ApplicationName { get; set; }
        = Assembly.GetEntryAssembly().GetName().Name;

    /// <summary>
    /// Nombre de la base de datos
    /// </summary>
    public string DatabaseName { get; set; } = "Kalypso";
}
