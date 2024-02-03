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
    public string ImageIdMetadata { get; set; }
    public long CreationDate { get; set; }

    public ItemRemote()
    {

    }

    public ItemRemote(string name, string path, string imageIdMetadata)
    {
        Name = name;
        Path = path;
        ImageIdMetadata = imageIdMetadata;
    }

    public ItemRemote(string id, string name, string path, string imageIdMetadata, long creationDate)
    {
        Id = id;
        Name = name;
        Path = path;
        ImageIdMetadata = imageIdMetadata;
        CreationDate = creationDate;
    }

   

    public Dictionary<string, Object> ToDictionary()
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();
        result["id"] = Id;
        result["name"] = Name;
        result["path"] = Path;
        result["image_id_metadata"] = ImageIdMetadata;
        result["creation_date"] = CreationDate;

        return result;
    }
}
