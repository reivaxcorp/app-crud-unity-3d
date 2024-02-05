
public class ItemLocalTest 
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string ImageIdMetadata { get; set; }
    public long CreationDate { get; set; }

    public ItemLocalTest() { }

    public ItemLocalTest(string id, string name, string path, string imageIdMetadata, long creationDate)
    {
        Id = id;
        Name = name;
        Path = path;
        ImageIdMetadata = imageIdMetadata;
        CreationDate = creationDate;
    }
}
