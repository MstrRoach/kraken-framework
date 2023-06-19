using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Processing;

/// <summary>
/// Interface para definir un arrancador especial para
/// las operaciones necesarias de una funcionalidad 
/// especifica
/// </summary>
public interface IInitializer
{
    /// <summary>
    /// Inicia el procesamiento de configuracion
    /// </summary>
    /// <returns></returns>
    Task Run();
}
