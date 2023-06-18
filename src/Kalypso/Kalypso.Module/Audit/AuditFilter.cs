using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.Audit;

/// <summary>
/// Contiene los filtros por los que se pueden buscar
/// dentro del almacen de auditoria
/// </summary>
public sealed class AuditFilter
{
    /// <summary>
    /// Id especifico
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Modulo especifico
    /// </summary>
    public string Module { get; set; }

    /// <summary>
    /// Id de entidad especifico
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// Entidad especifica
    /// </summary>
    public string Entity { get; set; }

    /// <summary>
    /// Operacion especifica
    /// </summary>
    public string Operation { get; set; }

    /// <summary>
    /// Usuario especifico
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Desde una fecha especifica, por default es la mas minima
    /// </summary>
    public DateTime From { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Hasta una fecha especifica, por default la mas actual
    /// </summary>
    public DateTime To { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica que pagina retornar
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Indica el tamaño de la pagina a devolver
    /// </summary>
    public int PageSize { get; set; } = 10;

    private const string ColumnFilter = "$Column = $Value";
    private const string DateFilter = "BETWEEN $From AND $To";
    private const string Pagination = "LIMIT $Size OFFSET $Offset";
    private const string Where = $@"
    WHERE $Filter $Pagination";

    public string GetFilter()
    {
        var filters = new List<string>();
        
        var id = Id.HasValue
            ? ColumnFilter
            .Replace("$Column", "Id")
            .Replace("$Value", Id.ToString())
            : string.Empty;
        
        var module = Module is not null
            ? ColumnFilter
            .Replace("$Column", "Module")
            .Replace("$Value", Module)
            : string.Empty;
        
        var entityId = EntityId is not null
            ? ColumnFilter
            .Replace("$Column", "EntityId")
            .Replace("$Value", EntityId)
            : string.Empty;

        var entity = Entity is not null
            ? ColumnFilter
            .Replace("$Column", "Entity")
            .Replace("$Value", Entity)
            : string.Empty;

        var operation = Operation is not null
            ? ColumnFilter
            .Replace("$Column", "Operation")
            .Replace("$Value", Operation)
            : string.Empty;

        var user = User is not null
            ? ColumnFilter
            .Replace("$Column", "User")
            .Replace("$Value", User)
            : string.Empty;

        var date = DateFilter
            .Replace("$From", From.ToString())
            .Replace("$To", To.ToString());

        filters.Add(id);
        filters.Add(module);
        filters.Add(entityId);
        filters.Add(entity);
        filters.Add(operation);
        filters.Add(date);
        filters.Add(user);

        var filter = string.Join("\nAND ", filters.Where(x => !string.IsNullOrEmpty(x)));

        var pagination = Pagination
            .Replace("$Size", PageSize.ToString())
            .Replace("$Offset", (Page * PageSize).ToString());

        return Where
            .Replace("$Filter", filter)
            .Replace("$Pagination", pagination);
    }
}
