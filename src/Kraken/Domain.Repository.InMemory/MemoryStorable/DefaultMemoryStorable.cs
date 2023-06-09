using Dapper;
using Domain.Repository.InMemory.TypeMappers;
using Kraken.Domain.Core;
using Kraken.Module.Common;
using Microsoft.Data.Sqlite;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Domain.Repository.InMemory.MemoryStorable;

/// <summary>
/// Implementacion por defecto para el accesor en memoria a la
/// base de datos
/// </summary>
/// <typeparam name="TModule"></typeparam>
/// <typeparam name="TAggregate"></typeparam>
public class DefaultMemoryStorable<TModule, TAggregate> : IMemoryStorable<TModule>
    where TModule : IModule
    where TAggregate : IAggregate
{
    /// <summary>
    /// Opciones de configuracion para el almacenamiento
    /// </summary>
    readonly InMemoryRepositoryOptions<TModule> _options;

    /// <summary>
    /// Mapeos de cada propiedad
    /// </summary>
    private List<PropertyMapperBase> PropertyMappers = new List<PropertyMapperBase>();

    public DefaultMemoryStorable(InMemoryRepositoryOptions<TModule> options)
    {
        _options = options;
        var aggregateType = typeof(TAggregate);
        PropertyMappers = GetPropertyMappers(aggregateType, aggregateType.GetProperties());
    }

    private string _insertCommand = @"INSERT INTO $TABLE_NAME($COLUMNS) VALUES($COLUMN_VALUES);$LAST_INSERTED";

    /// <summary>
    /// Agrega al almmaacen una entidad del tipo agregado
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public TAggregate Add(TAggregate aggregate)
    {
        // Creacion de los contenedores para las columnas, parametros de
        // columnas y valores de columnas
        var columnsNameParams = new SortedList<string, string>();
        // Contenedor de los valores para parametros
        var columnsValue = new Dictionary<string, object>();
        // Recorremos lass propiedades mapeadas
        foreach (var mapper in PropertyMappers)
        {
            if (mapper.IsAutoincrement())
                continue;
            var paramName = "@$PARAM".Replace("$PARAM", mapper.GetPropertyName().ToUpper());
            columnsNameParams.Add(
                mapper.GetColumnName(),
                paramName
                );
            var columnValue = mapper.GetPropertyValue(aggregate);
            columnsValue.Add(paramName,columnValue);
        }
        var insertCommand = _insertCommand
            .Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper())
            .Replace("$COLUMNS", string.Join(",", columnsNameParams.Keys))
            .Replace("$COLUMN_VALUES", string.Join(",", columnsNameParams.Values))
            .Replace("$LAST_INSERTED", columnsNameParams.ContainsKey("ID") 
                ? string.Empty : 
                "SELECT LAST_INSERT_ROWID();"); 
        
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        int result = columnsNameParams.ContainsKey("ID")
            ? connection.Execute(insertCommand, columnsValue)
            : connection.ExecuteScalar<int>(insertCommand, columnsValue);
        connection.Close();
        if (result == 0)
            throw new Exception("Error to create aggregate");
        if (columnsNameParams.ContainsKey("ID"))
            return aggregate;
        PropertyMappers
            .First(x => x.GetPropertyName() == "Id")
            .SetPropertyValue(aggregate, result);
        return aggregate;
    }

    /// <summary>
    /// Inicializa el esquema de la base de datos
    /// </summary>
    /// <returns></returns>
    public Task Initialize()
    {
        var aggregateType = typeof(TAggregate);
        PropertyMappers = GetPropertyMappers(aggregateType, aggregateType.GetProperties());
        CheckTableExistence();
        CheckColumnsExistence();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene los mapeos de las propiedades para la entidad
    /// actual
    /// </summary>
    /// <param name="container"></param>
    /// <param name="properties"></param>
    /// <param name="isNested"></param>
    /// <returns></returns>
    public List<PropertyMapperBase> GetPropertyMappers(
        Type container,
        PropertyInfo[] properties,
        bool isNested = false)
    {
        // Lista de propiedades mapeadas
        List<PropertyMapperBase> propertyMappers = new();
        // Tipo abierto del mapeador de propiedad
        var propertyMapperOpenType = typeof(PropertyMapper<>);
        // Proceso de iteracion
        foreach (var prop in properties)
        {
            // Si no se puede leer o escribir no lo mapeamos
            if (!prop.CanRead || !prop.CanWrite)
                continue;
            // Si no es tipo primitivo se mete a recursion
            if (!IsMapperType(prop.PropertyType))
            {
                propertyMappers.AddRange(
                    GetPropertyMappers(
                        prop.PropertyType,
                        prop.PropertyType.GetProperties(),
                        true
                        )
                    );
                continue;
            }
            // Creamos el tipo de la implementacion del property mapper
            var propertyMapperType = propertyMapperOpenType.MakeGenericType(prop.PropertyType);
            // Creamos la instancia del property mapper cerrado
            var propertyMapper = Activator.CreateInstance(
                propertyMapperType,
                container,
                isNested,
                prop.Name,
                MapperRelation) as PropertyMapperBase;
            // La agregamos a la lista de propiedades
            propertyMappers.Add(propertyMapper);
        }
        // Regresamos la lista de propiedades mapeadas
        return propertyMappers;
    }

    /// <summary>
    /// Indica si un tipo tiene un mapeador de tipo o requiere
    /// ser analizado para descomponer las propiedades
    /// internas
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool IsMapperType(Type type)
    {
        var interfaceMapper = typeof(ITypeMapper<>).MakeGenericType(type);
        return MapperRelation.ContainsKey(interfaceMapper);
    }

    /// <summary>
    /// Relacion de todas los tipos de objetos net y sus mapeadores
    /// para sqlite
    /// </summary>
    private Dictionary<Type, object> MapperRelation = new()
    {
        { typeof(ITypeMapper<Guid>), new GuidMapper() },
        { typeof(ITypeMapper<int>), new IntMapper() },
        { typeof(ITypeMapper<string>), new StringMapper() },
        { typeof(ITypeMapper<DateTime>), new DatetimeMapper() },
        { typeof(ITypeMapper<bool>), new BooleanMapper() },
    };

    /// <summary>
    /// Comando de creacion
    /// </summary>
    private string _createCommand = @"CREATE TABLE IF NOT EXISTS $TABLE_NAME($COLUMNS_DEFINITION);";

    /// <summary>
    /// Verifica que la tabla para la entidad actual exista en la base de 
    /// datos para las operaciones de lectura y escritura
    /// </summary>
    private void CheckTableExistence()
    {
        var tableCreation = _createCommand
            .Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper());
        var columnDefinitions = PropertyMappers.Select(x => x.GetColumnDefinition());
        var columnDefinitionBody = string.Join(',', columnDefinitions);
        tableCreation = tableCreation.Replace("$COLUMNS_DEFINITION", columnDefinitionBody);
        // Coneccion a la base de datos
        var connectionString = GetConnectionString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var result = connection.Execute(tableCreation);
        connection.Close();
    }

    /// <summary>
    /// Commando para obtener informacion de las columnas de la tabla
    /// </summary>
    private string _columnInfoCommand = @"
