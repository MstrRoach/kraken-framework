namespace Support.Domain.Entities;

internal class Priority
{
    public static Priority Low = new(400, "Low");
    public static Priority Medium = new(401, "Medium");
    public static Priority High = new(402, "High");
    public int Id { get; set; }
    public string Name { get; set; }

    public Priority(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
