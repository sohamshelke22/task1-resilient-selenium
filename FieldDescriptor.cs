public class FieldDescriptor
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string[] Keywords { get; set; }

    public FieldDescriptor(string name, string type, string[] keywords)
    {
        Name = name;
        Type = type;
        Keywords = keywords;
    }
}
