using Kraken.Host;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenExample.Features;

public static class SerilogProviderExtensions
{
    /// <summary>
    /// Plantilla para mostrar la salida por consola
    /// </summary>
    private const string ConsoleOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}";

    /// <summary>
    /// Platilla para escribir la salida en los archivos
    /// </summary>
    private const string FileOutputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";

    /// <summary>
    /// Utiliza el logging configurado d kraken en el hostbuilder 
    /// con las configuraciones y los enrichters especificados
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="loggerSectionName"></param>
    /// <param name="appSectionName"></param>
    /// <returns></returns>
    public static IHostBuilder UseSerilogKrakenLogging(this IHostBuilder builder,
        Action<LoggerSinkConfiguration> providersConfigure,
        Action<LoggerConfiguration> loggerConfigure = null)
        => builder.UseSerilog((context, loggerConfiguration) =>
        {
            // Cargamos las configuraciones
            var appOptions = context.Configuration.GetNamedSection<AppOptions>();
            var loggerOptions = context.Configuration.GetNamedSection<LoggerOptions>();
            // Enriquecemos los loggers
            MapOptions(loggerOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
            providersConfigure(loggerConfiguration.WriteTo);
            loggerConfigure?.Invoke(loggerConfiguration);
        });

    /// <summary>
    /// Mapea las configuraciones de las opciones custom con las opciones 
    /// de serilog
    /// </summary>
    /// <param name="loggerOptions"></param>
    /// <param name="appOptions"></param>
    /// <param name="loggerConfiguration"></param>
    /// <param name="environmentName"></param>
    private static void MapOptions(LoggerOptions loggerOptions, AppOptions appOptions,
        LoggerConfiguration loggerConfiguration, string environmentName)
    {
        var level = GetLogEventLevel(loggerOptions.Level);

        loggerConfiguration.Enrich.FromLogContext()
            .MinimumLevel.Is(level)
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", appOptions.Name)
            .Enrich.WithProperty("Instance", appOptions.Instance)
            .Enrich.WithProperty("Version", appOptions.Version);

        foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
        {
            loggerConfiguration.Enrich.WithProperty(key, value);
        }

        foreach (var (key, value) in loggerOptions.Overrides ?? new Dictionary<string, string>())
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

        loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));

        //Configure(loggerConfiguration, loggerOptions);
    }

    /// <summary>
    /// Obtiene el nivel del logeo para la aplicacion
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static LogEventLevel GetLogEventLevel(string level)
        => Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;
}

/// <summary>
/// Configuraciones para configurar el logger
/// </summary>
public class LoggerOptions
{
    public string Level { get; set; }
    public IDictionary<string, string> Overrides { get; set; }
    public IEnumerable<string> ExcludePaths { get; set; }
    public IEnumerable<string> ExcludeProperties { get; set; }
    public IDictionary<string, object> Tags { get; set; }
}
