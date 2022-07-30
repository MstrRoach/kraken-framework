using Kraken.Core.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Host.Outbox;

/// <summary>
/// Accesor para el contexto de bandeja de salida
/// </summary>
public sealed class OutboxContextAccesor
{

    /// <summary>
    /// Accesor para el contexto de la bandeja de salida
    /// </summary>
    private ContextHolder _holder = new();

    public Guid Id { get; } = Guid.NewGuid();

    public IOutboxContext Context
    {
        get => _holder?.Context;
        set
        {
            if (_holder is not null)
                _holder.Context = null;
            if (value is not null)
                _holder = new ContextHolder { Context = value };
        }
    }

    /// <summary>
    /// Contenedor del contexto
    /// </summary>
    private class ContextHolder
    {
        public IOutboxContext Context;
    }

}
