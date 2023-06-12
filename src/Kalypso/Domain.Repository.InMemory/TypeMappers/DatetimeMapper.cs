namespace Dottex.Domain.Repository.InMemory.TypeMappers;

public class DatetimeMapper : ITypeMapper<DateTime>
{
    public string TypeName { get; } = "TEXT";

    public bool IsNullable => false;

    public string DefaultValue => "DEFAULT '$VALUE'"
        .Replace("$VALUE", new DateTime(2000, 01, 01).ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF"));

    public bool IsAutoIncrement => false;


    public DateTime FromDatabase(string data)
        => Convert.ToDateTime(data);

    public DateTime FromDatabase(object data)
        => Convert.ToDateTime(data);

    public string ToDatabase(DateTime data)
        => data.ToString();
}
