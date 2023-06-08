using Domain.Repository.InMemory.TypeMappers;

namespace Domain.Repository.InMemory.MemoryStorable;

/// <summary>
/// Conserva informacion relevante acerca de una propiedad
/// </summary>
/// <typeparam name="T"></typeparam>
public class PropertyMapper<T> : PropertyMapperBase
{
    /// <summary>
    /// Indica quien es el contenedor del campo, para
    /// indicar a que instancia pertenece
    /// </summary>
    public Type Container { get; set; }

    /// <summary>
    /// Nombre de la propiedad
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indica si el tipo es anidado o es el principal
    /// </summary>
    public bool IsNested { get; set; }

    /// <summary>
    /// Mapeador del tipo de propiedad
    /// </summary>
    public ITypeMapper<T> TypeMapper { get; set; }

    /// <summary>
    /// Indica si el registro puede ser nulo o no
    /// </summary>
    public bool IsNullable { get; set; } = default(T) is null;

    /// <summary>
    /// Template para la definicion de columnas
    /// </summary>
    private string _columnDefinition = "$COLUMN_NAME $COLUMN_TYPE $COLUMN_NULLITY $COLUMN_DEFAULT $KEY_DEFINITION";

    /// <summary>
    /// Constructor del mapeador de propiedad especifico
    /// </summary>
    /// <param name="name"></param>
    /// <param name="mapperRelation"></param>
    public PropertyMapper(Type container, bool isNested, string name,
        Dictionary<Type, object> mapperRelation)
    {
        Container = container;
        IsNested = isNested;
        Name = name;
        TypeMapper = mapperRelation.TryGetValue(typeof(ITypeMapper<T>), out var mapper)
            ? mapper as ITypeMapper<T>
            : null;
    }

    /// <summary>
    /// Devuelve la definicion de columna
    /// </summary>
    /// <returns></returns>
    public override string GetColumnDefinition()
    {
        var columnName = IsNested
            ? $"{Container.Name.Replace("Module", string.Empty).ToUpper()}_{Name.ToUpper()}"
            : Name.ToUpper();
        return _columnDefinition.Replace("$COLUMN_NAME", columnName)
            .Replace("$COLUMN_TYPE", TypeMapper.TypeName)
            .Replace("$COLUMN_NULLITY", TypeMapper.IsNullable ? "NULL" : "NOT NULL")
            .Replace("$COLUMN_DEFAULT", TypeMapper.IsNullable ? string.Empty : TypeMapper.DefaultValue)
            .Replace("$KEY_DEFINITION", Name == "Id" ? "PRIMARY KEY" : string.Empty);
    }

    /// <summary>
    /// Devuelve el nommbre de la columna
    /// </summary>
    /// <returns></returns>
    public override string GetColumnName()
    {
        return IsNested
            ? $"{Container.Name.Replace("Module", string.Empty).ToUpper()}_{Name.ToUpper()}"
            : Name.ToUpper();
    }
}