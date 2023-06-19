using Dottex.Kalypso.Module.TransactionalOutbox;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

/// <summary>
/// Componente que administra la ejecucion del evento y marca
/// los eventos como procesados 
/// </summary>
internal class OutboxProcessor
{
    /// <summary>
    /// Almacen de los eventos despachados
    /// </summary>
    private readonly IOutboxStorage _storage;

    /// <summary>
    /// Proveedor de los servicios de aplicacion
    /// </summary>
    private readonly IOutboxDispatcher _dispatcher;

    public OutboxProcessor(IOutboxStorage storage, IOutboxDispatcher dispatcher)
    {
        _storage = storage;
        _dispatcher = dispatcher;
    }

    public async Task Process(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dispatcher.Process(message, cancellationToken);
            await _storage.Update(
                id: message.Id,
                status: OutboxRecordStatus.Processed,
                sentAt: DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            await _storage.Update(
                id: message.Id,
                status: OutboxRecordStatus.OnError,
                notes: ex.Message);
        }
    }
}
