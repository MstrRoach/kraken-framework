﻿using Kraken.Module.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Module.Inbox;

/// <summary>
/// Interface para definir el almacenamiento para las reacciones 
/// por modulos
/// </summary>
public interface IInboxStorage
{
    /// <summary>
    /// Guarda todos los registros pasados en la lista
    /// de una sola vez
    /// </summary>
    /// <param name="records"></param>
    /// <returns></returns>
    Task SaveAll(List<InboxRecord> records);

    /// <summary>
    /// Guarda un registro en los archivos
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    Task Save(InboxRecord record);

    /// <summary>
    /// Actualiza un registro de reaccion con un estado y el payload
    /// de error en caso de ser necesario
    /// </summary>
    /// <param name="id"></param>
    /// <param name="status"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    Task Update(Guid id, InboxRecordStatus status, string error = null);

    /// <summary>
    /// Obtiene las reacciones que deben de procesarse 
    /// de nuevo en caso de que haya errores
    /// </summary>
    /// <returns></returns>
    Task<List<InboxRecord>> GetUnprocessed();
}

/// <summary>
/// Interface para definir almacenes para modulos especificos
/// </summary>
/// <typeparam name="TModule"></typeparam>
public interface IInboxStorage<TModule> : IInboxStorage
where TModule : IModule
{

}