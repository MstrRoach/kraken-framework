using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Logging.Providers
{
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
        /// Seccion de la configuracion para la configuracion de la applicacion
        /// </summary>
        private const string AppSectionName = "kraken";

        /// <summary>
        /// Seccion de la configuracion para la configuracion del logger
        /// </summary>
        private const string LoggerSectionName = "logger";

        /// <summary>
        /// Utiliza el logging configurado d kraken en el hostbuilder 
        /// con las configuraciones y los enrichters especificados
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loggerSectionName"></param>
        /// <param name="appSectionName"></param>
        /// <returns></returns>
        public static IHostBuilder UseSerilogKrakenLogging(this IHostBuilder builder,
            Action<LoggerConfiguration> loggerSetup,
            string loggerSectionName = LoggerSectionName,
            string appSectionName = AppSectionName)
            => builder.UseSerilog((context, loggerConfiguration) =>
            {
                // Si las secciones no son dadas ponemmos el default
                if (string.IsNullOrWhiteSpace(loggerSectionName))
                    loggerSectionName = LoggerSectionName;

                if (string.IsNullOrWhiteSpace(appSectionName))
                    appSectionName = AppSectionName;
                // Cargamos las configuraciones
                var appOptions = context.Configuration.GetOptions<AppOptions>(appSectionName);
                var loggerOptions = context.Configuration.GetOptions<LoggerOptions>(loggerSectionName);
                loggerSetup(loggerOptions);
                // Enriquecemos los loggers
                MapOptions(loggerOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);

            });
    }
}
