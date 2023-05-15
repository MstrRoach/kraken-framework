using Kraken.Module.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.OutboxOld;

/// <summary>
/// Proveedor de contexto asincrono para el control de la
/// bandeja de salida transaccional
/// </summary>
internal sealed class OutboxContextProvider
{
    /// <summary>
    /// Mantiene el contexto a traves de hilos de ejecucion
    /// </summary>
    private ContextHolder Holder = new();

    /// <summary>
    /// Accesor al contexto
    /// </summary>
    public IOutboxContext Context
    {
        get => Holder.Context;
        set
        {
            // Get current context holder value
            var holder = Holder;
            // If holder exist, remove context
            if (holder != null)
            {
                holder.Context = null;
            }
            // If new context is present, set it 
            if (value != null)
            {
                Holder = new ContextHolder { Context = value };
            }
        }
    }

    /// <summary>
    /// Contenedor del contexto
    /// </summary>
    /// <param name="Context"></param>
    private record ContextHolder
    {
        public IOutboxContext Context;
    }
}
