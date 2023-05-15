using Kraken.Module.Outbox;
using Kraken.Module.Server;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.OutboxOld;

public class DefaultOutboxStorageFactory
{
    /// <summary>
    /// Proveedor de los servicios
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor de la fabrica de almacenadores
    /// </summary>
    /// <param name="serviceProvider"></param>
    public DefaultOutboxStorageFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Crea una instancia del outbox storage, ya sea definida por el
    /// cliente para el modulo o la instancia por defecto en memoria
    /// </summary>
    /// <typeparam name="TModule"></typeparam>
    /// <returns></returns>
    public IOutboxStorage<TModule> Create<TModule>()
        where TModule : IModule
    {
        // Creamos el tipo para el servicio especifico
        var specificOutboxStorageType = typeof(IOutboxStorage<TModule>);
        // Obtenemos el storage si esta registrado
        var specificOutboxStorage = _serviceProvider.GetService(specificOutboxStorageType);
        // Si no es nula, la devolvemos
        if (specificOutboxStorage is not null)
            return specificOutboxStorage as IOutboxStorage<TModule>;
        // Creamos el tipo por default para el storage
        var defaultOutboxStorageType = typeof(DefaultOutboxStorage<TModule>);
        // Obtenemos el servicio
        var defaultOutboxStorage = _serviceProvider.GetService(defaultOutboxStorageType);
        // Lo devolvemos
        return defaultOutboxStorage as IOutboxStorage<TModule>;
    }
}
