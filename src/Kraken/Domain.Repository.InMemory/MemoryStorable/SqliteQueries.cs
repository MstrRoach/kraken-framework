using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository.InMemory.MemoryStorable;

internal class SqliteQueries
{
    public const string TableName = "$TABLE_NAME";

    public const string ColumnDefinition = "$COLUMNS_DEFINITION";

    /// <summary>
    /// Cadena para crear una tabla
    /// </summary>
    public const string CreateTable = $"CREATE TABLE IF NOT EXISTS {TableName}({ColumnDefinition});";

    /// <summary>
    /// Cadena para revissar las columnas
    /// </summary>
    public const string ColumnCheck = 
@"WITH CTE_CurrentCols as(
	SELECT  [name]
	FROM pragma_table_info('$TABLE_NAME')
),
CTE_EntityCols as (
	$COLUMNS_BODY
),
CTE_MissingCols as (
	SELECT EC.name [name], true [status]
	FROM CTE_EntityCols as EC
	LEFT JOIN CTE_CurrentCols as CC ON CC.name = EC.name
	WHERE CC.name IS NULL
),
CTE_DeletedCols as (
	SELECT CC.name [name], false [status]
	FROM CTE_CurrentCols as CC
	LEFT JOIN CTE_EntityCols as EC ON EC.name = CC.name
	WHERE EC.name IS null
)
SELECT name, status
FROM CTE_MissingCols EC
UNION ALL
SELECT name, status
FROM CTE_DeletedCols DC";

    /// <summary>
    /// Cadena para agregar columna a una tabla
    /// </summary>
    public const string AddColumn = "ALTER TABLE $TABLE_NAME ADD COLUMN $COLUMN_DEFINITION;";

    /// <summary>
    /// Cadena para eliminar una columna de la tabla
    /// </summary>
    public const string DeleteColumn = "ALTER TABLE $TABLE_NAME DROP COLUMN $COLUMN_NAME;";

    /// <summary>
    /// Cadena para insertar valores en una tabla
    /// </summary>
    public const string Insert = "INSERT INTO $TABLE_NAME($COLUMNS) VALUES($COLUMN_VALUES);$LAST_INSERTED";

    /// <summary>
    /// Cadena para seleccionar valores de una tabla
    /// </summary>
    public const string Select = "SELECT $COLUMNS FROM $TABLE;";

    /// <summary>
    /// Cadena para actualizar valores de una tabla
    /// </summary>
    public const string Update = "UPDATE $TABLE_NAME SET $UPDATES WHERE $CONDITION";

    /// <summary>
    /// Cadena para poner una columna igualada a un parametro
    /// </summary>
    public const string ColumnEqual = "$COLUMN = $PARAM";

    /// <summary>
    /// Cadena para eliminar un registro de la tabla
    /// </summary>
    public const string Delete = "DELETE FROM $TABLE_NAME WHERE $CONDITION;";

    /// <summary>
    /// Crea el comando para crear una tabla
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnDefinitions"></param>
    /// <returns></returns>
    public static string CreateTableCommand<T>(List<string> columnDefinitions)
    {
        var tableCreation = SqliteQueries.CreateTable
            .Replace(TableName, typeof(T).Name.ToUpper())
            .Replace(ColumnDefinition, string.Join(",", columnDefinitions));
        return tableCreation;
    }

}
