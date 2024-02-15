
/// <summary>
/// El item que salvaremos localemente.
/// </summary>
/// 
[System.Serializable]
public class ItemLocal
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ImageName { get; set; }
    public long CreationDate { get; set; }

    public ItemLocal() { }

    public ItemLocal(string id, string name, string imageName, long creationDate)
    {
        Id = id;
        Name = name;
        ImageName = imageName;
        CreationDate = creationDate;
    }
}

