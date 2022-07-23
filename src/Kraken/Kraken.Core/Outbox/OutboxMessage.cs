﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Outbox;

/// <summary>
/// Clase base para guardar los mensajes en la bandeja de salida
/// transaccional
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Id del mensaje
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id de correlacion del evento
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Usuario que desencadeno el evento
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Nombre del evento
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Tipo del evento
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Datos del evento serializados
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Id de trazabilidad
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// Indica la fecha de creacion
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indica la fecha de procesamiento
    /// </summary>
    public DateTime? SentAt { get; set; }
}