WITH CTE_CurrentCols as(
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
    /// Template para agregar una columna
    /// </summary>
    private string _columnAddCommand = "ALTER TABLE $TABLE_NAME ADD COLUMN $COLUMN_DEFINITION;";

    /// <summary>
    /// Template para eliminar una columna
    /// </summary>
    private string _columnDeleteCommand = "ALTER TABLE $TABLE_NAME DROP COLUMN $COLUMN_NAME;";

    /// <summary>
    /// Verifica que el schema de columnas sea correcto
    /// </summary>
    private void CheckColumnsExistence()
    {
        var columnInfo = _columnInfoCommand.Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper());
        var columnList = PropertyMappers
            .Select(x => "SELECT '%COLUMN' as [name]".Replace("%COLUMN", x.GetColumnName()));
        var columnBody = string.Join("\nUNION ALL\n", columnList);
        columnInfo = columnInfo.Replace("$COLUMNS_BODY", columnBody);
        // Coneccion a la base de datos
        var connectionString = GetConnectionString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        // Obtener las columnas faltantes
        var missingColumns = connection.Query<ColumnInfo>(columnInfo);
        connection.Close();
        if (missingColumns.Count() == 0)
            return;
        // Columnas eliminadas
        var deletedColumnCommands = missingColumns
            .Where(x => !x.status)
            .Select(x => _columnDeleteCommand
                .Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper())
                .Replace("$COLUMN_NAME", x.name.ToUpper()))
            .ToList();
        // Columnas para agregar
        var columnCreationCommands = missingColumns
            .Where(x => x.status)
            .Join(
            PropertyMappers,
            missing => missing.name,
            mapper => mapper.GetColumnName(),
            (missing, mapper) => mapper)
            .Select(x => _columnAddCommand
                .Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper())
                .Replace("$COLUMN_DEFINITION", x.GetColumnDefinition()))
            .Concat(deletedColumnCommands)
            .ToList();
        connection.Open();
        // Ejecucion de los comandos
        foreach (var command in columnCreationCommands)
        {
            var result = connection.Execute(command);
        }
        connection.Close();
    }

    private record ColumnInfo
    {
        public string name { get; set; }
        public bool status { get; set; }
    }

    /// <summary>
    /// Construye la cadena de conexion para la base de datos
    /// del modulo
    /// </summary>
    /// <returns></returns>
    private string GetConnectionString()
    {
        var database = "$DATABASE.db".Replace("$DATABASE", _options.DatabaseName);
        var path = Path.Combine(
            _options.ApplicationData,
            _options.ApplicationName,
            database
            );
        return "Data Source=$DATABASE_PATH;".Replace("$DATABASE_PATH", path);
    }

}
