using Dottex.Kalypso.Module.Audit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Server.Audit;

/// <summary>
/// Controlador para recuperar y filtrar los registros de
/// auditoria a partir de filtros preexistentes
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuditLogsController : ControllerBase
{
    readonly IAuditStorage _storage;
    public AuditLogsController(IAuditStorage storage)
    {
        _storage = storage;
    }
    [HttpGet]
    public IActionResult GetAuditLogs([FromQuery] AuditFilter filter)
    {
        var values = _storage.GetAll(filter);
        return Ok(values);
    }
}
