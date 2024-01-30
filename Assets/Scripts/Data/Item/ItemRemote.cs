using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
/// <summary>
/// El item que salvaremos remotamente.
/// </summary>
public class ItemRemote 
{
    public string Id {  get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public long Timestamp { get; set; }

    public ItemRemote()
    {

    }

    public ItemRemote(string id, string name, string path, long timestamp)
    {
        Id = id;
        Name = name;
        Path = path;
        Timestamp = timestamp;
    }

    public Dictionary<string, Object> ToDictionary()
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();
        result["id"] = Id;
        result["name"] = Name;
        result["path"] = Path;
        result["timestamp"] = Timestamp;

        return result;
    }
}
