using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.TransactionalOutbox;

/// <summary>
/// Componente encargado de contener informacion acerca de la
/// solicitud actual y la transaccion ademas de centraalizar
/// los eventos que se van desencadenando en las operaciones
/// </summary>
internal class OutboxContext
{
    /// <summary>
    /// Id de la transaccion del contexto
    /// </summary>
    public Guid TransactionId { get; init; } = Guid.Empty;

    /// <summary>
    /// Indica el modulo al cual pertenece la transaccion. 
    /// Solo puede existir una transaccion por modulo.
    /// </summary>
    public string Module { get; init; } = "Kalypso";
}
