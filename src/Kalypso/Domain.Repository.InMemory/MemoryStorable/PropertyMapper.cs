using Dottex.Domain.Repository.InMemory.TypeMappers;
using System.Data;

namespace Dottex.Domain.Repository.InMemory.MemoryStorable;

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
    /// Indica si una columna es autoincrementable
    /// </summary>
    /// <returns></returns>
    public override bool IsAutoincrement()
    {
        if (!TypeMapper.IsAutoIncrement)
            return false;
        if (Name != "Id")
            return false;
        return true;
    }

    /// <summary>
    /// Template para la definicion de columnas
    /// </summary>
    private string _columnDefinition = "$COLUMN_NAME $COLUMN_TYPE $KEY_DEFINITION $COLUMN_AUTOINCREMENT $COLUMN_NULLITY $COLUMN_DEFAULT ";

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
        var columnDefinition = IsAutoincrement()
            ? _columnDefinition
                .Replace("$COLUMN_NULLITY", string.Empty)
                .Replace("$COLUMN_DEFAULT", string.Empty)
            : _columnDefinition
                .Replace("$COLUMN_AUTOINCREMENT", string.Empty);
        return columnDefinition.Replace("$COLUMN_NAME", columnName)
            .Replace("$COLUMN_TYPE", TypeMapper.TypeName)
            .Replace("$COLUMN_AUTOINCREMENT", IsAutoincrement() ? "AUTOINCREMENT" : string.Empty)
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

    /// <summary>
    /// Devuelve el nombre de la propiedad
    /// </summary>
    /// <returns></returns>
    public override string GetPropertyName()
        => Name;

    /// <summary>
    /// Obtiene el valor de la propiedad a traves de la recursion
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public override object GetPropertyValue(object instance)
    {
        if (instance is null)
            return instance;
        // Si el tipo del contenedor es el mismo que el tipo de
        // la instancia obtenemos el valor a partir de la propiedad
        if (instance.GetType() == Container)
            return Container.GetProperty(Name).GetValue(instance);
        // Buscamos el valor del contenedor en el objeto de instancia
        return GetPropertyValue(
            instance
            .GetType()
            .GetProperty(Container.Name)
            .GetValue(instance)
            );
    }

    /// <summary>
    /// Settea el valor en la instancia para la propiedad actual 
    /// especificada
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override object SetPropertyValue(object instance, object value)
    {
        // Si el contenedor es el mismo que de la instancia
        if (instance.GetType() == Container)
        {
            // Seteamos el valor dentro de aqui
            Container.GetProperty(Name).SetValue(instance, TypeMapper.FromDatabase(value));
            return instance;
        }
        // Obtenemos la instaancia del contenedor
        var containerInstance = instance
            .GetType()
            .GetProperty(Container.Name)
            .GetValue(instance);
        // Si el contenedor es nulo
        if (containerInstance is null)
        {
            // Creamos la instancia
            containerInstance = Activator.CreateInstance(Container, true);
        }
        // Mandamos a settear de nuevo la propiedad
        SetPropertyValue(containerInstance, value);
        // Setteamos el contenedor dentro de la instancia padre
        instance.GetType()
            .GetProperty(Container.Name)
            .SetValue(instance, containerInstance);
        return instance;
    }

    public override object SetPropertyValue(object instance, IDataReader value)
        => SetPropertyValue(instance, value[GetColumnName()]);
}