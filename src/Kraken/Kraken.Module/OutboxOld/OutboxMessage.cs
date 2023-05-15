using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.OutboxOld;

/// <summary>
/// Define un mensaje utilizado para el procesamiento
/// de los eventos que envuelven. Esto, para conservar y transaportar
/// detalles del contexto sin la necesidad de extrapolar un mensaje de
/// almacenamiento fuera de su campo de accion
/// </summary>
public record OutboxMessage(Guid Id, Guid CorrelationId, string UserId,
    string Username, string TraceId, object Event);
