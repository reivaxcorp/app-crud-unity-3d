
public class ItemLocalTest 
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string ImageName { get; set; }
    public long CreationDate { get; set; }

    public ItemLocalTest() { }

    public ItemLocalTest(string id, string name, string imageName, long creationDate)
    {
        Id = id;
        Name = name;
        ImageName = imageName;
        CreationDate = creationDate;
    }
}
