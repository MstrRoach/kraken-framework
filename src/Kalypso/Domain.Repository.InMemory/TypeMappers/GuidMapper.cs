namespace Dottex.Domain.Repository.InMemory.TypeMappers;

public class GuidMapper : ITypeMapper<Guid>
{
    public string TypeName { get; } = "TEXT";

    public bool IsNullable => false;

    public string DefaultValue => "DEFAULT '00000000-0000-0000-0000-000000000000'";

    public bool IsAutoIncrement => false;

    public Guid FromDatabase(string data)
        => Guid.Parse(data);
    public Guid FromDatabase(object data)
        => FromDatabase(Convert.ToString(data));

    public string ToDatabase(Guid data)
    {
        return data.ToString();
    }
}
