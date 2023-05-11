using Kraken.Standard.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Outbox;

internal class DefaultOutboxContext : IOutboxContext
{
    /// <summary>
    /// Id de la transaccion del contexto
    /// </summary>
    public Guid TransactionId { get; } = Guid.Empty;

    /// <summary>
    /// Indica el modulo al cual pertenece la transaccion. 
    /// Solo puede existir una transaccion por modulo.
    /// </summary>
    public string Module { get; } = "";

    /// <summary>
    /// Lista de eventos de dominio que deben de enviarse a procesamiento
    /// cuando se confirme la transaccion
    /// </summary>
    private List<OutboxMessage> events = new List<OutboxMessage>();

    /// <summary>
    /// Lista de solo lectura para obtener los eventos de domminio de
    /// la transaccion actual
    /// </summary>
    public IReadOnlyCollection<OutboxMessage> Events => events.AsReadOnly();

    public DefaultOutboxContext(Guid transactionId)
    {
        this.TransactionId = transactionId;
    }

    /// <summary>
    /// Agrega los eventos a la lista de eventos
    /// </summary>
    /// <param name="event"></param>
    public void AddProcessMessage(OutboxMessage @event)
    {
        if (@event is null)
            return;
        events.Add(@event);
    }

    /// <summary>
    /// Limpia la lista de eventos
    /// </summary>
    public void Cleanup()
    {
        this.events.Clear();
    }
}
