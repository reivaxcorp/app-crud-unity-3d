using System.Collections.Generic;
using System;

public class ItemRemoteTest 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ImageName { get; set; }
    public long CreationDate { get; set; }

    public ItemRemoteTest(){}

    public ItemRemoteTest(string name, string imageName)
    {
        Name = name;
        ImageName = imageName;
    }

    public ItemRemoteTest(string id, string name, string imageName, long creationDate)
    {
        Id = id;
        Name = name;
        ImageName = imageName;
        CreationDate = creationDate;
    }

    public Dictionary<string, Object> ToDictionary()
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();
        result["id"] = Id;
        result["name"] = Name;
        result["image_name"] = ImageName;
        result["creation_date"] = CreationDate;

        return result;
    }
}
