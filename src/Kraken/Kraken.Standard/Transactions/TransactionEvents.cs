using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraken.Standard.Transactions;

/// <summary>
/// Evento para indicar que una transaccion ha sido iniciada
/// </summary>
public record TransactionStarted(Guid TransactionId) : UnitWorkEventBase;

/// <summary>
/// Indica que una transaccion fue finalizada
/// correctammente
/// </summary>
public record TransactionCommited(Guid TransactionId) : UnitWorkEventBase;

/// <summary>
/// Indica que una transaccion fallo y que se revirtio
/// </summary>
public record TransacctionFailed(Guid TransactionId) : UnitWorkEventBase:
