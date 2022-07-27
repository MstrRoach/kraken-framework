﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Core.Reaction
{
    /// <summary>
    /// Administra el procesamiento de los registros 
    /// de reaccion que se procesan y que se ejecutan
    /// </summary>
    public interface IReactionLog
    {
        /// <summary>
        /// Proceso y guarda la reaccion envuelta en el objeto 
        /// de registro de proceso
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        Task SaveAsync(ProcessRecord record);
    }
}
