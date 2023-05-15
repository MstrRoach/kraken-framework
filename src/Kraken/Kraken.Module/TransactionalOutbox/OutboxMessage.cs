using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.TransactionalOutbox;

/// <summary>
/// Registro que contiene la informacion contextual y relevante
/// de un evento que debe ser ejecuctado
/// </summary>
public record OutboxMessage(
    Guid Id, 
    Guid CorrelationId, 
    Guid TransactionId, 
    string origin, 
    string User, 
    string Username, 
    string traceId, 
    object Event
);
