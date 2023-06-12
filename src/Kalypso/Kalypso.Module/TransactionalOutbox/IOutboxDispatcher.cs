using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.TransactionalOutbox;

/// <summary>
/// Define el procesamiento de los eventos, de forma
/// definida por quien implementa la operacion
/// </summary>
public interface IOutboxDispatcher
{
    /// <summary>
    /// Processa un mensaje y los distribuye para realizar las operaciones
    /// pendientes para el evento
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Process(OutboxMessage message, CancellationToken cancellationToken = default);
}
