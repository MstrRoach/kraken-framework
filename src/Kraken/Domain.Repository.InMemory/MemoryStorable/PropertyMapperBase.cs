namespace Domain.Repository.InMemory.MemoryStorable;

/// <summary>
/// Clase base para obtener informacion de cada propiedad
/// </summary>
public abstract class PropertyMapperBase
{
    /// <summary>
    /// Definicion de creacion de la columna
    /// </summary>
    public abstract string GetColumnDefinition();

    /// <summary>
    /// Nombre de la columna
    /// </summary>
    /// <returns></returns>
    public abstract string GetColumnName();

    /// <summary>
    /// Nombre de la propiedad almacenada
    /// </summary>
    /// <returns></returns>
    public abstract string GetPropertyName();

    /// <summary>
    /// Obtiene el valor para la propiedad buscandola
    /// dentro del objeto pasado por parametro
    /// </summary>
    /// <returns></returns>
    public abstract object GetPropertyValue(object instance);

    /// <summary>
    /// Setea el valor en la instancia para la propiedad mapeada
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="value"></param>
    public abstract void SetPropertyValue(object instance, object value);

    /// <summary>
    /// Indica si una propiedad es autoincrementable
    /// </summary>
    /// <returns></returns>
    public abstract bool IsAutoincrement();
}
