using System.Collections.Generic;
using System;

public class ItemRemoteTest 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string ImageIdMetadata { get; set; }
    public long CreationDate { get; set; }

    public ItemRemoteTest()
    {

    }

    public ItemRemoteTest(string name, string path, string imageIdMetadata)
    {
        Name = name;
        Path = path;
        ImageIdMetadata = imageIdMetadata;
    }

    public ItemRemoteTest(string id, string name, string path, string imageIdMetadata, long creationDate)
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
