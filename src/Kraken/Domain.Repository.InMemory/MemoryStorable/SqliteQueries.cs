using Microsoft.AspNetCore.Http;
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

    public const string Column = "$COLUMN_NAME";

    public const string Condition = "$CONDITION";

    /// <summary>
    /// Cadena para crear una tabla
    /// </summary>
    public const string CreateTable = $"CREATE TABLE IF NOT EXISTS {TableName}({ColumnDefinition});";

    /// <summary>
    /// Cadena para revissar las columnas
    /// </summary>
    public const string ColumnCheck = 
$@"WITH CTE_CurrentCols as(
	SELECT  [name]
	FROM pragma_table_info('{TableName}')
),
CTE_EntityCols as (
	{ColumnDefinition}
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
    public const string AddColumn = $"ALTER TABLE {TableName} ADD COLUMN {ColumnDefinition};";

    /// <summary>
    /// Cadena para eliminar una columna de la tabla
    /// </summary>
    public const string DeleteColumn = $"ALTER TABLE {TableName} DROP COLUMN {Column};";

    /// <summary>
    /// Cadena para insertar valores en una tabla
    /// </summary>
    public const string Insert = "INSERT INTO $TABLE_NAME($COLUMNS) VALUES($COLUMN_VALUES);$LAST_INSERTED";

    /// <summary>
    /// Plantilla para hacer una seleccion de una columna especifica
    /// </summary>
    public const string ColumnParam = "SELECT '$COLUMN' as [name]";

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
    public const string Delete = $"DELETE FROM {TableName} WHERE {Condition};";

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

    /// <summary>
    /// Construye el comando para revisar que una tabla contenga todas
    /// las columnas necesarias
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnList"></param>
    /// <returns></returns>
    public static string CheckColumnsCommand<T>(List<string> columnList)
    {
        // Creamos la lista de columnas a verificar
        var columnParamList = columnList
            .Select(x => ColumnParam.Replace("$COLUMN", x))
            .ToList();
        // Creamos el comando para revisar el schema de columnas
        var columnCheckCommand = ColumnCheck
            .Replace(TableName, typeof(T).Name.ToUpper())
            .Replace(ColumnDefinition, string.Join("\nUNION ALL\n", columnParamList));
        // retornamos el comando
        return columnCheckCommand;
    }

    /// <summary>
    /// Crea el comando para eliminar una columna
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnName"></param>
    /// <returns></returns>
    public static string DropColumnCommand<T>(string columnName)
    {
        return DeleteColumn
            .Replace(TableName, typeof(T).Name.ToUpper())
            .Replace(Column, columnName);
    }

    /// <summary>
    /// Crea el comando para agregar una columna a una tabla determinada
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="columnDefinition"></param>
    /// <returns></returns>
    public static string AddColumnCommand<T>(string columnDefinition)
    {
        return AddColumn
            .Replace(TableName, typeof(T).Name.ToUpper())
            .Replace(ColumnDefinition, columnDefinition);
    }
}
