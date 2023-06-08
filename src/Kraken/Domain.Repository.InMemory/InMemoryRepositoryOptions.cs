using Kraken.Module.Common;
using System.Reflection;

namespace Domain.Repository.InMemory;

/// <summary>
/// Datos para la configuracion de los repositorios en memoria
/// </summary>
/// <typeparam name="TModule"></typeparam>
public class InMemoryRepositoryOptions<TModule>
    where TModule : IModule
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
        typeof(TModule).Name.Replace("Module", string.Empty);
}
