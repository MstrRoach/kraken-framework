using Dottex.Kalypso.Module.Common;
using System.Reflection;

namespace Dottex.Domain.AuditStorage.InMemory;

/// <summary>
/// Datos de configuracion para el almacen de auditoria
/// </summary>
/// <typeparam name="T"></typeparam>
public class InMemoryAuditStorageOptions<T> where T : IModule
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
    public string DatabaseName { get; set; } =
        typeof(T).Name.Replace("Module", string.Empty);
}