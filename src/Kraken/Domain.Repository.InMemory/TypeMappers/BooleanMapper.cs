namespace Domain.Repository.InMemory.TypeMappers;

public class BooleanMapper : ITypeMapper<bool>
{
    public string TypeName { get; } = "INTEGER";

    public bool IsNullable => false;

    public string DefaultValue => "DEFAULT 0";

    public bool FromDatabase(string data)
        => Convert.ToBoolean(data);

    public string ToDatabase(bool data)
        => data.ToString();
}
