using Kraken.Module.TransactionalOutbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.TransactionalOutbox;

/// <summary>
/// Componente encargado de contener informacion acerca de la
/// solicitud actual y la transaccion ademas de centraalizar
/// los eventos que se van desencadenando en las operaciones
/// </summary>
internal class OutboxContext
{
    /// <summary>
    /// Id de la transaccion del contexto
    /// </summary>
    public Guid TransactionId { get; init; } = Guid.Empty;

    /// <summary>
    /// Indica el modulo al cual pertenece la transaccion. 
    /// Solo puede existir una transaccion por modulo.
    /// </summary>
    public string Module { get; init; } = "Kraken";

    /// <summary>
    /// Lista de eventos de dominio que deben de enviarse a procesamiento
    /// cuando se confirme la transaccion
    /// </summary>
    private List<OutboxMessage> events = new List<OutboxMessage>();

    /// <summary>
    /// Lista de solo lectura de los eventos almacenados dentro del contexto
    /// </summary>
    public IReadOnlyCollection<OutboxMessage> Events => events.AsReadOnly();

    /// <summary>
    /// Agrega el evento a la lista de eventos del contexto
    /// </summary>
    /// <param name="event"></param>
    public void AddEventMessage(OutboxMessage @event)
    {
        if (@event is null)
            return;
        events.Add(@event);
    }

    /// <summary>
    /// Limpia la lista de eventos
    /// </summary>
    public void Cleanup() => events.Clear();
}
