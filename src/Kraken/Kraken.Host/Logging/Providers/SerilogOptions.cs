using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Logging.Providers;

/// <summary>
/// Opciones para serilog como proveedor de logeo
/// </summary>
public class SerilogOptions
{
    /// <summary>
    /// Indica el nivel minimo de loggeo
    /// </summary>
    public string Level { get; set; }

    /// <summary>
    /// Contiene las sobreeescrituras
    /// </summary>
    public IDictionary<string, string> Overrides { get; set; }

    /// <summary>
    /// Las rutas excluidas
    /// </summary>
    public IEnumerable<string> ExcludePaths { get; set; }

    /// <summary>
    /// Las propiedades excluidas
    /// </summary>
    public IEnumerable<string> ExcludeProperties { get; set; }

    /// <summary>
    /// Las etiquetas
    /// </summary>
    public IDictionary<string, object> Tags { get; set; }

    /// <summary>
    /// Lista de proveedores de logeo
    /// </summary>
    public List<ILoggerProvider> Providers { get; } = new();

    /// <summary>
    /// Agrega un proveedor a la lista para configurarlo
    /// </summary>
    /// <param name="provider"></param>
    public void AddProviderLogger(ILoggerProvider provider)
    {
        this.Providers.Add(provider);
    }
}

/// <summary>
/// Interface para definir a los provedores
/// </summary>
public interface ILoggerProvider
{
    /// <summary>
    /// Configura el proveedor con los valores customizados
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    void Configure(LoggerConfiguration loggerConfiguration);
}
