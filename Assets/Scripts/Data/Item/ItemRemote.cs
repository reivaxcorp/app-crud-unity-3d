/// <summary>
/// El item que salvaremos remotamente.
/// </summary>
public class ItemRemote 
{
    public string Id {  get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Timestamp { get; set; }

    public ItemRemote()
    {

    }

    public ItemRemote(string id, string name, string path, string timestamp)
    {
        Id = id;
        Name = name;
        Path = path;
        Timestamp = timestamp;
    }
}
