// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Kraken.Core.Processing;

/// <inheritdoc />
/// <summary>
/// Un hilo de procesamiento abstracto
/// </summary>
public interface IProcessingServer : IDisposable
{
    /// <summary>
    /// Realiza una verificacion para revisar si el
    /// proceso esta vivo
    /// </summary>
    void Pulse() { }

    /// <summary>
    /// Inicia el procesamiento de la operacion
    /// </summary>
    /// <param name="stoppingToken"></param>
    void Start(CancellationToken stoppingToken);
}
