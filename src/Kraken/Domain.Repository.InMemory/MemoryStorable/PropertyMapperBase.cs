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
}
