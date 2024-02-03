
/// <summary>
/// El item que salvaremos localemente.
/// </summary>
/// 
[System.Serializable]
public class ItemLocal
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string ImageIdMetadata { get; set; }
    public long CreationDate { get; set; }

    public ItemLocal() { }

    public ItemLocal(string id, string name, string path, string imageIdMetadata, long creationDate)
    {
        Id = id;
        Name = name;
        Path = path;
        ImageIdMetadata = imageIdMetadata;
        CreationDate = creationDate;
    }
}

