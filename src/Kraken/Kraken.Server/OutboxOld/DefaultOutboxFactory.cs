using Kraken.Module.Outbox;
using Kraken.Module.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.OutboxOld;

internal class DefaultOutboxFactory
{
    /// <summary>
    /// Logger para la fabrica de bandejas de salida
    /// </summary>
    private readonly ILogger<DefaultOutboxFactory> _logger;

    /// <summary>
    /// Definicion de los modulos existentes
    /// </summary>
    private readonly ModuleRegistry _moduleRegistry;

    /// <summary>
    /// Proveedor de los servicios para obtener las baandejas 
    /// de salida registradas
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public DefaultOutboxFactory(ILogger<DefaultOutboxFactory> logger,
        ModuleRegistry moduleRegistry, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _moduleRegistry = moduleRegistry;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Crea la bandeja de entrada para el tipo especificado
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IOutbox Create<T>()
    {
        // Obtenemos el modulo
        var module = _moduleRegistry.Resolve<T>();
        // Creamos el tipo de outbox que debe de recuperarse de los servicios
        //var specificOutboxType = typeof(IOutbox<>).MakeGenericType(module);
        // Obtenemos el servicio
        //var specificOutbox = _serviceProvider.GetService(specificOutboxType);
        // Si no es nulo, lo regresamos
        //if (specificOutbox is not null)
        //    return specificOutbox as IOutbox;
        // Creamos el tipo cerrado para la bandeja de salida por defecto
        var defaultOutboxType = typeof(DefaultOutbox<>).MakeGenericType(module);
        // Obtenemos el servicio
        var defaultOutbox = _serviceProvider.GetService(defaultOutboxType);
        // lo regresamos como bandeja de salida
        return defaultOutbox as IOutbox;
    }
}
