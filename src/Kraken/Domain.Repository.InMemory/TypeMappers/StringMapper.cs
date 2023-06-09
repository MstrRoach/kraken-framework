namespace Domain.Repository.InMemory.TypeMappers;

public class StringMapper : ITypeMapper<string>
{
    public string TypeName { get; } = "TEXT";

    public bool IsNullable => true;

    public string DefaultValue => "DEFAULT null";

    public bool IsAutoIncrement => false;

    public string FromDatabase(string data)
        => data;

    public string FromDatabase(object data)
        => Convert.ToString(data);

    public string ToDatabase(string data)
        => data;
}
