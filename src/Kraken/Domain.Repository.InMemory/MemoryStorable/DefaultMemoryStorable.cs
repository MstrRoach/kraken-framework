using Dapper;
using Domain.Repository.InMemory.TypeMappers;
using Kraken.Domain.Core;
using Kraken.Module.Common;
using Microsoft.Data.Sqlite;
using System.Collections;
using System.Linq;
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
            columnsValue.Add(paramName, columnValue);
        }
        var insertCommand = SqliteQueries.Insert
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
    /// Obtiene todos los elementos para un tipo especifico
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TAggregate> GetAll()
    {
        var columns = String.Join(",",PropertyMappers.Select(x => x.GetColumnName()));
        var selectCommand = SqliteQueries.Select
            .Replace("$TABLE", typeof(TAggregate).Name.ToUpper())
            .Replace("$COLUMNS", columns);
        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.ExecuteReader(selectCommand);
        while (result.Read())
        {
            var rootInstance = Activator.CreateInstance(typeof(TAggregate),true);
            rootInstance = PropertyMappers.Aggregate(
                rootInstance, 
                (instance, mapper) => mapper.SetPropertyValue(instance, result));
            yield return (TAggregate)rootInstance;
        }
        connection.Close();
    }

    /// <summary>
    /// Actualiza un registro
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public TAggregate Update(TAggregate aggregate)
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
            columnsValue.Add(paramName, columnValue);
        }
        // Creamos la lista de actualizacion
        var updateColumn = columnsNameParams
            .Where(x => x.Key != "ID")
            .Select(x => SqliteQueries.ColumnEqual
                .Replace("$COLUMN", x.Key)
                .Replace("$PARAM", x.Value)
            ).ToList();

        var idCondition = columnsNameParams
            .Where(x => x.Key == "ID")
            .Select(x => SqliteQueries.ColumnEqual
                .Replace("$COLUMN", x.Key)
                .Replace("$PARAM", x.Value))
            .FirstOrDefault();
        // Remplazamos
        var updatedCommand = SqliteQueries.Update
            .Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper())
            .Replace("$UPDATES", string.Join(",", updateColumn))
            .Replace("$CONDITION", idCondition);

        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.Execute(updatedCommand, columnsValue);
        connection.Close();
        return aggregate;
    }

    /// <summary>
    /// Elimina un registro del almacen
    /// </summary>
    /// <param name="aggregate"></param>
    /// <returns></returns>
    public TAggregate Delete(TAggregate aggregate)
    {
        // Obtenemos el mapeador del tipo id
        var mapper = PropertyMappers.FirstOrDefault(x => x.GetPropertyName() == "Id");
        // Creamos el nombre del parametro
        var param = "@$PARAM".Replace("$PARAM", mapper.GetPropertyName().ToUpper());
        // Creamos la condicion
        var columnDelete = SqliteQueries.ColumnEqual
            .Replace("$COLUMN", mapper.GetColumnName())
            .Replace("$PARAM", param);
        // Creamos el comando
        var deleteCommand = SqliteQueries.Delete
            .Replace("$TABLE_NAME", typeof(TAggregate).Name.ToUpper())
            .Replace("$CONDITION", columnDelete);
        // Creamos el diccionario con los valores
        var columnsValue = new Dictionary<string, object>();
        // Agregamos el objeto
        columnsValue.Add(param, mapper.GetPropertyValue(aggregate));

        using var connection = new SqliteConnection(GetConnectionString());
        connection.Open();
        var result = connection.Execute(deleteCommand, columnsValue);
        connection.Close();
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
    /// Verifica que la tabla para la entidad actual exista en la base de 
    /// datos para las operaciones de lectura y escritura
    /// </summary>
    private void CheckTableExistence()
    {
        var tableCreation = SqliteQueries.CreateTableCommand<TAggregate>(
                PropertyMappers
                .Select(x => x.GetColumnDefinition())
                .ToList());
        // Coneccion a la base de datos
        var connectionString = GetConnectionString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var result = connection.Execute(tableCreation);
        connection.Close();
    }

    /// <summary>
    /// Verifica que el schema de columnas sea correcto
    /// </summary>
    private void CheckColumnsExistence()
    {
        var columnCheckCommand = SqliteQueries.CheckColumnsCommand<TAggregate>(PropertyMappers.Select(x => x.GetColumnName()).ToList());
        // Coneccion a la base de datos
        var connectionString = GetConnectionString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        // Obtener las columnas faltantes
        var missingColumns = connection.Query<ColumnInfo>(columnCheckCommand);
        connection.Close();
        if (missingColumns.Count() == 0)
            return;
        // Columnas eliminadas
        var deletedColumnCommands = missingColumns
            .Where(x => !x.status)
            .Select(x => SqliteQueries.DropColumnCommand<TAggregate>(x.name))
            .ToList();
        // Columnas para agregar
        var columnCreationCommands = missingColumns
            .Where(x => x.status)
            .Join(
            PropertyMappers,
            missing => missing.name,
            mapper => mapper.GetColumnName(),
            (missing, mapper) => mapper)
            .Select(x => SqliteQueries
            .AddColumnCommand<TAggregate>(x.GetColumnDefinition()))
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
