using Kraken.Standard.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Middlewares.Contexts;

/// <summary>
/// Contiene y administra los contextos a partir de las solicitudes
/// </summary>
public sealed class ContextProvider
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    /// <summary>
    /// Accesor al contextoo
    /// </summary>
    public IContext Context
    {
        get => Holder.Value?.Context;
        set
        {
            // Get current context holder value
            var holder = Holder.Value;
            // If holder exist, remove context
            if (holder != null)
            {
                holder.Context = null;
            }
            // If new context is present, set it 
            if (value != null)
            {
                Holder.Value = new ContextHolder { Context = value };
            }
        }
    }

    private record ContextHolder
    {
        public IContext Context;
    }
}
