namespace Domain.Repository.InMemory.TypeMappers;

public interface ITypeMapper<TType>
{
    /// <summary>
    /// Nombre del tipo en la base de datos
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// Indica si el tipo es nulable
    /// </summary>
    bool IsNullable { get; }

    /// <summary>
    /// Indica el valor por defecto
    /// </summary>
    string DefaultValue { get; }

    /// <summary>
    /// Transforma una cadena con datos en la salida
    /// especificada por el transformador
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    TType FromDatabase(string data);

    /// <summary>
    /// Devuelve la representacion en cadena para el 
    /// tipo especificado
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string ToDatabase(TType data);
}
