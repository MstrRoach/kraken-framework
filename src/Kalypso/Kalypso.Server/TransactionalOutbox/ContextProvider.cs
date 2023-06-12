using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

/// <summary>
/// Componente encargado de administrar el contexto para
/// la bandeja de salida transaccional
/// </summary>
internal sealed class ContextProvider
{
    /// <summary>
    /// Mantiene el contexto para permitir la asignacion de un
    /// contexto en el alcance de la solicitud actual
    /// </summary>
    private ContextHolder Holder = new();

    /// <summary>
    /// Accesor al contexto
    /// </summary>
    public OutboxContext Context
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
        public OutboxContext Context;
    }
}
