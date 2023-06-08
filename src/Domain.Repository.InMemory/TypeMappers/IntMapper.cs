﻿namespace Domain.Repository.InMemory.TypeMappers;

public class IntMapper : ITypeMapper<int>
{
    public string TypeName { get; } = "INTEGER";

    public bool IsNullable => false;

    public string DefaultValue => "DEFAULT 0";

    public int FromDatabase(string data)
    {
        return Convert.ToInt32(data);
    }

    public string ToDatabase(int data)
    {
        return data.ToString();
    }
}
