﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Request.Mediator;

/// <summary>
/// Define el handler para los eventos modulares
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IModuleEventHandler<in TEvent> :
    INotificationHandler<TEvent>
    where TEvent : IModuleEvent
{
}
