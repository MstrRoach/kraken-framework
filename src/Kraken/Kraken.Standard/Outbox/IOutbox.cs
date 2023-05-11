using Kraken.Standard.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Outbox;

public interface IOutbox
{
    /// <summary>
    /// Guarda un mensaje en la base de datos
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    Task Save(OutboxMessage messages);

    /// <summary>
    /// Publica los mensajes que no han sido publicados
    /// </summary>
    /// <returns></returns>
    Task PublishUnsent();

    /// <summary>
    /// Limpia la bandeja de salida para tener mas 
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    Task Cleanup(DateTime? to = null);

    /// <summary>
    /// Obtiene un mensaje de processamiento que coincida con el
    /// id especificado
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<OutboxMessage> Get(Guid id);
}

/// <summary>
/// Define la interface para especificar bandejas de salida 
/// por modulos
/// </summary>
/// <typeparam name="TModule"></typeparam>
public interface IOutbox<TModule> : IOutbox
where TModule : IModule
{

}