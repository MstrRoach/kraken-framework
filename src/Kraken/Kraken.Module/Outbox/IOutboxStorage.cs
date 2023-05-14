using Kraken.Module.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Outbox;

/// <summary>
/// Interface para definir el sistema de almacenamiento para
/// las bandejas de salida
/// </summary>
/// <typeparam name="TModule"></typeparam>
public interface IOutboxStorage<TModule>
where TModule : IModule
{
    /// <summary>
    /// Almacena un mensaje en el store de base de datos
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task Save(StorableMessage message);

    /// <summary>
    /// Obtiene los mensajes no enviados
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<StorableMessage>> GetUnsentAsync();

    /// <summary>
    /// Obtiene un evento por id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<StorableMessage> Get(Guid id);

    /// <summary>
    /// Realiza una limpieza de los eventos processados
    /// </summary>
    /// <returns></returns>
    Task Cleanup();
}
