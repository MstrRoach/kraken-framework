using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dottex.Kalypso.Module.TransactionalReaction;

public class ReactionFilter
{
    /// <summary>
    /// Busqueda por id especifico
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Busqueda por transaccion
    /// </summary>
    public Guid? TransactionId { get; set; }

    /// <summary>
    /// Busqueda por modulo de origen
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Busquedda por modulo de destino
    /// </summary>
    public string? Target { get; set; }

    /// <summary>
    /// Busqueda por estatus
    /// </summary>
    public string? Status { get; set; }

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
    private const string DateFilter = "$Column BETWEEN $From AND $To";
    private const string Pagination = "LIMIT $Size OFFSET $Offset";
    private const string Where = $@"WHERE $Filter $Pagination";

    public string GetFilter()
    {
        var filters = new List<string>();

        var id = Id.HasValue
            ? ColumnFilter
            .Replace("$Column", "Id")
            .Replace("$Value", "@Id")
            : string.Empty;

        var transactionId = TransactionId is not null
            ? ColumnFilter
            .Replace("$Column", "TransactionId")
            .Replace("$Value", "@TransactionId")
            : string.Empty;

        var origin = Origin is not null
            ? ColumnFilter
            .Replace("$Column", "Origin")
            .Replace("$Value", "@Origin")
            : string.Empty;

        var target = Target is not null
            ? ColumnFilter
            .Replace("$Column", "Target")
            .Replace("$Value", "@Target")
            : string.Empty;

        var status = Status is not null
            ? ColumnFilter
            .Replace("$Column", "Status")
            .Replace("$Value", "@Status")
            : string.Empty;

        var date = DateFilter
            .Replace("$Column", "CreatedAt")
            .Replace("$From", "@From")
            .Replace("$To", "@To");

        filters.Add(id);
        filters.Add(transactionId);
        filters.Add(origin);
        filters.Add(target);
        filters.Add(status);
        filters.Add(date);

        var filter = string.Join("\nAND ", filters.Where(x => !string.IsNullOrEmpty(x)));

        var pagination = Pagination
            .Replace("$Size", "@PageSize")
            .Replace("$Offset", "@Offset");

        return Where
            .Replace("$Filter", filter)
            .Replace("$Pagination", pagination);
    }
}
