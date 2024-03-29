﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.TransactionalOutbox;

/// <summary>
/// Registro que contiene la informacion contextual y relevante
/// de un evento que debe ser ejecuctado
/// </summary>
public record OutboxMessage(
    Guid Id,
    Guid CorrelationId,
    Guid TransactionId,
    string Origin,
    string User,
    string Username,
    string TraceId,
    object Event
);
