﻿using Dottex.Kalypso.Module.Request.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Request;

/// <summary>
/// Contrato para definir el handler para los eventos
/// de arquitectura
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IArchEventHandler<in TEvent> :
    INotificationHandler<TEvent>
    where TEvent : IArchEvent
{
}
