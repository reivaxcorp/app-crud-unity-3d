
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
    public long Timestamp { get; set; }

    public ItemLocal() { }
    public ItemLocal(string id, string name, string path, long timestamp)
    {
        Id = id;
        Name = name;
        Path = path;
        Timestamp = timestamp;
    }
}

