﻿namespace Dottex.Domain.Repository.InMemory.TypeMappers;

public class BooleanMapper : ITypeMapper<bool>
{
    public string TypeName { get; } = "INTEGER";

    public bool IsNullable => false;

    public string DefaultValue => "DEFAULT 0";

    public bool IsAutoIncrement => false;

    public bool FromDatabase(string data)
        => Convert.ToBoolean(data);

    public bool FromDatabase(object data)
        => Convert.ToBoolean(data);

    public string ToDatabase(bool data)
        => data.ToString();
}
