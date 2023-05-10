﻿using Kraken.Standard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Transactions;

/// <summary>
/// Evento base para los eventos de la unidad de trabajo
/// </summary>
public record UnitWorkEventBase : IModuleEvent
{
    /// <summary>
    /// Id del evento
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nombre del componente que genera el evento
    /// </summary>
    public string Component => "UnitWork";

    /// <summary>
    /// Indica la fecha en la que ocurrio el evento
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}