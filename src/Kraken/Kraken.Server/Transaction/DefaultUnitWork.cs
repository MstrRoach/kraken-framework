using Kraken.Module.Server;
using Kraken.Module.Transactions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Server.Transaction;

internal class DefaultUnitWork<TModule> : IUnitWork<TModule>
    where TModule : IModule
{
    /// <summary>
    /// Id de la transaccion
    /// </summary>
    public Guid TransactionId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Registro de eventos internos para logeo
    /// </summary>
    private Stack<string> _transactionLog = new();

    /// <summary>
    /// Lista inmutable de los eventos
    /// </summary>
    public ImmutableList<string> TransactionLog => _transactionLog.ToImmutableList();

    /// <summary>
    /// Inicio de transaccion
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void StartTransaction()
    {
        // Generar nuevo id
        TransactionId = Guid.NewGuid();
        _transactionLog.Push($"[{DateTime.UtcNow}] >>> Transaction started | Id = [{TransactionId.ToString()}]");
    }

    /// <summary>
    /// Confirma la transaccion
    /// </summary>
    /// <returns></returns>
    public Task Commit()
    {
        _transactionLog.Push($"[{DateTime.UtcNow}] >>> Transaction commited | Id = [{TransactionId.ToString()}]");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Revierte la transaccion
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task Rollback()
    {
        _transactionLog.Push($"[{DateTime.UtcNow}] >>> Transaction reverted | Id = [{TransactionId.ToString()}]");
        return Task.CompletedTask;
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        await action();
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        return await action();
    }




}
