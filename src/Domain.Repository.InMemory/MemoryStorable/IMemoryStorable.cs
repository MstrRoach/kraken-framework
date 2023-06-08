using Kraken.Module.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository.InMemory.MemoryStorable;

/// <summary>
/// Interface para definir los elementos almacenables
/// </summary>
/// <typeparam name="TModule"></typeparam>
public interface IMemoryStorable<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Inicializa el almacenamiento en memoria
    /// </summary>
    Task Initialize();
}