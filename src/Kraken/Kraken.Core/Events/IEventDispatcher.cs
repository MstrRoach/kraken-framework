using Kraken.Core.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Events;

/// <summary>
/// Componente encargado de recibir eventos, colocarlos en listas de
/// espera hasta que un evento de la unidad de trabajo le indique cuales
/// eventos tienen que ser puestos en cola de procesamiento y cuales
/// deben de removerse de los canales por que sus procesos fallaron
/// </summary>
public interface IEventDispatcher : IProcessingServer
{
    /// <summary>
    /// Agrega un evento a la lista de espera junto con el id de la
    /// transaccion la cual origino
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    Task AddToWaitingList(IKrakenEvent @event);

    /// <summary>
    /// Indica que una transaccion fue completada
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    Task TransactionCompleted(Guid transactionId);

    /// <summary>
    /// Indica que una transaccion fallo
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    Task TransactionFailed(Guid transactionId);


}
