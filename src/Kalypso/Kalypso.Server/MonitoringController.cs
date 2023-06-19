using Dottex.Kalypso.Module.Audit;
using Dottex.Kalypso.Module.TransactionalOutbox;
using Dottex.Kalypso.Module.TransactionalReaction;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server;

/// <summary>
/// Controlador para recuperar y filtrar los registros de
/// auditoria a partir de filtros preexistentes
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MonitoringController : ControllerBase
{
    readonly IAuditStorage _auditStorage;
    readonly IOutboxStorage _outboxStorage;
    readonly IReactionStorage _reactionStorage;
    public MonitoringController(IAuditStorage auditStorage, 
        IOutboxStorage outboxStorage,
        IReactionStorage reactionStorage)
    {
        _auditStorage = auditStorage;
        _outboxStorage = outboxStorage;
        _reactionStorage = reactionStorage;
    }

    /// <summary>
    /// Acceso a la informacion de auditoria
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet("Audit")]
    public IActionResult GetAuditLogs([FromQuery] AuditFilter filter)
    {
        var values = _auditStorage.GetAll(filter);
        return Ok(values);
    }

    /// <summary>
    /// Acceso a la informacion de bandeja de salida
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet("Outbox")]
    public IActionResult GetOutboxRecords([FromQuery] OutboxFilter filter)
    {
        var values = _outboxStorage.GetBy(filter);
        return Ok(values);
    }

    /// <summary>
    /// Acceso a la informacion de reacciones
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet("Reaction")]
    public IActionResult GetReactionRecords([FromQuery] ReactionFilter filter)
    {
        var values = _reactionStorage.GetBy(filter);
        return Ok(values);
    }
}
